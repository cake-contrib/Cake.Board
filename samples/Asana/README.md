# Asana sample

## How to use
In order to usage this _addin_, add to your Cake script.
``` csharp
#addin "nuget:?package=Cake.Board.Asana&loaddependencies=true"
#addin "nuget:?package=Polly"
```
``` csharp
Task("GetTaskById")
   .Does(async () => {
      IWorkItem workItem = await GetWorkItemByIdAsync(
        new Asana(EnvironmentVariable("PERSONAL_ACCESS_TOKEN")) {
          ProjectId = EnvironmentVariable("ASANA_PROJECT_ID")
        },
        taskId);
   });
```
or 
``` csharp
Task("GetTasksByProjectId")
   .Does(async () => {
      var wits = new List<IWorkItem>();
      
      foreach (var item in await GetTasksByProjectIdAsync(new Asana(EnvironmentVariable("PERSONAL_ACCESS_TOKEN")), projectId)) {
         wits.Add(await GetWorkItemByIdAsync(board, item.Id));
      };
   });
```

### Run sample
For use this sample you need to create file `env.json` and place it on this folder level.

The file should be like this:
``` json
{
  "pat": "<YOUR_PERSONAL_ACCESS_TOKEN>",
  "project_id": "<YOUT_PROJECT_ID>",
  "task_id": "<YOUR_TASK_ID>"
}
```

Now you can run sample with `build.cmd`.
