// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cake.Board.Abstractions
{
    /// <summary>
    /// Represents a board where your team plan, track and discuss work.
    /// </summary>
    public interface IBoard
    {
        /// <summary>
        /// Fetch work item by Id.
        /// </summary>
        /// <param name="id">The work item Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IWorkItem> GetWorkItemByIdAsync(string id);

        /// <summary>
        /// Fetch all work items by query Id.
        /// </summary>
        /// <param name="queryId">The query Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Obsolete("This method is deprecated, use ExecuteBatch instead.")]
        Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId);

        /// <summary>
        /// Fetch all work items by query Id in specific team.
        /// </summary>
        /// <param name="queryId">The query Id.</param>
        /// <param name="project">The project name.</param>
        /// <param name="team">The team's name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [Obsolete("This method is deprecated, use ExecuteBatch instead.")]
        Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId, string project, string team);

        /// <summary>
        /// Execute a set of commands.
        /// </summary>
        /// <param name="commands">The set of commands requests.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<string> ExecuteBatch(string commands);
    }
}
