using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace gpu_snatcher.Models.Schemas
{
    public class InStockSchema
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("url")]
        public string URL { get; set; }

        [BsonElement("endpoint")]
        public string EndpointName { get; set; }

        [BsonElement("endpointItem")]
        public EndpointItem EndpointItem { get; set; }
    }
}
