using System;
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
            var serviceName = Environment.GetEnvironmentVariable("AzureSearchName") ?? "";
            var indexName = "analyzers-test";
            var apiKey = Environment.GetEnvironmentVariable("AzureSearchKey") ?? "";

            var serviceEndpoint = new Uri($"https://{serviceName}.search.windows.net/");
            var credential = new AzureKeyCredential(apiKey);
            var searchIndexClient = new SearchIndexClient(serviceEndpoint, credential);
            var searchClient = new SearchClient(serviceEndpoint, indexName, credential);

            if (AnsiConsole.Capabilities.SupportLinks)
                AnsiConsole.MarkupLine(
                    $"[link=https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/main/src/ASDemo]Demo for index {indexName}[/]!");

            HorizontalRule("Creating index");

            var index = new SearchIndex(indexName)
            {
                Fields =
                {
                    new SimpleField("uniqueId", SearchFieldDataType.String)
                        {IsKey = true, IsFilterable = true, IsSortable = true},
                    new SearchableField("name")
                    {
                        IsFilterable = true, IsSortable = true
                    },
                    new SearchableField("polishName") {IsFilterable = true, IsSortable = true},
                    new SearchableField("underscore") {IsFilterable = true, IsSortable = true},
                    new SimpleField("updated", SearchFieldDataType.DateTimeOffset)
                        {IsFilterable = true, IsSortable = true},
                    new SearchableField("keyPhrases", true) {IsFilterable = true}
                }
            };

            //add analyzer to the index
            index.Analyzers.Add(new LuceneStandardAnalyzer("luceneStandardAnalyzer"));

            AnsiConsole.WriteLine("Creating or updating index, if changed");
            
            //In order to add new analyzers, tokenizers, token filters, or character filters to an existing index, or modify its similarity settings,
            //set the 'allowIndexDowntime' query parameter to 'true' in the index update request
            await searchIndexClient.CreateOrUpdateIndexAsync(index, true);

            var addingData = Environment.GetEnvironmentVariable("AddingData");
            bool.TryParse(addingData, out var addData);

            if (addData)
            {
                HorizontalRule("Adding data to the index");
                AnsiConsole.WriteLine($"Adding data to the search index in {indexName}");
                await AddDataAsync(searchClient);
                AnsiConsole.WriteLine("Completed adding data");
            }

            OutputIndexLexicalAnaylsis(index);

            async Task<Response<SearchResults<SearchModel>>> QueryDataAsync(string query, SearchOptions options)
            {
                var data = await searchClient.SearchAsync<SearchModel>(query, options);
                return data;
            }

            HorizontalRule("Executing search");

            AnsiConsole.WriteLine("Query #1: Searching for styles value");

            var stylesText = "styles";

            AnalyzeText(searchIndexClient, indexName, stylesText);

            var response = await QueryDataAsync(stylesText, new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            });

            WriteDocuments(response);

            AnsiConsole.WriteLine("Query #2: Searching for text jquery");

            var jqueryText = "jquery";
            AnalyzeText(searchIndexClient, indexName, jqueryText);
            
            response = await QueryDataAsync(jqueryText, new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            });

            WriteDocuments(response);

            AnsiConsole.WriteLine("--> press any field to exit");
            System.Console.Read();
        }

        private static void OutputIndexLexicalAnaylsis(SearchIndex index)
        {
            HorizontalRule("Showing lexical analysis");

            AnsiConsole.WriteLine("Showing analyzers");

            foreach (var analyzer in index.Analyzers)
            {
                AnsiConsole.WriteLine($"[u]{analyzer.Name}[/]");
            }

            AnsiConsole.WriteLine("Show tokenizers for index");

            foreach (var lexicalTokenizer in index.Tokenizers)
            {
                AnsiConsole.WriteLine($"[u]{lexicalTokenizer.Name}[/]");
            }

            AnsiConsole.WriteLine("Show char filters");

            foreach (var charFilter in index.CharFilters)
            {
                AnsiConsole.WriteLine($"[u]{charFilter.Name}[/]");
            }
        }

        private static void AnalyzeText(SearchIndexClient client, string indexName, string stylesText)
        {
            AnsiConsole.WriteLine($"Analyzing query text {stylesText}");
            
            var analyzeText = client.AnalyzeText(indexName, new AnalyzeTextOptions(stylesText,
                LexicalTokenizerName.Standard));
            var table = new Table()
                .Border(TableBorder.Square)
                .BorderColor(Color.Red)
                .Title("Search results")
                .AddColumn(new TableColumn("[u]Token[/]").Centered())
                .AddColumn(new TableColumn("[u]Index (first character)[/]"))
                .AddColumn(new TableColumn("[u]Index (last character)[/]"))
                .AddColumn(new TableColumn("[u]Position[/]").Centered());

            foreach (var tokenInfo in analyzeText.Value)
            {
                table.AddRow(new Text(tokenInfo.Token).Centered(),
                    new Text(tokenInfo.StartOffset.ToString()),
                    new Text(tokenInfo.EndOffset.ToString()),
                    new Text(tokenInfo.Position.ToString()).Centered());
            }
        }

        private static async Task AddDataAsync(SearchClient searchClient)
        {
            var modelName1 = "external styles are defined withing the element, inside the section of an HTML page";
            string[] keyWords1 = {"CSS", "styles", "metadata", "element", "EF01"};
            var underscore1 = "bootstrap_filter_forms_EF05";
            var modelName2 = "To include an external JavaScript file, use the script tag with the attribute src";
            string[] keyWords2 = {"javascript", "EF02", "script", "tag", "src"};
            var underscore2 = "material_jquery_selector_EF06";
            var modelName3 = "Move to the home position";
            string[] keyWords3 = {"storage", "container", "EF03", "home"};
            var underscore3 = "location_custom_EF07";
            var modelName4 = "removal of stuck magnets restores machine cycle";
            string[] keyWords4 = {"magnets", "machine", "cycle", "EF04"};
            var underscore4 = "metal_tokens_filters_EF08";

            var batch = IndexDocumentsBatch.Create(
                IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName1,
                    KeyPhrases = keyWords1,
                    Underscore = underscore1,
                    Updated = new DateTime(2020, 10, 1, 7, 0, 0)
                })
                , IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName2,
                    KeyPhrases = keyWords2,
                    Underscore = underscore2,
                    Updated = new DateTime(2020, 9, 2, 8, 54, 0)
                }), IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName4,
                    KeyPhrases = keyWords4,
                    Underscore = underscore3,
                    Updated = new DateTime(2020, 8, 2, 12, 11, 0)
                }), IndexDocumentsAction.Upload(new SearchModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = modelName3,
                    KeyPhrases = keyWords3,
                    Underscore = underscore4,
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
                .AddColumn(new TableColumn("[u]Score[/]").Centered());

            foreach (var response in searchResults.GetResults())
            {
                var doc = response.Document;
                var score = response.Score;
                table.AddRow(new Text(doc.Id).Centered(),
                    new Text(doc.Name),
                    new Text(score.ToString()).Centered());
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