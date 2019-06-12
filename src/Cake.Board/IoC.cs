// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Abstractions;
using Cake.Board.Extensions;
using Cake.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Board
{
    /// <summary>
    /// Todo.
    /// </summary>
    public class IoC
    {
        private static IServiceProvider _services;

        private IoC()
        {
        }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <typeparam name="TContainer">Todo1.</typeparam>
        /// <param name="container">Todo2.</param>
        /// <param name="context">Todo3.</param>
        public static void WireUp<TContainer>(TContainer container, ICakeContext context)
            where TContainer : IDependencyContainer
        {
            if (IoC._services != null)
                return;

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(context.Log);

            IoC._services = container.Configure(serviceCollection).BuildServiceProvider();
        }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <typeparam name="T">Todo1.</typeparam>
        /// <returns>Todo2.</returns>
        public static T Get<T>() => IoC._services.GetService<T>();
    }
}
