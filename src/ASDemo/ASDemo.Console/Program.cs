using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Spectre.Console;

namespace ASDemo.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceName = Environment.GetEnvironmentVariable("AzureSearchName");
            var indexName = "modelmaps";
            var apiKey = Environment.GetEnvironmentVariable("AzureSearchKey");

            var serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            var credential = new AzureKeyCredential(apiKey);
            var searchIndexClient = new SearchIndexClient(serviceEndpoint, credential);
            var searchClient = new SearchClient(serviceEndpoint, indexName, credential);

            if (AnsiConsole.Capabilities.SupportLinks)
                AnsiConsole.MarkupLine(
                    $"[link=https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/main/src/ASDemo]Demo for index {indexName}[/]!");

            HorizontalRule("Creating index with synonym maps");

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
                    new SearchableField("keyPhrases", true) {IsFilterable = true}
                }
            };

            AnsiConsole.WriteLine("Creating or updating index, if changed");
            await searchIndexClient.CreateOrUpdateIndexAsync(index);
            AnsiConsole.WriteLine("Index updated, off to synonym maps!");

            // applying synonym maps to get the same result
            if (searchIndexClient.GetSynonymMapNames().Value.Contains("name-map"))
                searchIndexClient.DeleteSynonymMap("name-map");
            
            //it is separated only by \n due to SOLR format
            string nameSynonyms = "css=>design\nsource,src\ndata=>storage";
            var synonmMap = new SynonymMap("name-map", nameSynonyms);
            await searchIndexClient.CreateOrUpdateSynonymMapAsync(synonmMap);

            var addingData = Environment.GetEnvironmentVariable("AddingData");
            bool.TryParse(addingData, out bool addData);

            if (addData)
            {
                HorizontalRule("Adding data to the index");
                AnsiConsole.WriteLine($"Adding data to the search index in {indexName}");
                await AddDataAsync(searchClient);
                AnsiConsole.WriteLine("Completed adding data");
            }

            HorizontalRule("Executing search with(out) synonym maps");
            AnsiConsole.WriteLine("Query #1: Searching for data value (without synonyms)");

            var queryManager = new QueryManager();
            var response = await queryManager.QueryDataAsync(searchClient, "data", new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            });

            WriteDocuments(response);

            HorizontalRule("Query #2: Searching for data value (with synonyms)");
            index.Fields.First(d => d.Name == "name").SynonymMapNames.Add("name-map");
            index.Fields.First(d => d.Name == "keyPhrases").SynonymMapNames.Add("name-map");
            await searchIndexClient.CreateOrUpdateIndexAsync(index);
            AnsiConsole.WriteLine("Index updated with Synonym Map....");
            
            response = await queryManager.QueryDataAsync(searchClient, "data", new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            });

            WriteDocuments(response);

            HorizontalRule("Query #3: Searching for design value - it doesn't exists (with synonyms)");
            response = await queryManager.QueryDataAsync(searchClient, "design", new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            });
            
            WriteDocuments(response);
            
            AnsiConsole.WriteLine("--> press any field to exit");
            System.Console.Read();
        }

        private static async Task AddDataAsync(SearchClient searchClient)
        {
            var translator = new Translator();

            string modelName1 = "external styles are defined withing the element, inside the section of an HTML page";
            string polishModelName1 = await translator.GetTranslatedTextAsync(modelName1);
            string[] keyWords1 = {"CSS", "styles", "metadata", "element", "EF01"};
            string modelName2 = "To include an external JavaScript file, use the script tag with the attribute src";
            string polishModelName2 = await translator.GetTranslatedTextAsync(modelName2);
            string[] keyWords2 = {"javascript", "EF02", "script", "tag", "src"};
            string modelName3 = "Move to the home position";
            string polishModelName3 = await translator.GetTranslatedTextAsync(modelName3);
            string[] keyWords3 = {"storage", "container", "EF03", "home"};
            string modelName4 = "removal of stuck magnets restores machine cycle";
            string polishModelName4 = await translator.GetTranslatedTextAsync(modelName4);
            string[] keyWords4 = {"magnets", "machine", "cycle", "EF04"};

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
            var table = new Table()
                .Border(TableBorder.Square)
                .BorderColor(Color.Red)
                .Title("Search results")
                .AddColumn(new TableColumn("[u]ID[/]").Centered())
                .AddColumn(new TableColumn("[u]Value[/]"))
                .AddColumn(new TableColumn("[u]Polish[/]"));

            foreach (var response in searchResults.GetResults())
            {
                var doc = response.Document;
                var score = response.Score;
                table.AddRow(new Text(doc.Id).Centered(),
                    new Text(doc.Name),
                    new Markup(doc.PolishName, Style.Plain));
            }

            AnsiConsole.Render(table);
        }

        private static void HorizontalRule(string title)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.Render(new Rule($"[white bold]{title}[/]").RuleStyle("grey").LeftAligned());
            AnsiConsole.WriteLine();
        }
    }
}