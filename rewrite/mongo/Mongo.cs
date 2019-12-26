using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace Orikivo
{
    public class Mongo
    {
        public static async Task TestAsync()
        {
            MongoUrl connection = MongoUrl.Create("mongodb://localhost:27017");

            MongoClient client = new MongoClient(connection);

            IMongoDatabase userDb = client.GetDatabase("main");

            await userDb.CreateCollectionAsync("users");
            

        }
    }

    // handles all methods needed with MongoDB
    public static class MongoHandler
    {

    }

    // stores all stuff related to mongoDB
    public class MongoContainer
    {

    }
}
