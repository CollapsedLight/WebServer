using MongoDB.Bson;
using MongoDB.Driver;

namespace CollapsedLight.WebApp.Service;

public class FuelService : IFuelService
{
    private readonly IMongoCollection<MileageDataRecord> _collection;

    public FuelService(MongoDatabaseHandler client)
    {
        _collection = client.WebApp.GetCollection<MileageDataRecord>("Mileage");
    }

    public IEnumerable<MileageDataRecord?> GetAll()
    {
        var filter = Builders<MileageDataRecord>.Filter.Lt(x => x.Date, DateTime.Now);
        var cursor = _collection.Find(filter).ToCursor();
        return cursor.ToEnumerable<MileageDataRecord>();
    }

    public IEnumerable<MileageDataRecord?> Get(int count)
    {
        return _collection.Find(new BsonDocument()).Limit(count).ToEnumerable(); 
    }

    public void PushRecord(MileageDataRecord record)
    {
        _collection.InsertOne(record); ;
    }
}
