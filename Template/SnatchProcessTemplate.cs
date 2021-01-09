using Discord;
using Discord.Webhook;
using gpu_snatcher.Models;
using gpu_snatcher.Models.Schemas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Template
{
    public abstract class SnatchProcessTemplate
    {
        private readonly DiscordWebhookClient _webhookClient;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;

        protected EndpointSchema Endpoint;
        protected Browser Browser;

        public SnatchProcessTemplate(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _webhookClient = new DiscordWebhookClient(config.GetSection("Worker")["webhookUri"]);
        }

        public async Task ExecuteProcessAsync(List<ProductSchema> products, EndpointSchema endpoint)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = new[] {
                    "--no-sandbox"
                }
            });

            _logger.LogInformation("Launched Browser.");

            Endpoint = endpoint;

            foreach (var product in products)
            {
                try
                {
                    List<EndpointItem> results = new List<EndpointItem>();

                    if (product.ProductUrls.Count > 0)
                    {
                        var productUrl = product.ProductUrls.Find(x => x.Endpoint == Endpoint.Name);

                        if (productUrl != null)
                        {
                            _logger.LogInformation($"Getting results for Product ({product.Title}) by Custom URL.");
                            results = await GetURLResults(productUrl.URL);
                        }
                    } 
                    else
                    {
                        _logger.LogInformation($"Getting results for Product ({product.Title}) by Query string Search.");
                        results = await GetQueryResults(product.Title);
                    }

                    if (results.Count < 1)
                        throw new Exception($"No Results Found for Product ({product.Title}).");

                    foreach (var res in results)
                    {
                        var Embed = new List<Embed>() { BuildEmbed(res) }; // library takes IEnumerable, but discord only takes 1 result
                        await _webhookClient.SendMessageAsync($"<@&{_config.GetSection("Worker")["discordRoleId"]}>", false, Embed);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex.ToString());
                    continue;
                }
                finally
                {
                    await Browser.CloseAsync();
                    _logger.LogInformation("Closed Browser.");
                }
            }
        }

        private static Embed BuildEmbed(EndpointItem item)
        {
            var builder = new EmbedBuilder()
            {
                Title = item.Title,
                ThumbnailUrl = item.ImageUrl,
                Color = Color.Blue,
                Timestamp = DateTime.Now
            };

            builder.AddField("Available", $"{item.Available}");
            builder.AddField("Price", $"{item.Price}$");
            builder.AddField("Link", item.PageUrl);

            return builder.Build();
        }

        protected abstract Task<List<EndpointItem>> GetQueryResults(string query);
        protected abstract Task<List<EndpointItem>> GetURLResults(string URL);
        protected abstract void SnatchProduct(string query);
    }
}
