// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.Extensions;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Board
{
    /// <summary>
    /// Provides a set of methods for extends <see cref="ICakeContext"/>.
    /// </summary>
    [CakeAliasCategory("BoardCommand")]
    public static class BoardCommandAliases
    {
        private static Func<IBoard, string, Task<IWorkItem>> _getWorkItemByIdBehaviourAsync = (board, id)
            => board.NotNull(nameof(board)).GetWorkItemByIdAsync(id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        /// <summary>
        /// Fetch the <see cref="IWorkItem"/> by Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="board">The <see cref="IBoard"/>.</param>
        /// <param name="id">The work item id.</param>
        /// <returns>A <see cref="Task{IWorkItem}"/> representing the result of the asynchronous operation.</returns>
        [CakeMethodAlias]
        public static async Task<IWorkItem> GetWorkItemByIdAsync(
            this ICakeContext context,
            IBoard board,
            string id) => await BoardCommandAliases._getWorkItemByIdBehaviourAsync(
                board.NotNull(nameof(board)),
                id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="board">Todo2.</param>
        /// <param name="commands">Todo3.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<string> ExecuteBatch(
            this ICakeContext context,
            IBoard board,
            string commands) => await board.ExecuteBatch(commands);
    }
}
