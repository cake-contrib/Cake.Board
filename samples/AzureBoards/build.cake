/*
 * Install addins.
 */
#addin "nuget:?package=Cake.Json&version=3.0.1"
#addin "nuget:?package=Newtonsoft.Json&version=11.0.1"
#addin "nuget:?package=Polly&version=7.1.0"

/*
 * Load references.
 */
#reference ../../src/Cake.Board.AzureBoards/bin/Release/netstandard2.0/Cake.Board.AzureBoards.dll
#reference ../../src/Cake.Board/bin/Release/netstandard2.0/Cake.Board.dll
#reference ../../src/Cake.Board.Abstractions/bin/Release/netstandard2.0/Cake.Board.Abstractions.dll

using Cake.Board.AzureBoards;
using Cake.Board.Abstractions;

const string ENV_FILE_PATH = "./env.json";

var pat = string.Empty;
var organization = string.Empty;
var project = string.Empty; 
var team = string.Empty;
var queryId = string.Empty;
var witId = string.Empty;
AzureBoards board = null;

Setup(context => 
{
    if(!FileExists(ENV_FILE_PATH)) 
        throw new FileNotFoundException(ENV_FILE_PATH);
    
    var envs = DeserializeJsonFromFile<IDictionary<string,string>>(ENV_FILE_PATH);
    pat = envs[nameof(pat)];
    organization = envs[nameof(organization)];
    project = envs[nameof(project)];
    team = envs[nameof(team)];
    queryId = envs["query_id"];
    witId = envs["wit_id"];

    board = new AzureBoards(pat, organization) {
        Project = project,
        Team = team
    };
});

Task("GetWorkItemById")
   .Does(async () => {
      var wit = await GetWorkItemByIdAsync(board, witId);
      Information(SerializeJsonPretty(wit));
   });
   
Task("GetWorkItemByQueryId")
   .Does(async () => {
      var wits = new List<IWorkItem>();
      
      foreach (var item in await GetWorkItemsByQueryIdAsync(board, queryId)) {
         wits.Add(await GetWorkItemByIdAsync(board, item.Id));
      };
   
      Information(SerializeJsonPretty(wits));
   });

var target = Argument("target", "Default");

Task("Default")
   .IsDependentOn("GetWorkItemById")
   .IsDependentOn("GetWorkItemByQueryId")
   .Does(() => {
      Information("Hello Cake.Board.AzureBoards!");
   });

RunTarget(target);
