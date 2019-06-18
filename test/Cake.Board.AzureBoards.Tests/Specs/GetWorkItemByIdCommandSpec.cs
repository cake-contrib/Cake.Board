// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;
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
    public class GetWorkItemByIdCommandSpec
    {
        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item Id
WHEN he wants to look for a work item in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task Scenario_SearchWorkItemById()
        {
            // Arrange
            var fileContent = JObject.Parse(await File.ReadAllTextAsync($"{Environment.CurrentDirectory}/azureboards-wit-response.json"));
            string witId = fileContent.Value<string>("id");
            string witType = fileContent.Value<JObject>("fields").Value<string>("System.WorkItemType");
            string witTitle = fileContent.Value<JObject>("fields").Value<string>("System.Title");
            string witDescription = fileContent.Value<JObject>("fields").Value<string>("System.Description");
            string witState = fileContent.Value<JObject>("fields").Value<string>("System.State");
            string witUrl = fileContent.Value<string>("url");

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
            var board = new AzureBoards(fakeClient);

            // Act
            IWorkItem wit = await fakeCakeContext.GetWorkItemByIdAsync(board, witId);

            // Assert
            Assert.Equal(witId, wit.Id);
            Assert.Equal(witState, wit.State);
            Assert.Equal(witTitle, wit.Title);
            Assert.Equal(witType, wit.Type);
            Assert.Equal(witDescription, wit.Description);
        }
    }
}
