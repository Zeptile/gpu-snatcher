using gpu_snatcher.Models.Schemas;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Services
{
    public class MongoService : IMongoService
    {
        private readonly IMongoCollection<ProductSchema> _products;
        private readonly IMongoCollection<EndpointSchema> _endpoints;

        public MongoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("connectionStrings")["snatcher"]);
            var database = client.GetDatabase("snatcher");

            _products = database.GetCollection<ProductSchema>("gsn_products");
            _endpoints = database.GetCollection<EndpointSchema>("gsn_endpoints");
        }

        public async Task<List<EndpointSchema>> GetAllEndpoints()
        {
            return (await _endpoints.FindAsync(s => true)).ToList();
        }

        public async Task<List<ProductSchema>> GetAllProducts()
        {
            return (await _products.FindAsync(s => true)).ToList();
        }
    }
}
