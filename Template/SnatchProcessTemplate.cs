using Discord;
using Discord.Webhook;
using gpu_snatcher.Helpers;
using gpu_snatcher.Models;
using gpu_snatcher.Models.Schemas;
using gpu_snatcher.Services;
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
        private readonly ILogger<ScraperWorker> _logger;
        private readonly IConfiguration _config;
        private readonly IMongoService _mongoService;

        private readonly string RoleId;

        protected EndpointSchema Endpoint;
        protected Browser Browser;

        public SnatchProcessTemplate(ILogger<ScraperWorker> logger, IConfiguration config, IMongoService mongoService)
        {
            _logger = logger;
            _config = config;
            _mongoService = mongoService;
            _webhookClient = new DiscordWebhookClient(config.GetSection("Workers")["webhookUri"]);

            RoleId = config.GetSection("Workers")["discordRoleId"];
        }

        public async Task ExecuteProcessAsync(List<ProductSchema> products, EndpointSchema endpoint)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
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

                    var currentInStocks = await _mongoService.GetInStockByEndpoint(Endpoint.Name);
                    var currentInStockItems = currentInStocks.Select(x => x.EndpointItem).ToList(); // Mapping to a SubList

                    if (results.Count < currentInStockItems.Count)
                    {
                        var missingItems = currentInStockItems.Except(results);

                        foreach (var item in missingItems)
                        {
                            await _mongoService.DeleteInStock(item.PageUrl);
                            item.Available = false;
                            await _webhookClient.SendMessageAsync($"<@&{RoleId}> This item is now out of Stock!", false, DiscordHelpers.BuildEmbed(item));
                        }
                    }

                    if (results.Count < 1)
                        throw new Exception($"No Results Found for Product ({product.Title}).");

                    foreach (var res in results)
                    {
                        var instock = currentInStocks.Find(x => x.URL == res.PageUrl);

                        if (instock == null)
                        {
                            await _mongoService.PostInStock(new InStockSchema()
                            {
                                URL = res.PageUrl,
                                EndpointName = Endpoint.Name,
                                EndpointItem = res
                            });

                            await _webhookClient.SendMessageAsync($"<@&{RoleId}>", false, DiscordHelpers.BuildEmbed(res));
                        }
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex.ToString());
                    continue;
                }
            }

            await Browser.CloseAsync();
            _logger.LogInformation("Closed Browser.");
        }

        protected abstract Task<List<EndpointItem>> GetQueryResults(string query);
        protected abstract Task<List<EndpointItem>> GetURLResults(string URL);
        protected abstract void SnatchProduct(string query);
    }
}
