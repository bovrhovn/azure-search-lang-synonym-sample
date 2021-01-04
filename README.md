# Azure Cognitive Search - demo about using Azure Search to query keywords

Sample includes creating index, filling that with random data, leveraging basic search functionality. including fuzzy search,field limited text, doing language translation with [Azure Translation service](https://azure.microsoft.com/en-us/services/cognitive-services/translator/) to implement the ability to search in multiple languages as well, doing [synonym maps](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/synonyms) for multiple related terms search and implementing [custom analyzers](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/analyzers) to transform tokenization of special text.

## Demo Walkthrough

In order for the demo to work, you will need an active subscription for [Azure](https://azure.com) cloud platform. Trial or [Azure Pass](https://www.microsoftazurepass.com/) or [MSDN](https://visualstudio.microsoft.com/subscriptions/) are valid as well. 

Demo requires 2 services in order to test the functionality:
- [Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) 
- Azure Cognitive Services, specifically [Translation Service](https://docs.microsoft.com/en-us/azure/cognitive-services/Translator/translator-info-overview)

Translator is used for translating text from english to polish language (when data is pushed to newly created index). [Method](https://github.com/bovrhovn/azure-search-lang-synonym-sample/blob/main/src/ASDemo/ASDemo.Console/Translator.cs) is written in a way, that accepts language from and language to and can be easily changed. 

## Demo run

In order to run the demo, you will need [.NET Core 3.1](https://dot.net) installed. You need to clone this repository, open terminal, navigate into [ASDemo.Console](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/main/src/ASDemo/ASDemo.Console) directory (locate *csproj* file). Execute dotnet run command to run the solution. 

Before you run the solution, you need to provide settings:
- Azure Search Key - you get it from Azure Search Service blade **Keys**
- Azure Search Name - you get it from Azure Search Service blade **Keys**
- Cognitive Services Key - you get it from Azure Cognitive Services blade **Keys and Endpoint**
- bool settings **AddData** - set **true**, if data should be uploaded to index (doesn't have check, if data exists, will add data) or if set **false**, skips insertion

![environment variables](https://csacoresettings.blob.core.windows.net/public/search-keyword-environment-variables.png)

The easiest way to define app settings is to leverage [environment variables](https://en.wikipedia.org/wiki/Environment_variable).

## Other EXAMPLES

For **synonym maps example** check [synonym branch](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/synonyms).

For **custom analyzers** check [analyzer branch](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/analyzers).

## Credits

In this example I am using [Spectre.Console](https://github.com/spectresystems/spectre.console) project (as a Nuget package), which makes output of Terminal apps beatufil. 
