using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace gpu_snatcher.Models.Schemas
{
    public class EndpointSchema
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("searchUrl")]
        public string SearchURL { get; set; }
    }
}
