using System;
using System.Threading.Tasks;
using ASDemo.Console;
using Azure;
using Azure.Search.Documents;
using NUnit.Framework;

namespace ASDemo.Tests
{
    public class Tests
    {
        private SearchClient searchClient;

        [SetUp]
        public void Setup()
        {
            var apiKey = Environment.GetEnvironmentVariable("AzureSearchKey");

            var serviceEndpoint = new Uri($"https://.search.windows.net/");
            var credential = new AzureKeyCredential(apiKey);
            searchClient = new SearchClient(serviceEndpoint, indexName, credential);
        }

        [Test(Description = "Query keywords to get back the results")]
        public async Task QueryKeywords()
        {
            var queryManager =  new QueryManager();
            var response = await queryManager.QueryDataAsync(searchClient,"css", new SearchOptions
            {
                Filter = "",
                IncludeTotalCount = true
            });
        } 
        
        [Test(Description = "Query polish language and getting back the results")]
        public async Task QueryPolishLanguage()
        {
            var queryManager =  new QueryManager();
            var response = await queryManager.QueryDataAsync(searchClient,"plik",new SearchOptions
            {
                Filter = "",
                OrderBy = {"updated desc"}
            });
            
        }
    }
}