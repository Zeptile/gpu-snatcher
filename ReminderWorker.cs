using Discord.Webhook;
using gpu_snatcher.Helpers;
using gpu_snatcher.Services;
using gpu_snatcher.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace gpu_snatcher
{
    public class ReminderWorker : BackgroundService
    {
        private readonly ILogger<ReminderWorker> _logger;
        private readonly IConfiguration _config;

        private readonly IMongoService _mongoService;
        private readonly DiscordWebhookClient _webhookClient;

        public ReminderWorker(ILogger<ReminderWorker> logger, IMongoService mongoService, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _mongoService = mongoService;
            _webhookClient = new DiscordWebhookClient(config.GetSection("Workers")["webhookUri"]);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var instocks = await _mongoService.GetAllInStocks();

                    if (instocks != null && instocks.Count > 0)
                    {
                        var roleId = _config.GetSection("Workers")["discordRoleId"];
                        await _webhookClient.SendMessageAsync($"<@&{roleId}> Remember that these items are still in stock!", false);
                        Parallel.ForEach(instocks, async instock =>
                        {
                            _logger.LogInformation($"Opening new thread for Product ({instock.EndpointItem.Title}) <ThreadID={Thread.CurrentThread.ManagedThreadId}>.");

                            await _webhookClient.SendMessageAsync("", false, DiscordHelpers.BuildEmbed(instock.EndpointItem));
                        });
                    }

                    _logger.LogInformation($"No Product are in stock currently.");

                }
                catch (Exception e)
                {
                    _logger.LogError($"Snatcher Worker failed unexpectedly => {e}", e);
                    // throw;
                }

                // 3 Days by Default
                await Task.Delay(int.Parse(_config.GetSection("Workers")["reminderInterval"]), stoppingToken);
            }
        }
    }
}