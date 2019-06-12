// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Cake.Board.AzureBoards.Tests.Fixtures;
using Cake.Board.Testing;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Specs
{
    [Collection(CollectionNames.WORK_ITEM_COMMANDS)]
    public class GetWorkItemByIdCommandSpec
    {
        private readonly WorkItemCommandFixture _fixture;

        public GetWorkItemByIdCommandSpec(WorkItemCommandFixture fixture) => this._fixture = fixture;

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.ACCEPTANCE_TEST)]
        public void ScenarioWithCorrectValue()
        {
        }
    }
}
