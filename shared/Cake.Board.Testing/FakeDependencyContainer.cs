// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Board.Testing
{
    /// <summary>
    /// Fake implementation of <see cref="IDependencyContainer"/>.
    /// </summary>
    public class FakeDependencyContainer : IDependencyContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDependencyContainer"/> class.
        /// </summary>
        /// <param name="behaviour">Depencency container behaviour.</param>
        public FakeDependencyContainer(Func<IServiceCollection, IServiceCollection> behaviour = null) => this.Behaviour = behaviour;

        /// <summary>
        /// Gets or sets depencency container behavior.
        /// </summary>
        public Func<IServiceCollection, IServiceCollection> Behaviour { get; set; }

        /// <inheritdoc/>
        public IServiceCollection Configure(IServiceCollection serviceDescriptors) => this.Behaviour != null ? this.Behaviour.Invoke(serviceDescriptors) : serviceDescriptors;
    }
}
