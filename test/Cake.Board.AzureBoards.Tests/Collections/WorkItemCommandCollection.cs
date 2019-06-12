// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Cake.Board.AzureBoards.Tests.Fixtures;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Collections
{
    [CollectionDefinition(CollectionNames.WORK_ITEM_COMMANDS)]
    public class WorkItemCommandCollection : ICollectionFixture<WorkItemCommandFixture>
    {
    }
}
