// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.Extensions;
using Cake.Core;
using Cake.Core.Annotations;

namespace Cake.Board.AzureBoards.Commands
{
    /// <summary>
    /// Todo.
    /// </summary>
    [CakeAliasCategory("Board")]
    public static class WorkItemCommand
    {
        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="board">Todo4.</param>
        /// <param name="id">Todo2.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [CakeMethodAlias]
        public static async Task<IWorkItem> GetWorkItemByIdAsync(
            this ICakeContext context,
            IBoard board,
            string id) => await board.GetWorkItemByIdAsync(
                id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="personalAccessToken">Todo2.</param>
        /// <param name="organization">Todo3.</param>
        /// <param name="id">Todo4.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<IWorkItem> GetWorkItemByIdAsync(
            this ICakeContext context,
            string personalAccessToken,
            string organization,
            string id) => await context.GetWorkItemByIdAsync(
                new AzureBoards(
                    personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)),
                    organization.ArgumentNotEmptyOrWhitespace(nameof(organization))),
                id);

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="board">Todo7.</param>
        /// <param name="id">Todo2.</param>
        /// <param name="project">Todo4.</param>
        /// <param name="team">Todo5.</param>
        /// <returns>Todo9.</returns>
        [CakeMethodAlias]
        public static async Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(
            this ICakeContext context,
            IBoard board,
            string id,
            string project,
            string team) => await board.GetWorkItemsByQueryIdAsync(
                id.ArgumentNotEmptyOrWhitespace(nameof(id)),
                project.ArgumentNotEmptyOrWhitespace(nameof(project)),
                team.ArgumentNotEmptyOrWhitespace(nameof(team)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="personalAccessToken">Todo6.</param>
        /// <param name="organization">Todo10.</param>
        /// <param name="id">Todo2.</param>
        /// <param name="project">Todo4.</param>
        /// <param name="team">Todo5.</param>
        /// <returns>Todo9.</returns>
        [CakeMethodAlias]
        public static async Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(
            this ICakeContext context,
            string personalAccessToken,
            string organization,
            string id,
            string project,
            string team) => await context.GetWorkItemsByQueryIdAsync(
                new AzureBoards(
                    personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)),
                    organization.ArgumentNotEmptyOrWhitespace(nameof(organization))),
                id.ArgumentNotEmptyOrWhitespace(nameof(id)),
                project.ArgumentNotEmptyOrWhitespace(nameof(project)),
                team.ArgumentNotEmptyOrWhitespace(nameof(team)));
    }
}
