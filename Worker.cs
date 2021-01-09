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
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;

        private readonly IMongoService _mongoService;

        public Worker(ILogger<Worker> logger, IMongoService mongoService, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _mongoService = mongoService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var endpoints = await _mongoService.GetAllEndpoints();
                    var products = await _mongoService.GetAllProducts();

                    foreach (var endpoint in endpoints)
                    {
                        var process = ProcessFactoryResolver.Resolve(endpoint.Name, _logger, _config);

                        if (process == null)
                        {
                            _logger.LogWarning($"No Process is implemented for endpoint ({endpoint.Name}).");
                            continue;
                        }
                            
                        await process.ExecuteProcessAsync(products, endpoint);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"Snatcher Worker failed unexpectedly => {e}", e);
                    // throw;
                }

                // 60 Seconds by Default
                await Task.Delay(int.Parse(_config.GetSection("Worker")["interval"]), stoppingToken);
            }
        }
    }
}
