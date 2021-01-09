using gpu_snatcher.Helpers;
using gpu_snatcher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Template
{
    public class CanadaComputersProcess : SnatchProcessTemplate
    {
        private readonly ILogger<Worker> _logger;
        public CanadaComputersProcess(ILogger<Worker> logger, IConfiguration config) : base (logger, config)
        {
            _logger = logger;
        }

        protected override async Task<List<EndpointItem>> GetURLResults(string URL)
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.GoToAsync(URL);

                _logger.LogInformation($"Loaded Custom URL ({URL}).");

                var results = await page.EvaluateFunctionAsync<List<EndpointItem>>(InjectionScripts.CanadaComputersGetAll);

                return results.Where(x => x.Available).ToList();
            }
        }

        protected override async Task<List<EndpointItem>> GetQueryResults(string query) { throw new NotImplementedException(); }
        protected override void SnatchProduct(string query) { throw new NotImplementedException(); }
    }
}