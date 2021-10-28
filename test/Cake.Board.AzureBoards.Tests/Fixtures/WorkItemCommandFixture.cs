// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Testing;
using Cake.Testing;

namespace Cake.Board.AzureBoards.Tests.Fixtures
{
    public class WorkItemCommandFixture : IDisposable
    {
        private FakeCakeContext _context;

        public WorkItemCommandFixture() => this._context = new FakeCakeContext(logBehaviour: () => new FakeLog());

        public void Dispose() => this._context = null;
    }
}
