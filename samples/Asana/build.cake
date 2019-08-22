/*
 * Install addins.
 */
#addin "nuget:?package=Cake.Json&version=3.0.1"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.1"
#addin "nuget:?package=Polly&version=7.1.0"

/*
 * Load references.
 */
#reference ../../src/Cake.Board.Asana/bin/Release/netstandard2.0/Cake.Board.Asana.dll
#reference ../../src/Cake.Board/bin/Release/netstandard2.0/Cake.Board.dll
#reference ../../src/Cake.Board.Abstractions/bin/Release/netstandard2.0/Cake.Board.Abstractions.dll

using Cake.Board.Asana;
using Cake.Board.Abstractions;

const string ENV_FILE_PATH = "./env.json";

var pat = string.Empty;
var projectId = string.Empty;
var taskId = string.Empty;
Asana board = null;

Setup(context => 
{
    if(!FileExists(ENV_FILE_PATH)) 
        throw new FileNotFoundException(ENV_FILE_PATH);
    
    var envs = DeserializeJsonFromFile<IDictionary<string,string>>(ENV_FILE_PATH);
    pat = envs[nameof(pat)];
    projectId = envs["project_id"];
    taskId = envs["task_id"];

    board = new Asana(pat) {
        ProjectId = projectId
    };
});

Task("GetTaskById")
   .Does(async () => {
      var wit = await GetWorkItemByIdAsync(board, taskId);
      Information(SerializeJsonPretty(wit));
   });
   
Task("GetTasksByProjectId")
   .Does(async () => {
      var wits = new List<IWorkItem>();
      
      foreach (var item in await GetTasksByProjectIdAsync(board, projectId)) {
         wits.Add(await GetWorkItemByIdAsync(board, item.Id));
      };
   
      Information(SerializeJsonPretty(wits));
   });

var target = Argument("target", "Default");

Task("Default")
   .IsDependentOn("GetTaskById")
   .IsDependentOn("GetTasksByProjectId")
   .Does(() => {
      Information("Hello Cake.Board.Asana!");
   });

RunTarget(target);
