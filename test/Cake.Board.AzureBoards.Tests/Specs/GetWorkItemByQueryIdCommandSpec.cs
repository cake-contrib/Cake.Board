// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.AzureBoards.Commands;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Specs
{
    public class GetWorkItemByQueryIdCommandSpec
    {
        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item query Id
WHEN he wants to fetch all work items by query in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task Scenario_SearchWorkItemByQueryId()
        {
            // Arrange
            string queryId = "someone-query";
            string project = "someone-project";
            string team = "someone-team";
            var fileContent = JObject.Parse(await File.ReadAllTextAsync($"{Environment.CurrentDirectory}/azureboards-wit_queries-response.json"));

            IEnumerable<WorkItem> workItems = fileContent["workItemRelations"].Values<JObject>().AsEnumerable()
                .Select(item => new WorkItem()
                {
                    Id = item["target"]["id"].Value<string>(),
                    Url = item["target"]["url"].Value<string>()
                }).ToList();

            var fakeResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(fileContent), Encoding.UTF8, "application/json")
            };
            var fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
            var fakeClient = new HttpClient(new FakeHttpMessageHandler(fakeResponse))
            {
                BaseAddress = new Uri("https://dev.azure.com/someone")
            };
            var board = new AzureBoards(fakeClient)
            {
                Project = project,
                Team = team
            };

            // Act
            IEnumerable<IWorkItem> wits = await board.GetWorkItemsByQueryIdAsync(queryId);

            // Assert
            IEnumerable<WorkItem> concreteWits = wits.Select(wit => Assert.IsType<WorkItem>(wit)).ToList();
            for (int i = 0; i < workItems.Count(); i++)
            {
                Assert.Equal(workItems.ElementAt(i).Id, concreteWits.ElementAt(i).Id);
                Assert.Equal(workItems.ElementAt(i).Url, concreteWits.ElementAt(i).Url);
            }
        }
    }
}
