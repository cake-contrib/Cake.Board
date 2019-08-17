// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.AzureBoards.Commands;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Testing;
using Cake.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pose;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Specs
{
    public class GetWorkItemByIdCommandSpec
    {
        private readonly JObject _fileContent;
        private readonly string _organization;
        private readonly string _pat;
        private readonly string _witId;
        private readonly string _witType;
        private readonly string _witTitle;
        private readonly string _witDescription;
        private readonly string _witState;
        private readonly string _witUrl;

        public GetWorkItemByIdCommandSpec()
        {
            this._fileContent = JObject.Parse(File.ReadAllText($"{Environment.CurrentDirectory}/wit-response.json"));
            this._witId = this._fileContent.Value<string>("id");
            this._witType = this._fileContent.Value<JObject>("fields").Value<string>("System.WorkItemType");
            this._witTitle = this._fileContent.Value<JObject>("fields").Value<string>("System.Title");
            this._witDescription = this._fileContent.Value<JObject>("fields").Value<string>("System.Description");
            this._witState = this._fileContent.Value<JObject>("fields").Value<string>("System.State");
            this._witUrl = this._fileContent.Value<string>("url");
            this._organization = "someone-organization";
            this._pat = "someone-pat";
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item Id
WHEN he wants to look for a work item in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromBoardExtension_SearchWorkItemById()
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
            var board = new AzureBoards(fakeClient);

            // Act
            IWorkItem wit = await board.GetWorkItemByIdAsync(this._witId);

            // Assert
            Assert.IsType<WorkItem>(wit);

            Assert.Equal(this._witId, wit.Id);
            Assert.Equal(this._witState, wit.State);
            Assert.Equal(this._witTitle, wit.Title);
            Assert.Equal(this._witType, wit.Type);
            Assert.Equal(this._witDescription, wit.Description);
            Assert.Equal(this._witUrl, ((WorkItem)wit).Url);
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item Id
WHEN he wants to look for a work item in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromCakeContextExtensionWithBoard_SearchWorkItemById()
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
            var board = new AzureBoards(fakeClient);

            // Act
            IWorkItem wit = await fakeCakeContext.GetWorkItemByIdAsync(board, this._witId);

            // Assert
            Assert.IsType<WorkItem>(wit);

            Assert.Equal(this._witId, wit.Id);
            Assert.Equal(this._witState, wit.State);
            Assert.Equal(this._witTitle, wit.Title);
            Assert.Equal(this._witType, wit.Type);
            Assert.Equal(this._witDescription, wit.Description);
            Assert.Equal(this._witUrl, ((WorkItem)wit).Url);
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item Id
WHEN he wants to look for a work item in Azure Boards
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromCakeContextExtensionWithPatAndOrganization_SearchWorkItemById()
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
            var board = new AzureBoards(fakeClient);

            FieldInfo commandBehaviour = typeof(WorkItemCommand).GetRuntimeFields().Single(p => p.Name == "_getWorkItemByIdBehaviourAsync");
            object originBehaviour = commandBehaviour.GetValue(typeof(WorkItemCommand));

            // Act
            commandBehaviour.SetValue(typeof(WorkItemCommand), (Func<IBoard, string, Task<IWorkItem>>)((azureBoard, id) => board.GetWorkItemByIdAsync(id)));
            IWorkItem wit = await fakeCakeContext.GetWorkItemByIdAsync(this._pat, this._organization, this._witId);
            commandBehaviour.SetValue(typeof(WorkItemCommand), originBehaviour);

            // Assert
            Assert.IsType<WorkItem>(wit);

            Assert.Equal(this._witId, wit.Id);
            Assert.Equal(this._witState, wit.State);
            Assert.Equal(this._witTitle, wit.Title);
            Assert.Equal(this._witType, wit.Type);
            Assert.Equal(this._witDescription, wit.Description);
            Assert.Equal(this._witUrl, ((WorkItem)wit).Url);
        }
    }
}
