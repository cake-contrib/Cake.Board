# Cake.Board
Cake Addin for work with work items boards.

[![Gitter](https://badges.gitter.im/cake-board/community.svg)](https://gitter.im/cake-board/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![codecov](https://codecov.io/gh/nicolabiancolini/Cake.Board/branch/master/graph/badge.svg)](https://codecov.io/gh/nicolabiancolini/Cake.Board)

### Build status
| OS | Build & Test |
| :---: | :---: |
| __Windows x64__ | [![Build Status](https://dev.azure.com/nicolabiancolini/Cake.Board/_apis/build/status/nicolabiancolini.Cake.Board?branchName=master&jobName=Windows%20Agent)](https://dev.azure.com/nicolabiancolini/Cake.Board/_build/latest?definitionId=8&branchName=master) |
| __Linux x64__ | [![Build Status](https://dev.azure.com/nicolabiancolini/Cake.Board/_apis/build/status/nicolabiancolini.Cake.Board?branchName=master&jobName=Ubuntu%20Agent)](https://dev.azure.com/nicolabiancolini/Cake.Board/_build/latest?definitionId=8&branchName=master) |

### Publish packages status
| Package | NuGet | NuGet Pre-Release |
| :---: | :---: | :---: |
| __Cake.Board.Abstractions__ | [![Nuget (with prereleases)](https://img.shields.io/nuget/v/Cake.Board.Abstractions.svg)](https://www.nuget.org/packages/Cake.Board.Abstractions) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Cake.Board.Abstractions.svg)](https://www.nuget.org/packages/Cake.Board.Abstractions) |
| __Cake.Board__ | [![Nuget (with prereleases)](https://img.shields.io/nuget/v/Cake.Board.svg)](https://www.nuget.org/packages/Cake.Board) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Cake.Board.svg)](https://www.nuget.org/packages/Cake.Board)
| __Cake.Board.AzureBoards__ | [![Nuget (with prereleases)](https://img.shields.io/nuget/v/Cake.Board.AzureBoards.svg)](https://www.nuget.org/packages/Cake.Board.AzureBoards) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Cake.Board.AzureBoards.svg)](https://www.nuget.org/packages/Cake.Board.AzureBoards)

## Usage
### Azure Boards
In order to usage this _addin_, add to your Cake script.
``` csharp
#addin "nuget:?package=Cake.Board.AzureBoards&loaddependencies=true"
#addin "nuget:?package=Polly"
```
``` csharp
Task("GetWorkItemById")
  .Does(async () => 
  {
    IWorkItem workItem = await GetWorkItemByIdAsync(
      new AzureBoards(
        EnvironmentVariable("PERSONAL_ACCESS_TOKEN"),
        EnvironmentVariable("AZURE_DEVOPS_ORGANIZATION")),
      id);
  });
```
or 
``` csharp
Task("GetWorkItemsByQueryIdAsync")
  .Does(async () => 
  {
    var board = new AzureBoards(
      EnvironmentVariable("PERSONAL_ACCESS_TOKEN"),
      EnvironmentVariable("AZURE_DEVOPS_ORGANIZATION"))
      {
        Project = EnvironmentVariable("AZURE_DEVOPS_PROJECT"),
        Team = EnvironmentVariable("AZURE_DEVOPS_TEAM")
      };

    IEnumerable<IWorkItem> workItems = await GetWorkItemsByQueryIdAsync(
      board,
      queryId);
  });
```

## Contributing
So you’re thinking about contributing to Cake? Great! It’s really appreciated.


Fork the repository.
Create a branch to work in.
Make your feature addition or bug fix.
Don't forget the unit tests.
Send a pull request.

## Contributing
You’re thinking about contributing to Cake.Board? Great! It’s really appreciated.
Please make small changes focused on the purpose of the branch in order to make the changes easily integrable.
All you have to do to get started is this!
``` bash
git clone https://github.com/nicolabiancolini/Cake.Board.git
git checkout -b <YOUR_BRANCH_NAME>
```
For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[![MIT license](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/nicolabiancolini/Cake.Board/blob/master/LICENSE)
