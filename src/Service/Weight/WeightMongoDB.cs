using MongoDB.Driver;

namespace CollapsedLight.WebApp.Service
{
    public class WeightMongoDB : IWeightDataService, IDisposable
    {
        private readonly IMongoCollection<WeightDataPoint> _collection;

        public WeightMongoDB(MongoDatabaseHandler database)
        {
            _collection = database.WebApp.GetCollection<WeightDataPoint>("Weight");
        }

        public void AddWeight(float value)
        {
            _collection.InsertOne(new WeightDataPoint(DateTime.UtcNow, value));
        }

        public void Dispose()
        {
        }

        public IEnumerable<WeightDataPoint> GetAll()
        {
            var filter = Builders<WeightDataPoint>.Filter.Lt(x => x.DateTime, DateTime.Now);
            var cursor = _collection.Find(filter).ToCursor();
            return cursor.ToList();
        }

        public IEnumerable<float> GetWeight(TimeSpan timespan)
        {
            var filter = Builders<WeightDataPoint>.Filter.Gt(x => x.DateTime, DateTime.Now.Subtract(timespan));
            var cursor = _collection.Find(filter).ToCursor();
            return cursor.ToList().Select(x => x.Weight);
        }
    }
}
