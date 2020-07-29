using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using Orikivo.Desync;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Orikivo
{
    public class Mongo
    {
        public static async Task InitializeAsync()
        {
        }

        public static async Task TestAsync()
        {
            MongoClient client = new MongoClient();
            IMongoDatabase database = client.GetDatabase("data");

            foreach (var item in client.ListDatabases().ToList())
            {
                Console.WriteLine(item);
            }

            var users = database.GetCollection<TestMongoObject>("test");

            //if (users == null)
            //{
            //    database.CreateCollection("test"); // TODO: Use MongoDB instead of JSON.
           //     users = database.GetCollection<TestMongoObject>("test");
           // }
            // edit the users directly from the database to simply stuff, and make a class that handles that directly.

            var value = new TestMongoObject("test_id", "default_text");

            //var filter = Builders<TestMongoObject>.Filter.Eq("id", "test_id");
            //var update = Builders<TestMongoObject>.Update.Set("content", "text");

            users.InsertOne(value);
            Console.WriteLine(users.Find(x => x.Id == "test_id"));
        }
    }

    [BsonDiscriminator("testmongoobject")]
    public class TestMongoObject
    {
        [BsonConstructor]
        public TestMongoObject(string id, string content)
        {
            Id = id;
            Content = content;
        }

        [BsonId]
        public string Id { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }
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
