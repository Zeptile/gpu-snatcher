using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gpu_snatcher.Models.Schemas
{
    public class ProductSchema
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("productUrls")]
        public List<ProductURL> ProductUrls { get; set; }

        [BsonElement("productType")]
        public ProductType ProductType { get; set; }
    }

    public class ProductURL
    {
        [BsonElement("endpoint")]
        public string Endpoint { get; set; }

        [BsonElement("url")]
        public string URL { get; set; }
    }

    public enum ProductType
    {
        GPU,
        CPU,
        MBO,
        PSU,
        RAM,
        HDD,
        SSD
    }
}
