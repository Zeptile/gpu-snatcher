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
    public class AmazonProcess : SnatchProcessTemplate
    {
        private readonly ILogger<Worker> _logger;
        public AmazonProcess(ILogger<Worker> logger, IConfiguration config) : base (logger, config)
        {
            _logger = logger;
        }

        protected override Task<List<EndpointItem>> GetQueryResults(string query)
        {
            throw new NotImplementedException();
        }
         
        protected override Task<List<EndpointItem>> GetURLResults(string URL)
        {
            throw new NotImplementedException();
        }

        protected override void SnatchProduct(string query)
        {
            throw new NotImplementedException();
        }
    }
} 
