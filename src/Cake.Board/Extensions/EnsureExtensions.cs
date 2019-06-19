// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

namespace Cake.Board.Extensions
{
    /// <summary>
    /// Todo.
    /// </summary>
    public static class EnsureExtensions
    {
        /// <summary>
        /// Todo.
        /// </summary>
        /// <typeparam name="T">Todo1.</typeparam>
        /// <param name="object">Todo2.</param>
        /// <param name="message">Todo3.</param>
        /// <returns>Todo4.</returns>
        public static T NotNull<T>(this T @object, string message) => @object == null ? throw new ArgumentNullException(message) : @object;

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="argument">Todo1.</param>
        /// <param name="message">Todo2.</param>
        /// <returns>Todo3.</returns>
        public static string ArgumentNotEmpty(this string argument, string message) => string.IsNullOrEmpty(argument) ? throw new ArgumentException(message) : argument;

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="argument">Todo1.</param>
        /// <param name="message">Todo2.</param>
        /// <returns>Todo3.</returns>
        public static string ArgumentNotEmptyOrWhitespace(this string argument, string message) => string.IsNullOrWhiteSpace(argument) ? throw new ArgumentException(message) : argument;
    }
}
