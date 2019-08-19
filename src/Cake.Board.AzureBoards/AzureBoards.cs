// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.AzureBoards.Converters;
using Cake.Board.AzureBoards.Models;
using Cake.Board.Extensions;
using Newtonsoft.Json;

namespace Cake.Board.AzureBoards
{
    /// <summary>
    /// Provides an object representation of <see href="https://docs.microsoft.com/azure/devops/boards/boards">Azure Boards</see>.
    /// </summary>
    public class AzureBoards : IBoard
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBoards"/> class.
        /// </summary>
        /// <param name="personalAccessToken">Personal Access Token <see href="https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate">Authenticate access with personal access tokens</see>.</param>
        /// <param name="organization">The Azure DevOps Organization.</param>
        public AzureBoards(string personalAccessToken, string organization)
            : this(new HttpClient())
        {
            this._client.BaseAddress = new Uri($"https://dev.azure.com/{organization.ArgumentNotEmptyOrWhitespace(nameof(organization))}");
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.UTF8.GetBytes($":{personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken))}")));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBoards"/> class.
        /// </summary>
        /// <param name="client">The preconfigured <see cref="HttpClient"/> with sets <see cref="AuthenticationHeaderValue"/>.</param>
        /// <example>
        /// <code>
        /// var client = new HttpClient
        /// {
        ///     BaseAddress = new Uri($"https://dev.azure.com/{organization.ArgumentNotEmptyOrWhitespace(nameof(organization))}")
        /// };
        /// client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($":{personalAccessToken}")));
        ///
        /// var board = new AzureBoards(client);
        /// </code>
        /// </example>
        public AzureBoards(HttpClient client) => this._client = client.NotNull(nameof(client));

        /// <summary>
        /// Gets or sets Azure DevOps project name.
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets Azure DevOps team project name.
        /// </summary>
        public string Team { get; set; }

        /// <inheritdoc/>
        public async Task<IWorkItem> GetWorkItemByIdAsync(string id)
        {
            HttpResponseMessage response = await HttpPolicyExtensions.WrapAllAsync()
                .ExecuteAsync(async () =>
                    await this._client.GetAsync($"{this._client.BaseAddress}/_apis/wit/workItems/{id.ArgumentNotEmptyOrWhitespace(nameof(id))}"));

            return JsonConvert.DeserializeObject<WorkItem>(await response.Content.ReadAsStringAsync(), new WorkItemConverter());
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId) => await this.GetWorkItemsByQueryIdAsync(
            queryId.ArgumentNotEmptyOrWhitespace(nameof(queryId)),
            string.IsNullOrWhiteSpace(this.Project) ? throw new InvalidOperationException($"{nameof(this.Project)} is null, empty or whitespace") : this.Project,
            string.IsNullOrWhiteSpace(this.Team) ? throw new InvalidOperationException($"{nameof(this.Team)} is null, empty or whitespace") : this.Team);

        /// <inheritdoc/>
        public async Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId, string project, string team)
        {
            HttpResponseMessage response = await HttpPolicyExtensions.WrapAllAsync()
                .ExecuteAsync(async () =>
                    await this._client.GetAsync($"{this._client.BaseAddress}/{project.ArgumentNotEmptyOrWhitespace(nameof(project))}/{team.ArgumentNotEmptyOrWhitespace(nameof(team))}/_apis/wit/wiql/{queryId.ArgumentNotEmptyOrWhitespace(nameof(queryId))}"));

            return JsonConvert.DeserializeObject<IEnumerable<WorkItem>>(await response.Content.ReadAsStringAsync(), new WorkItemsConverter());
        }

        /// <inheritdoc/>
        public Task<string> ExecuteBatch(string commands) => throw new NotImplementedException();
    }
}
