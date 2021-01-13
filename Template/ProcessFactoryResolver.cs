using gpu_snatcher.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace gpu_snatcher.Template
{
    public static class ProcessFactoryResolver
    {
        public static SnatchProcessTemplate Resolve(string endpointName, ILogger<ScraperWorker> logger, IConfiguration config, IMongoService mongoService)
        {
            return endpointName switch
            {
                "canadacomputers" => new CanadaComputersProcess(logger, config, mongoService),
                _ => null,
            };
        }
    }
}
