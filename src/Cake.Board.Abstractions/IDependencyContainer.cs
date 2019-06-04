// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Cake.Board.Abstractions
{
    /// <summary>
    /// Todo.
    /// </summary>
    public interface IDependencyContainer
    {
        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="serviceDescriptors">Todo1.</param>
        /// <returns>Todo2.</returns>
        IServiceCollection Configure(IServiceCollection serviceDescriptors);
    }
}
