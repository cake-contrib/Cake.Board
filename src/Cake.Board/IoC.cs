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

        private static IoC _instance;

        private IoC()
        {
        }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="cakeContext">Todo1.</param>
        /// <param name="board">Todo2.</param>
        /// <returns>Todo3.</returns>
        public static IoC WireUp(ICakeContext cakeContext, IBoard board)
        {
            if (IoC._instance == null)
                IoC._instance = new IoC().Configure(cakeContext, board);

            return IoC._instance;
        }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <typeparam name="T">Todo1.</typeparam>
        /// <returns>Todo2.</returns>
        public static T Get<T>() => IoC._services.GetService<T>();

        private IoC Configure(ICakeContext cakeContext, IBoard board)
        {
            Ensure.NotNull(cakeContext, nameof(cakeContext));
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(cakeContext.Log);
            serviceCollection.AddSingleton(board);

            IoC._services = serviceCollection.BuildServiceProvider();

            return this;
        }
    }
}
