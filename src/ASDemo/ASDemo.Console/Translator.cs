using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ASDemo.Console
{
    public class Translator
    {
        public async Task<string> GetTranslatedTextAsync(string textToTranslate, string from = "en", string to = "pl")
        {
            var subscriptionKey = Environment.GetEnvironmentVariable("CognitiveServicesSubscriptionKey");
            string route = $"translate?api-version=3.0&from={from}&to={to}";
            var body = new object[] {new {Text = textToTranslate}};
            var requestBody = JsonConvert.SerializeObject(body);

            using var client = new HttpClient();
            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(Constants.TranslatorEndpoint + route),
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            request.Headers.Add("Ocp-Apim-Subscription-Region", "westeurope");
            var response = await client.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                System.Console.WriteLine("There was an error calling translation service. Check details...");
                System.Console.WriteLine(response.ReasonPhrase);
            }

            var translatedText = await response.Content.ReadAsStringAsync();
            var translatorModel = JsonConvert.DeserializeObject<TranslationModel[]>(translatedText);
            if (translatorModel.Length > 0)
                return translatorModel[0].Translations[0].Text;
            
            System.Console.WriteLine("No translation returned. Check data and service");
            return string.Empty;
        }
    }

    public class TranslationModel
    {
        [JsonProperty("translations")]
        public Translation[] Translations { get; set; }
    }

    public class Translation
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }
}