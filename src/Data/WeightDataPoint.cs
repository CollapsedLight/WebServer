using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollapsedLight.WebApp.Data;

[BsonIgnoreExtraElements]
public record WeightDataPoint
{
    public ObjectId Id { get; set; }

    [BsonElement("dateTime")]
    public DateTime DateTime { get; set; }
    public float Weight { get; set; }

    public WeightDataPoint(DateTime date, float weight)
    {
        DateTime = date;
        Weight = weight;
    }

    public WeightDataPoint()
        : this(DateTime.UtcNow,0)
    {}
}
