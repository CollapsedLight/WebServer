using MongoDB.Driver;

namespace CollapsedLight.WebApp.Service
{
    public class MongoDatabaseHandler
    {
        private MongoClient _client { get; }

        public MongoDatabaseHandler()
        {
            _client = new MongoClient(Credentials.MongoDb);
        }

        public IMongoDatabase WebApp
            => _client.GetDatabase("webapp");

        public IMongoDatabase Gardening
            => _client.GetDatabase("gardening");

    }
}
