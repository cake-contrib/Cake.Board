// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.Extensions;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Board.Asana
{
    /// <summary>
    /// Provides a set of methods for extends <see cref="ICakeContext"/>.
    /// </summary>
    [CakeAliasCategory("AsanaCommand")]
    public static class AsanaCommandAliases
    {
        [Obsolete]
        private static Func<IBoard, string, Task<IEnumerable<IWorkItem>>> _getTasksByProjectIdBehaviourAsync = (board, project)
            => ((Asana)board.NotNull(nameof(board))).GetWorkItemsByProjectIdAsync(project.ArgumentNotEmptyOrWhitespace(nameof(project)));

        /// <summary>
        /// Fetch the <see cref="IWorkItem"/> by Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="personalAccessToken">The personal access token.</param>
        /// <param name="id">The task id.</param>
        /// <returns>A <see cref="Task{IWorkItem}"/> representing the result of the asynchronous operation.</returns>
        [CakeMethodAlias]
        public static async Task<IWorkItem> GetTaskByIdAsync(
            this ICakeContext context,
            string personalAccessToken,
            string id) => await context.GetWorkItemByIdAsync(
                new Asana(personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken))),
                id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        /// <summary>
        /// Fetch the <see cref="IEnumerable{IWorkItem}"/> by query Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="board">The <see cref="IBoard"/>.</param>
        /// <param name="projectId">The project id.</param>
        /// <returns>An <see cref="IEnumerable{IWorkItem}"/>.</returns>
        [CakeMethodAlias]
        [Obsolete]
        public static async Task<IEnumerable<IWorkItem>> GetTasksByProjectIdAsync(
            this ICakeContext context,
            IBoard board,
            string projectId) => await AsanaCommandAliases._getTasksByProjectIdBehaviourAsync(
                board.NotNull(nameof(board)),
                projectId.ArgumentNotEmptyOrWhitespace(nameof(projectId)));

        /// <summary>
        /// Fetch the <see cref="IEnumerable{IWorkItem}"/> by query Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="personalAccessToken">The personal access token.</param>
        /// <param name="projectId">The project id.</param>
        /// <returns>An <see cref="IEnumerable{IWorkItem}"/>.</returns>
        [CakeMethodAlias]
        [Obsolete]
        public static async Task<IEnumerable<IWorkItem>> GetTasksByProjectIdAsync(
            this ICakeContext context,
            string personalAccessToken,
            string projectId)
        {
            Asana board = new Asana(personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)))
            {
                ProjectId = projectId.ArgumentNotEmptyOrWhitespace(nameof(projectId))
            };

            return await context.GetTasksByProjectIdAsync(board, board.ProjectId);
        }
    }
}
