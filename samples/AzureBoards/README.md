# Azure Boards sample

## How to use
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

### Run sample
For use this sample you need to create file `env.json` and place it on this folder level.

The file should be like this:
``` json
{
  "pat": "<YOUR_PERSONAL_ACCESS_TOKEN>",
  "organization": "<YOUR_ORGANIZATION>",
  "project": "<YOUT_PROJECT>",
  "team": "<YOUR_TEAM>",
  "query_id": "<YOUR_WORK_ITEM_QUERY_ID>",
  "wit_id": "<YOUR_WORK_ITEM_ID>"
}
```

Now you can run sample with `build.cmd`.
