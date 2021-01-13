using gpu_snatcher.Models.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Services
{
    public interface IMongoService
    {
        Task<List<EndpointSchema>> GetAllEndpoints();
        Task<List<ProductSchema>> GetAllProducts();
        Task<List<InStockSchema>> GetAllInStocks();
        Task<List<InStockSchema>> GetInStockByEndpoint(string endpoint);
        Task PostInStock(InStockSchema item);
        Task DeleteInStock(string URL);
    }
}
