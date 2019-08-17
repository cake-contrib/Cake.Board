// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

using Cake.Core.IO;

namespace Cake.Board.Abstractions
{
    /// <summary>
    /// Rappresents a list of change.
    /// </summary>
    /// <typeparam name="T">Concrete implementation of <see cref="IWorkItem"/>.</typeparam>
    public interface IReleaseNotes<T>
        where T : IWorkItem
    {
        /// <summary>
        /// Gets list of enhancements.
        /// </summary>
        IEnumerable<T> Enhancements { get; }

        /// <summary>
        /// Gets list of bug fixes.
        /// </summary>
        IEnumerable<T> BugFixes { get; }

        /// <summary>
        /// Generate release notes.
        /// </summary>
        /// <returns>A list of bytes representing the contents of the release notes.</returns>
        Task<byte[]> GenerateAsync();

        /// <summary>
        /// Generate release notes.
        /// </summary>
        /// <param name="path">The <see cref="FilePath"/> where the release notes will be saved.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task GenerateAsync(FilePath path);
    }
}
