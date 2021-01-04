# Azure Cognitive Search - demo about using Azure Search to query keywords

Sample includes creating and filling in data / working with search [analyzers](https://docs.microsoft.com/en-us/azure/search/search-analyzers).

## Demo Walkthrough

In order for the demo to work, you will need an active subscription for cloud platform [Azure](https://azure.com). 

Demo requires [Azure Search](https://docs.microsoft.com/en-us/azure/search/search-what-is-azure-search) in order to test the functionality.

In order to test the functionality, you can use **free version** of Azure Search service.

## Demo run

In order to run the demo, you will need [.NET Core 3.1](https://dot.net) installed. You need to clone this repository, open terminal, navigate into [ASDemo.Console](https://github.com/bovrhovn/azure-search-lang-synonym-sample/tree/analyzers/src/ASDemo/ASDemo.Console) directory (locate *csproj* file). Execute dotnet run command to run the solution. 

Before you run the solution, you need to provide few settings:
- Azure Search Key - you get it from Azure Search Service blade **Keys**
- Azure Search Name - - you get it from Azure Search Service blade **Keys** 
- bool setting **AddData** to define, if data should be uploaded to index - **true** - inserts data - **false**, skips insertion

The best way to set the settings is to set the [environment variables](https://en.wikipedia.org/wiki/Environment_variable).  

![Settings](https://camo.githubusercontent.com/40f92e89700cc3ad393d69becd837b8f453ed02634217a759dbcbad8dd9fe26c/68747470733a2f2f637361636f726573657474696e67732e626c6f622e636f72652e77696e646f77732e6e65742f7075626c69632f7365617263682d6b6579776f72642d656e7669726f6e6d656e742d7661726961626c65732e706e67)

You should see the following result: 


## Credits

In this example I am using [Spectre.Console](https://github.com/spectresystems/spectre.console) project (as a Nuget package), which makes output of Terminal apps beatufil. 
