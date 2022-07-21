using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CollapsedLight.WebApp.Data;

[BsonIgnoreExtraElements]
public record MileageDataRecord
{
    public ObjectId Id { get; init; }
    public DateTime Date { get; init; } 
    public double Fill { get; set; }
    public double Distance { get; set; }
    public double Price { get; set; }
    public double Consumption { get; set; }

    public MileageDataRecord(double fill, double distance, double price, double consumption)
    {
        Date = DateTime.Now;
        Fill = fill;
        Distance = distance;   
        Price = price;
        Consumption = consumption;
    }

    public MileageDataRecord(DateTime date, double fill, double distance, double price, double consumption)
    {
        Date = date;
        Fill = fill;
        Distance = distance;   
        Price = price;
        Consumption = consumption;
    }

 }


