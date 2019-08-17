// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

using Cake.Board.AzureBoards.Commands;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Testing;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Specs
{
    public class GenerateReleaseNotesSpec
    {
        private IEnumerable<WorkItem> _wits;

        public GenerateReleaseNotesSpec()
        {
            this._wits = new List<WorkItem>
            {
                new WorkItem
                {
                    Id = "10",
                    Type = "Bug",
                    Url = "https://lorem.ipsum/10",
                    Title = "in tellus integer feugiat scelerisque varius morbi enim nunc faucibus",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "8",
                    Type = "Issue",
                    Url = "https://lorem.ipsum/8",
                    Title = "risus feugiat in ante metus dictum at tempor commodo ullamcorper",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "100",
                    Type = "Task",
                    Url = "https://lorem.ipsum/100",
                    Title = "nam at lectus urna duis convallis convallis tellus id interdum",
                    Description = string.Empty,
                    State = "Closed"
                },
                new WorkItem
                {
                    Id = "1",
                    Type = "Feature",
                    Url = "https://lorem.ipsum/1",
                    Title = "eu nisl nunc mi ipsum faucibus vitae aliquet nec ullamcorper",
                    Description = string.Empty,
                    State = "Closed"
                }
            };
        }

        [Fact(DisplayName = @"GIVEN a DevOps engineer with a list of work items
WHEN he wants to create a release notes
THEN it must be able to obtain the content sought")]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public async Task ScenarioFromBoardExtension_SearchWorkItemByQueryId()
        {
            // Arrange
            var fakeCakeContext = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());

            // Act
            await fakeCakeContext.GenerateReleaseNotesAsync(default, this._wits);

            // Assert
            Assert.True(false);
        }
    }
}
