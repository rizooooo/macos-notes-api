using MongoDB.Bson;
using MongoDB.Driver;

namespace NotesDotnet.Config
{
    public class MongoDbContext
    {
        const string connectionUri =
            "mongodb+srv://drizojose0396:<db_password>@cluster0.ao32yev.mongodb.net/?appName=Cluster0";

        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        // settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        // // Create a new client and connect to the server
        // var client = new MongoClient(settings);
        public MongoDbContext()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            var client = new MongoClient(settings);

            // Send a ping to confirm a successful connection
            try
            {
                var result = client
                    .GetDatabase("admin")
                    .RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
