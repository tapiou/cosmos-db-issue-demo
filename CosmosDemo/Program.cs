using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace CosmosDemo
{
    public class Program
    {
        private static readonly string endpointUrl = "https://localhost:8081";
        private static readonly string authorizationKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private static readonly string databaseId = "demodb";
        private static readonly string collectionId = "democollection";

        private static DocumentClient client;

        private static readonly FeedOptions feedOptions = new FeedOptions
        {
            EnableCrossPartitionQuery = true
        };

        public static void Main(string[] args)
        {
            try
            {
                using (client = new DocumentClient(new Uri(endpointUrl), authorizationKey))
                {
                    // To run this console application, create local cosmos db and collection with Azure Cosmos DB Emulator             

                    UpsertDemoDocuments().Wait();

                    var uri = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

                    var rows = client.CreateDocumentQuery<DocumentModel>(uri, feedOptions)
                        .Where(i => i.amount < 0m)
                        .ToList();

                    foreach (var row in rows)
                    {
                        string json = JsonConvert.SerializeObject(row, Formatting.Indented);
                        Console.WriteLine(json);
                    }
                }
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            Console.ReadKey();
        }

        private static async Task UpsertDemoDocuments()
        {
            var docs = new List<DocumentModel>();

            docs.Add(new DocumentModel
            {
                id = Guid.NewGuid().ToString(),
                amount = -2m,
                sizeClass = "negative"
            });
            docs.Add(new DocumentModel
            {
                id = Guid.NewGuid().ToString(),
                amount = 123m,
                sizeClass = "small"
            });
            docs.Add(new DocumentModel
            {
                id = Guid.NewGuid().ToString(),
                amount = 123123m,
                sizeClass = "large"
            });
            docs.Add(new DocumentModel
            {
                id = Guid.NewGuid().ToString(),
                amount = -132.21m,
                sizeClass = "negative"
            });
            docs.Add(new DocumentModel
            {
                id = Guid.NewGuid().ToString(),
                amount = 123.23m,
                sizeClass = "small"
            });
            docs.Add(new DocumentModel
            {
                id = Guid.NewGuid().ToString(),
                amount = 123123.1237127m,
                sizeClass = "large"
            });

            var collectionLink = UriFactory.CreateDocumentCollectionUri(databaseId, collectionId);

            foreach (var doc in docs)
            {
                var response = await client.UpsertDocumentAsync(collectionLink, doc);
                Console.WriteLine(response.StatusCode);
            }
        }
    }
}
