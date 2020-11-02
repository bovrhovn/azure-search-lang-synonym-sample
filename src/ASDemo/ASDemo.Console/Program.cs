using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

namespace ASDemo.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceName = Environment.GetEnvironmentVariable("AzureSearchName");
            var indexName = "models";
            var apiKey = Environment.GetEnvironmentVariable("AzureSearchKey");

            var serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            var credential = new AzureKeyCredential(apiKey);
            var searchIndexClient = new SearchIndexClient(serviceEndpoint, credential);
            var searchClient = new SearchClient(serviceEndpoint, indexName, credential);

            System.Console.WriteLine("-- Demo start ---");
            var index = new SearchIndex(indexName)
            {
                Fields =
                {
                    new SimpleField("uniqueId", SearchFieldDataType.String)
                        {IsKey = true, IsFilterable = true, IsSortable = true},
                    new SearchableField("name") {IsFilterable = true, IsSortable = true},
                    new SearchableField("polishName") {IsFilterable = true, IsSortable = true},
                    new SimpleField("updated", SearchFieldDataType.DateTimeOffset)
                        {IsFilterable = true, IsSortable = true},
                    new SearchableField("keyPhrases") {IsFilterable = true, IsSortable = true}
                }
            };
            
            System.Console.WriteLine("Creating index....");
            await searchIndexClient.CreateOrUpdateIndexAsync(index);
            System.Console.WriteLine("Index is created");

            await AddDataAsync(searchClient);
            await QueryDataAsync(searchClient);
            System.Console.WriteLine("-- Demo finished --");
            System.Console.WriteLine("--> press any field to exit");
            System.Console.Read();
        }

        private static async Task QueryDataAsync(SearchClient searchClient)
        {
            System.Console.WriteLine("Query #1: Find css in all fields...");
            var options = new SearchOptions
            {
                Filter = "",
                OrderBy = { "Name" }
            };

            var response = await searchClient.SearchAsync<SearchModel>("css", options);
            WriteDocuments(response);

            System.Console.WriteLine("Query #2: search with OData filters");

            options = new SearchOptions
            {
                Filter = "name eq 'css'",
            };

            response = await searchClient.SearchAsync<SearchModel>("*", options);
            WriteDocuments(response);

            System.Console.WriteLine("Query #3: search with polish version");

            options = new SearchOptions
            {
                Filter = "",
                OrderBy = { "updated desc" }
            };

            response = await searchClient.SearchAsync<SearchModel>("magnesow", options);
            WriteDocuments(response);
        }
        
        private static async Task AddDataAsync(SearchClient searchClient)
        {
            var translator = new Translator();

            string modelName1 = "external styles are defined withing the element, inside the section of an HTML page";
            string polishModelName1 = await translator.GetTranslatedTextAsync(modelName1);
            string[] keyWords1 = new[] {"CSS", "styles","metadata","element","EF01"};
            string modelName2 = "To include an external JavaScript file, use the script tag with the attribute src";
            string polishModelName2 = await translator.GetTranslatedTextAsync(modelName2);
            string[] keyWords2 = new[] {"javascript", "EF02","script","tag","src"};
            string modelName3 = "Move to the home position";
            string polishModelName3 = await translator.GetTranslatedTextAsync(modelName3);
            string[] keyWords3 = new[] {"storage", "container","EF03","home"};
            string modelName4 = "removal of stuck magnets restores machine cycle";
            string polishModelName4 = await translator.GetTranslatedTextAsync(modelName4);
            string[] keyWords4 = new[] {"magnets", "machine","cycle","EF04"};

            var batch = IndexDocumentsBatch.Create(
                IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(), Name = modelName1, PolishName = polishModelName1,
                    KeyPhrases = keyWords1, Updated = new DateTime(2020, 10, 1, 7, 0, 0)
                })
                , IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName2,
                    KeyPhrases = keyWords2,
                    PolishName = polishModelName2,
                    Updated = new DateTime(2020, 9, 2, 8, 54, 0)
                }), IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName4,
                    KeyPhrases = keyWords4,
                    PolishName = polishModelName4,
                    Updated = new DateTime(2020, 8, 2, 12, 11, 0)
                }), IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName3,
                    KeyPhrases = keyWords3,
                    PolishName = polishModelName3,
                    Updated = new DateTime(2020, 7, 2, 21, 21, 0)
                }));

            await searchClient.IndexDocumentsAsync(batch, new IndexDocumentsOptions {ThrowOnAnyError = true});
        }
        
        private static void WriteDocuments(SearchResults<SearchModel> searchResults)
        {
            foreach (var response in searchResults.GetResults())
            {
                var doc = response.Document;
                var score = response.Score;
                System.Console.WriteLine($"ID: {doc.Id}\tValue: {doc.Name}\tPolish: {doc.PolishName}");
            }

            System.Console.WriteLine("");
        }
    }
}