// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Extensions;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.Board.AzureBoards.Commands
{
    /// <summary>
    /// Provides a set of methods for extends <see cref="ICakeContext"/>.
    /// </summary>
    [CakeAliasCategory("Board")]
    public static class WorkItemCommand
    {
        private static Func<IBoard, string, Task<IWorkItem>> _getWorkItemByIdBehaviourAsync = (board, id)
            => board.NotNull(nameof(board)).GetWorkItemByIdAsync(id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        private static Func<IBoard, string, Task<IEnumerable<IWorkItem>>> _getWorkItemsByQueryIdBehaviourAsync = (board, id)
            => board.NotNull(nameof(board)).GetWorkItemsByQueryIdAsync(id.ArgumentNotEmptyOrWhitespace(nameof(id)));

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
            string id) => await WorkItemCommand._getWorkItemByIdBehaviourAsync(
                board.NotNull(nameof(board)),
                id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        /// <summary>
        /// Fetch the <see cref="IWorkItem"/> by Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="personalAccessToken">The personal access token.</param>
        /// <param name="organization">The organization where the board is placed.</param>
        /// <param name="id">The work item id.</param>
        /// <returns>A <see cref="Task{IWorkItem}"/> representing the result of the asynchronous operation.</returns>
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
        /// Fetch the <see cref="IEnumerable{IWorkItem}"/> by query Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="board">The <see cref="IBoard"/>.</param>
        /// <param name="id">The query id.</param>
        /// <returns>An <see cref="IEnumerable{IWorkItem}"/>.</returns>
        [CakeMethodAlias]
        public static async Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(
            this ICakeContext context,
            IBoard board,
            string id) => await WorkItemCommand._getWorkItemsByQueryIdBehaviourAsync(
                board.NotNull(nameof(board)),
                id.ArgumentNotEmptyOrWhitespace(nameof(id)));

        /// <summary>
        /// Fetch the <see cref="IEnumerable{IWorkItem}"/> by query Id.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="personalAccessToken">The personal access token.</param>
        /// <param name="organization">The organization where the board is placed.</param>
        /// <param name="id">The query id.</param>
        /// <param name="project">The project where the board is placed.</param>
        /// <param name="team">The target team.</param>
        /// <returns>An <see cref="IEnumerable{IWorkItem}"/>.</returns>
        [CakeMethodAlias]
        public static async Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(
            this ICakeContext context,
            string personalAccessToken,
            string organization,
            string id,
            string project,
            string team)
        {
            var board = new AzureBoards(
                    personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)),
                    organization.ArgumentNotEmptyOrWhitespace(nameof(organization)))
            {
                Project = project.ArgumentNotEmptyOrWhitespace(nameof(project)),
                Team = team.ArgumentNotEmptyOrWhitespace(nameof(team))
            };

            return await context.GetWorkItemsByQueryIdAsync(
                    board,
                    id.ArgumentNotEmptyOrWhitespace(nameof(id)));
        }

        /// <summary>
        /// Produces release notes based on the work items provided.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="workItems">The work items.</param>
        /// <returns>A string that conteins release notes.</returns>
        public static string GenerateReleaseNotes(
            this ICakeContext context,
            IEnumerable<WorkItem> workItems) => new ReleaseNotes(workItems.Select(wit => (IWorkItem)wit).ToList()).Generate();

        /// <summary>
        /// Produces release notes based on the work items provided.
        /// </summary>
        /// <param name="context">The <see cref="ICakeContext"/> of precess.</param>
        /// <param name="releaseNotes">The <see cref="FilePath"/> where save release notes.</param>
        /// <param name="workItems">The work items.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public static Task GenerateReleaseNotesAsync(
            this ICakeContext context,
            FilePath releaseNotes,
            IEnumerable<WorkItem> workItems)
        {
            _ = context.GenerateReleaseNotes(workItems);
            throw new NotImplementedException();
        }
    }
}
