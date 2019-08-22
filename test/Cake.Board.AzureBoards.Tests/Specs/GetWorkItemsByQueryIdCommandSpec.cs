// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Specs
{
    public class GetWorkItemsByQueryIdCommandSpec
    {
        private readonly string _queryId;
        private readonly string _organization;
        private readonly string _pat;
        private readonly string _project;
        private readonly string _team;
        private readonly JObject _fileContent;
        private List<WorkItem> _workItems;

        public GetWorkItemsByQueryIdCommandSpec()
        {
            this._fileContent = JObject.Parse(File.ReadAllText($"{Environment.CurrentDirectory}/wit_queries-response.json"));
            this._workItems = this._fileContent["workItemRelations"].Values<JObject>().AsEnumerable()
                .Select(item => new WorkItem()
                {
                    Id = item["target"]["id"].Value<string>(),
                    Url = item["target"]["url"].Value<string>()
                }).ToList();
            this._queryId = "someone-query";
            this._organization = "someoune-organization";
            this._project = "someone-project";
            this._team = "someone-team";
            this._pat = "someone-pat";
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item query Id
WHEN he wants to fetch all work items by query in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromBoardExtension_SearchWorkItemByQueryId()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(this._fileContent), Encoding.UTF8, "application/json")
            };
            var fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            var fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri($"https://dev.azure.com/{this._organization}")
            };
            var board = new AzureBoards(fakeClient)
            {
                Project = this._project,
                Team = this._team
            };

            // Act
            IEnumerable<IWorkItem> wits = await board.GetWorkItemsByQueryIdAsync(this._queryId);

            // Assert
            IEnumerable<WorkItem> concreteWits = wits.Select(wit => Assert.IsType<WorkItem>(wit)).ToList();
            for (int i = 0; i < this._workItems.Count(); i++)
            {
                Assert.Equal(this._workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
                Assert.Equal(this._workItems.ElementAt(i).Url, concreteWits.ElementAt(i).Url);
            }
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item query Id
WHEN he wants to fetch all work items by query in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        [Obsolete]
        public async Task ScenarioFromCakeContextExtension_SearchWorkItemByQueryId()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(this._fileContent), Encoding.UTF8, "application/json")
            };
            var fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            var fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri($"https://dev.azure.com/{this._organization}")
            };
            var board = new AzureBoards(fakeClient)
            {
                Project = this._project,
                Team = this._team
            };

            // Act
            IEnumerable<IWorkItem> wits = await fakeCakeContext.GetWorkItemsByQueryIdAsync(board, this._queryId);

            // Assert
            IEnumerable<WorkItem> concreteWits = wits.Select(wit => Assert.IsType<WorkItem>(wit)).ToList();
            for (int i = 0; i < this._workItems.Count(); i++)
            {
                Assert.Equal(this._workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
                Assert.Equal(this._workItems.ElementAt(i).Url, concreteWits.ElementAt(i).Url);
            }
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item query Id
WHEN he wants to fetch all work items by query in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        [Obsolete]
        public async Task ScenarioFromCakeContextExtensionWithPatAndOrganization_SearchWorkItemByQueryId()
        {
            // Arrange
            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(this._fileContent), Encoding.UTF8, "application/json")
            };
            var fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            var fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri($"https://dev.azure.com/{this._organization}")
            };
            var board = new AzureBoards(fakeClient)
            {
                Project = this._project,
                Team = this._team
            };

            FieldInfo commandBehaviour = typeof(AzureBoardsCommandAliases).GetRuntimeFields().Single(p => p.Name == "_getWorkItemsByQueryIdBehaviourAsync");
            object originBehaviour = commandBehaviour.GetValue(typeof(AzureBoardsCommandAliases));

            // Act
            commandBehaviour.SetValue(typeof(AzureBoardsCommandAliases), (Func<IBoard, string, Task<IEnumerable<IWorkItem>>>)((azureBoard, id) => board.GetWorkItemsByQueryIdAsync(id)));
            IEnumerable<IWorkItem> wits = await fakeCakeContext.GetWorkItemsByQueryIdAsync(this._pat, this._organization, this._queryId, this._project, this._team);
            commandBehaviour.SetValue(typeof(AzureBoardsCommandAliases), originBehaviour);

            // Assert
            IEnumerable<WorkItem> concreteWits = wits.Select(wit => Assert.IsType<WorkItem>(wit)).ToList();
            for (int i = 0; i < this._workItems.Count(); i++)
            {
                Assert.Equal(this._workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
                Assert.Equal(this._workItems.ElementAt(i).Url, concreteWits.ElementAt(i).Url);
            }
        }
    }
}
