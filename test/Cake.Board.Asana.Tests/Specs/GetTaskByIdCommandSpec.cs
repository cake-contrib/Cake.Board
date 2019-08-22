// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
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
    public class GetTaskByIdCommandSpec
    {
        private readonly JObject _fileContent;
        private readonly string _pat;
        private readonly string _taskid;
        private readonly string _taskType;
        private readonly string _taskTitle;
        private readonly string _taskDescription;
        private readonly string _taskState;

        public GetTaskByIdCommandSpec()
        {
            this._fileContent = JObject.Parse(File.ReadAllText($"{Environment.CurrentDirectory}/task-response.json"));
            this._taskid = this._fileContent.Value<JObject>("data").Value<long>("id").ToString();
            this._taskType = this._fileContent.Value<JObject>("data").Value<string>("resource_type");
            this._taskTitle = this._fileContent.Value<JObject>("data").Value<string>("name");
            this._taskDescription = this._fileContent.Value<JObject>("data").Value<string>("notes");
            this._taskState = this._fileContent.Value<JObject>("data").Value<string>("assignee_status");
            this._pat = "someone-pat";
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a task Id
WHEN he wants to look for a task in Asana
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromBoardExtension_SearchTaskById()
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
            Asana board = new Asana(fakeClient);

            // Act
            IWorkItem wit = await board.GetWorkItemByIdAsync(this._taskid);

            // Assert
            Assert.IsType<Models.Task>(wit);

            Assert.Equal(this._taskid, wit.Id);
            Assert.Equal(this._taskState, ((Models.Task)wit).State);
            Assert.Equal(this._taskTitle, wit.Title);
            Assert.Equal(this._taskType, wit.Type);
            Assert.Equal(this._taskDescription, wit.Description);
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item Id
WHEN he wants to look for a work item in Asana
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromCakeContextExtensionWithBoard_SearchTaskById()
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
            Asana board = new Asana(fakeClient);

            // Act
            IWorkItem wit = await fakeCakeContext.GetWorkItemByIdAsync(board, this._taskid);

            // Assert
            Assert.IsType<Models.Task>(wit);

            Assert.Equal(this._taskid, wit.Id);
            Assert.Equal(this._taskState, ((Models.Task)wit).State);
            Assert.Equal(this._taskTitle, wit.Title);
            Assert.Equal(this._taskType, wit.Type);
            Assert.Equal(this._taskDescription, wit.Description);
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a work item Id
WHEN he wants to look for a work item in Asana
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromCakeContextExtensionWithPatAndOrganization_SearchTaskById()
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
            Asana board = new Asana(fakeClient);

            FieldInfo commandBehaviour = typeof(BoardCommandAliases).GetRuntimeFields().Single(p => p.Name == "_getWorkItemByIdBehaviourAsync");
            object originBehaviour = commandBehaviour.GetValue(typeof(BoardCommandAliases));

            // Act
            commandBehaviour.SetValue(typeof(BoardCommandAliases), (Func<IBoard, string, Task<IWorkItem>>)((azureBoard, id) => board.GetWorkItemByIdAsync(id)));
            IWorkItem wit = await fakeCakeContext.GetTaskByIdAsync(this._pat, this._taskid);
            commandBehaviour.SetValue(typeof(BoardCommandAliases), originBehaviour);

            // Assert
            Assert.IsType<Models.Task>(wit);

            Assert.Equal(this._taskid, wit.Id);
            Assert.Equal(this._taskState, ((Models.Task)wit).State);
            Assert.Equal(this._taskTitle, wit.Title);
            Assert.Equal(this._taskType, wit.Type);
            Assert.Equal(this._taskDescription, wit.Description);
        }
    }
}
