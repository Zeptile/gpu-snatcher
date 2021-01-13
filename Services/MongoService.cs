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
        private readonly IMongoCollection<InStockSchema> _instocks;
        private readonly IMongoCollection<EndpointSchema> _endpoints;

        public MongoService(IConfiguration config)
        {
            var client = new MongoClient(config.GetSection("connectionStrings")["snatcher"]);
            var database = client.GetDatabase("snatcher");

            _products = database.GetCollection<ProductSchema>("gsn_products");
            _instocks = database.GetCollection<InStockSchema>("gsn_instocks");
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

        public async Task<List<InStockSchema>> GetAllInStocks()
        {
            return (await _instocks.FindAsync(s => true)).ToList();
        }

        public async Task<List<InStockSchema>> GetInStockByEndpoint(string endpoint)
        {
            return (await _instocks.FindAsync(s => s.EndpointName == endpoint)).ToList();
        }

        public async Task PostInStock(InStockSchema item)
        {
            await _instocks.InsertOneAsync(item);
        }

        public async Task DeleteInStock(string URL)
        {
            await _instocks.DeleteOneAsync(s => s.URL == URL);
        }
    }
}
