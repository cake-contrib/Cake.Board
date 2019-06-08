// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Board.Abstractions
{
    /// <summary>
    /// Todo.
    /// </summary>
    public interface IBoard
    {
        /// <summary>
        /// Fetch work item by Id.
        /// </summary>
        /// <param name="id">Work item Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IWorkItem> GetWorkItemByIdAsync(string id);

        /// <summary>
        /// Fetch all work items by query Id.
        /// </summary>
        /// <param name="queryId">Query Id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId);

        /// <summary>
        /// Fetch all work items by query Id in specific team.
        /// </summary>
        /// <param name="queryId">Query Id.</param>
        /// <param name="project">Project name.</param>
        /// <param name="team">Team name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId, string project, string team);
    }
}
