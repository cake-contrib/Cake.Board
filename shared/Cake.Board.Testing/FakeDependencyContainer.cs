// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Cake.Board.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Board.Testing
{
    /// <summary>
    /// Fake implementation of <see cref="IDependencyContainer"/>.
    /// </summary>
    public class FakeDependencyContainer : IDependencyContainer
    {
        /// <inheritdoc/>
        public IServiceCollection Configure(IServiceCollection serviceDescriptors) => serviceDescriptors;
    }
}
