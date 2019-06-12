// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Testing;

namespace Cake.Board.AzureBoards.Tests.Fixtures
{
    public class WorkItemCommandFixture : IDisposable
    {
        private FakeDependencyContainer _container;
        private FakeCakeContext _context;

        public WorkItemCommandFixture()
        {
            this._container = new FakeDependencyContainer();
            this._context = new FakeCakeContext(logBehaviour: () => new FakeCakeLog());
        }

        public void Dispose()
        {
            this._container = null;
            this._context = null;
        }
    }
}
