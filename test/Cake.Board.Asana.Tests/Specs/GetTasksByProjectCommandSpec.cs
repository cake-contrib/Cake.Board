// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cake.Board.Asana.Tests.Specs
{
    public class GetTasksByProjectCommandSpec
    {
        private readonly string _pat;
        private readonly string _project;
        private readonly JObject _fileContent;
        private List<Models.Task> _workItems;

        public GetTasksByProjectCommandSpec()
        {
            this._fileContent = JObject.Parse(File.ReadAllText($"{Environment.CurrentDirectory}/project_tasks-response.json"));
            this._workItems = this._fileContent["data"].Values<JObject>().AsEnumerable()
                .Select(item => new Models.Task()
                {
                    Id = item["id"].Value<long>().ToString()
                }).ToList();
            this._project = "someone-project";
            this._pat = "someone-pat";
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a project name
WHEN he wants to fetch all work items by query in Asana
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromBoardExtension_SearchTasksByProject()
        {
            // Arrange
            HttpResponseMessage fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(this._fileContent), Encoding.UTF8, "application/json")
            };
            FakeCakeContext fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            HttpClient fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri("https://app.asana.com/api/1.0")
            };
            Asana board = new Asana(fakeClient)
            {
                Project = this._project
            };

            // Act
            IEnumerable<IWorkItem> wits = await board.GetWorkItemsByProjectAsync();

            // Assert
            IEnumerable<Models.Task> concreteWits = wits.Select(wit => Assert.IsType<Models.Task>(wit)).ToList();
            for (int i = 0; i < this._workItems.Count(); i++)
            {
                Assert.Equal(this._workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
            }
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a project name
WHEN he wants to fetch all work items by query in Asana
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        [Obsolete]
        public async Task ScenarioFromCakeContextExtension_SearchTasksByProject()
        {
            // Arrange
            HttpResponseMessage fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(this._fileContent), Encoding.UTF8, "application/json")
            };
            FakeCakeContext fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            HttpClient fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri("https://app.asana.com/api/1.0")
            };
            Asana board = new Asana(fakeClient)
            {
                Project = this._project
            };

            // Act
            IEnumerable<IWorkItem> wits = await fakeCakeContext.GetTasksByProjectAsync(board, this._project);

            // Assert
            IEnumerable<Models.Task> concreteWits = wits.Select(wit => Assert.IsType<Models.Task>(wit)).ToList();
            for (int i = 0; i < this._workItems.Count(); i++)
            {
                Assert.Equal(this._workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
            }
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a project name
WHEN he wants to fetch all work items by query in Asana
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        [Obsolete]
        public async Task ScenarioFromCakeContextExtensionWithPatAndProject_SearchTasksByProject()
        {
            // Arrange
            HttpResponseMessage fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(this._fileContent), Encoding.UTF8, "application/json")
            };
            FakeCakeContext fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            HttpClient fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri("https://app.asana.com/api/1.0")
            };
            Asana board = new Asana(fakeClient)
            {
                Project = this._project
            };

            FieldInfo commandBehaviour = typeof(AsanaCommandAliases).GetRuntimeFields().Single(p => p.Name == "_getTasksByProjectBehaviourAsync");
            object originBehaviour = commandBehaviour.GetValue(typeof(AsanaCommandAliases));

            // Act
            commandBehaviour.SetValue(typeof(AsanaCommandAliases), (Func<IBoard, string, Task<IEnumerable<IWorkItem>>>)((azureBoard, id) => ((Asana)board).GetWorkItemsByProjectAsync(id)));
            IEnumerable<IWorkItem> wits = await fakeCakeContext.GetTasksByProjectAsync(this._pat, this._project);
            commandBehaviour.SetValue(typeof(AsanaCommandAliases), originBehaviour);

            // Assert
            IEnumerable<Models.Task> concreteWits = wits.Select(wit => Assert.IsType<Models.Task>(wit)).ToList();
            for (int i = 0; i < this._workItems.Count(); i++)
            {
                Assert.Equal(this._workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
            }
        }
    }
}
