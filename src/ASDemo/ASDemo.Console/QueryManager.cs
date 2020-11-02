using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;

namespace ASDemo.Console
{
    /// <summary>
    /// QueryManager class for different search options
    /// </summary>
    /// <remarks>
    ///    easily testable
    /// </remarks>
    public class QueryManager
    {
        public async Task<Response<SearchResults<SearchModel>>> QueryDataAsync(SearchClient searchClient, 
            string query, SearchOptions options)
        {
            var response = await searchClient.SearchAsync<SearchModel>(query, options);
            return response;
        }
    }
}