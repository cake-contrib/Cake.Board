// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Cake.Board.AzureBoards.Commands;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Testing;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Specs
{
    public class GenerateReleaseNotesSpec
    {
        private readonly IEnumerable<WorkItem> _wits;
        private readonly string _releaseNotes;

        public GenerateReleaseNotesSpec()
        {
            this._wits = new List<WorkItem>
            {
                new WorkItem
                {
                    Id = "120",
                    Type = "Bug",
                    Url = "https://lorem.ipsum/120",
                    Title = "URL parts incorrectly identifies the user and repository parts.",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "119",
                    Type = "Issue",
                    Url = "https://lorem.ipsum/119",
                    Title = "Correct issue with casing of label name.",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "124",
                    Type = "Task",
                    Url = "https://lorem.ipsum/124",
                    Title = "Add support for adding default labels to issues on a repository.",
                    Description = "Create a class to insert labels on issue from configuration file.",
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "117",
                    Type = "Feature",
                    Url = "https://lorem.ipsum/117",
                    Title = "Add Token option for CLI as alternative to user/password.",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "117",
                    Type = "Feature",
                    Url = "https://lorem.ipsum/117",
                    Title = "Add Token option for CLI as alternative to user/password.",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "116",
                    Type = "Epic",
                    Url = "https://lorem.ipsum/116",
                    Title = "Add GitReleaseManager as Global Tool",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "121",
                    Type = "Epic",
                    Url = "https://lorem.ipsum/121",
                    Title = "Include link to closed issues in milestone link.",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "115",
                    Type = "Feature",
                    Url = "https://lorem.ipsum/115",
                    Title = "Switch to new csproj format.",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "110",
                    Type = "Task",
                    Url = "https://lorem.ipsum/110",
                    Title = "Update project NuGet packages",
                    Description = string.Empty,
                    State = "Closed"
                }
            };
            this._releaseNotes = File.ReadAllText($"{Environment.CurrentDirectory}/release-notes.md");
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a list of work items
WHEN he wants to create a release notes
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public void ScenarioFromBoardExtension_SearchWorkItemByQueryId()
        {
            // Arrange
            var fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());

            // Act
            string releaseNotes = fakeCakeContext.GenerateReleaseNotes(this._wits);

            // Assert
            Assert.Equal(this._releaseNotes, releaseNotes);
        }
    }
}
