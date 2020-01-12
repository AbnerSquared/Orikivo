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
            // something like DatabaseUser
            await userDb.CreateCollectionAsync("users"); // TODO: Use mongo instead of JSON.
            // edit the users directly from the database to simply stuff, and make a class that handles that directly.
            

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
