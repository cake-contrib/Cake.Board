# Cake.Board
Cake Addin for work with work items boards.

[![Gitter](https://badges.gitter.im/cake-board/community.svg)](https://gitter.im/cake-board/community?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![codecov](https://codecov.io/gh/nicolabiancolini/Cake.Board/branch/master/graph/badge.svg)](https://codecov.io/gh/nicolabiancolini/Cake.Board)

### Build status
| OS | Build & Test |
| :---: | :---: |
| __Windows x64__ | [![Build Status](https://dev.azure.com/nicolabiancolini/Cake.Board/_apis/build/status/nicolabiancolini.Cake.Board?branchName=master&jobName=Windows%20Agent)](https://dev.azure.com/nicolabiancolini/Cake.Board/_build/latest?definitionId=8&branchName=master) |
| __Linux x64__ | [![Build Status](https://dev.azure.com/nicolabiancolini/Cake.Board/_apis/build/status/nicolabiancolini.Cake.Board?branchName=master&jobName=Ubuntu%20Agent)](https://dev.azure.com/nicolabiancolini/Cake.Board/_build/latest?definitionId=8&branchName=master) |

## Usage
### Azure Boards
In order to usage this _addin_, add to your Cake script.
``` csharp
#addin "nuget:?package=Cake.Board"
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
## License
[![MIT license](https://img.shields.io/badge/license-MIT-brightgreen.svg)](https://github.com/nicolabiancolini/Cake.Board/blob/master/LICENSE)
