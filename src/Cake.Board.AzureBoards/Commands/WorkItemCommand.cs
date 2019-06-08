// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
    public static class WorkItemCommand
    {
        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="id">Todo2.</param>
        /// <param name="organization">Todo3.</param>
        /// <param name="personalAccessToken">Todo6.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public static async Task<IWorkItem> GetWorkItemByIdAsync(this ICakeContext context, string id, string organization, string personalAccessToken)
        {
            WorkItemCommand.WireUp(
                context.NotNull(nameof(context)),
                personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)),
                organization.ArgumentNotEmptyOrWhitespace(nameof(organization)));

            AzureBoards board = IoC.Get<AzureBoards>();

            return await board.GetWorkItemByIdAsync(id.ArgumentNotEmptyOrWhitespace(nameof(id)));
        }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="id">Todo2.</param>
        /// <param name="organization">Todo3.</param>
        /// <param name="project">Todo4.</param>
        /// <param name="team">Todo5.</param>
        /// <param name="personalAccessToken">Todo6.</param>
        /// <returns>Todo7.</returns>
        [CakeMethodAlias]
        public static async Task<IEnumerable<IWorkItem>> GetWorkItemByQueryIdAsync(this ICakeContext context, string id, string organization, string project, string team, string personalAccessToken)
        {
            WorkItemCommand.WireUp(
                context.NotNull(nameof(context)),
                personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)),
                organization.ArgumentNotEmptyOrWhitespace(nameof(organization)));

            AzureBoards board = IoC.Get<AzureBoards>();

            return await board.GetWorkItemsByQueryIdAsync(id.ArgumentNotEmptyOrWhitespace(nameof(id)), project.ArgumentNotEmptyOrWhitespace(nameof(project)), team.ArgumentNotEmptyOrWhitespace(nameof(team)));
        }

        private static void WireUp(ICakeContext context, string personalAccessToken, string organization)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri($"dev.azure.com/{organization.ArgumentNotEmptyOrWhitespace(nameof(organization))}")
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($":{personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken))}")));

            IoC.WireUp(context.NotNull(nameof(context)), new AzureBoards(client));
        }
    }
}
