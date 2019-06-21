// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

namespace Cake.Board.Extensions
{
    /// <summary>
    /// Represents a set of utilities to ensure parameters.
    /// </summary>
    public static class EnsureExtensions
    {
        /// <summary>
        /// Ensure the object is not null.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="object">The object.</param>
        /// <param name="parameterName">The parameter name.</param>
        /// <returns>A object pass as <paramref name="object"/>.</returns>
        public static T NotNull<T>(this T @object, string parameterName) => @object == null ? throw new ArgumentNullException(parameterName) : @object;

        /// <summary>
        /// Ensure the string is not null or empty.
        /// </summary>
        /// <param name="argument">The string value.</param>
        /// <param name="message">The message of <see cref="ArgumentException"/>.</param>
        /// <returns>A string pass as <paramref name="argument"/>.</returns>
        public static string ArgumentNotEmpty(this string argument, string message) => string.IsNullOrEmpty(argument) ? throw new ArgumentException(message) : argument;

        /// <summary>
        /// Ensure the string is not null, empty, or white space.
        /// </summary>
        /// <param name="argument">The string value.</param>
        /// <param name="message">The message of <see cref="ArgumentException"/>.</param>
        /// <returns>A string pass as <paramref name="argument"/>.</returns>
        public static string ArgumentNotEmptyOrWhitespace(this string argument, string message) => string.IsNullOrWhiteSpace(argument) ? throw new ArgumentException(message) : argument;
    }
}
