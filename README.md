# Azure Cognitive Search - demo about using Azure Search to query keywords

Sample includes creating and filling in data, including fuzzy search,field limited text and language translation service to have the ability to search in multiple languages as well.

## Demo Walkthrough

In order for the demo to work, you will need an active subscription for cloud platform [Azure](https://azure.com). 

Demo requires 2 services in order to test the functionality:
- [Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) 
- Azure Cognitive Services, specifically [Translation Service](https://docs.microsoft.com/en-us/azure/cognitive-services/Translator/translator-info-overview)

Translator is used for translating text from english to polish language (when data is pushed to newly created index). [Method](https://github.com/bovrhovn/azure-search-lang-synonym-sample/blob/main/src/ASDemo/ASDemo.Console/Translator.cs) is written in a way, that accepts language from and language to and can be easily changed. 

## Demo run

In order to run the demo, you will need [.NET Core 3.1](https://dot.net) installed. You need to clone this repository, open terminal, navigate into [ASDemo.Console](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/main/src/ASDemo/ASDemo.Console) directory (locate *csproj* file). Execute dotnet run command to run the solution. 

Before you run the solution, you need to provide settings:
- Azure search Key - you get it from Azure Search Service blade **Keys**
- Azure Search Name - - you get it from Azure Search Service blade **Keys**
- Cognitive Services Key - - you get it from Azure Cognitive Services blade **Keys and Endpoint**
- bool settings, where you define, if data should be uploaded to index - **true** - inserts data - **false**, skips insertion

![environment variables](https://webeudatastorage.blob.core.windows.net/web/search-keyword-environment-variables.png)

Depends on which editor you use or operating system you are testing this, environment variables are set.

You should see the following result: 
![environment variables](https://webeudatastorage.blob.core.windows.net/web/azure-search-synonym-map-output.png)

## Credits

In this example I am using [Spectre.Console](https://github.com/spectresystems/spectre.console) project (as a Nuget package), which makes output of Terminal apps beatufil. 
