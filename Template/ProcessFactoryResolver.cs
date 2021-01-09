using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Template
{
    public static class ProcessFactoryResolver
    {
        public static SnatchProcessTemplate Resolve(string endpointName, ILogger<Worker> logger, IConfiguration config)
        {
            return endpointName switch
            {
                "canadacomputers" => new CanadaComputersProcess(logger, config),
                _ => null,
            };
        }
    }
}
