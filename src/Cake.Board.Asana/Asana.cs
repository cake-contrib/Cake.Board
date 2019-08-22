// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Board.Asana.Converters;
using Cake.Board.Extensions;
using Newtonsoft.Json;

namespace Cake.Board.Asana
{
    /// <summary>
    /// Provides an object representation of <see href="https://asana.com/">Asana</see>.
    /// </summary>
    public class Asana : IBoard
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Asana"/> class.
        /// </summary>
        /// <param name="personalAccessToken">Personal Access Token <see href="https://asana.com/developers/documentation/getting-started/quick-start">Authenticate access with personal access tokens</see>.</param>
        public Asana(string personalAccessToken)
            : this(new HttpClient())
        {
            this._client.BaseAddress = new Uri($"https://app.asana.com/api/1.0");
            this._client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Asana"/> class.
        /// </summary>
        /// <param name="client">The preconfigured <see cref="HttpClient"/> with sets <see cref="AuthenticationHeaderValue"/>.</param>
        /// <example>
        /// <code>
        /// var client = new HttpClient
        /// {
        ///     BaseAddress = new Uri($"https://app.asana.com/api/1.0")
        /// };
        /// client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", {personalAccessToken})));
        ///
        /// var board = new AzureBoards(client);
        /// </code>
        /// </example>
        public Asana(HttpClient client) => this._client = client.NotNull(nameof(client));

        /// <summary>
        /// Gets or sets Asana project name.
        /// </summary>
        public string Project { get; set; }

        /// <inheritdoc/>
        public Task<string> ExecuteBatch(string commands) => throw new NotImplementedException();

        /// <inheritdoc/>
        public async Task<IWorkItem> GetWorkItemByIdAsync(string id)
        {
            HttpResponseMessage response = await HttpPolicyExtensions.WrapAllAsync()
                .ExecuteAsync(async () => await this._client.GetAsync($"{this._client.BaseAddress}/tasks/{id.ArgumentNotEmptyOrWhitespace(nameof(id))}"));

            return JsonConvert.DeserializeObject<Models.Task>(await response.Content.ReadAsStringAsync(), new TaskConverter());
        }

        /// <summary>
        /// Fetch all work items by project.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<IWorkItem>> GetWorkItemsByProjectAsync() => await this.GetWorkItemsByProjectAsync(
            string.IsNullOrWhiteSpace(this.Project) ? throw new InvalidOperationException($"{nameof(this.Project)} is null, empty or whitespace") : this.Project);

        /// <summary>
        /// Fetch all work items by project.
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<IEnumerable<IWorkItem>> GetWorkItemsByProjectAsync(string project)
        {
            HttpResponseMessage response = await HttpPolicyExtensions.WrapAllAsync()
                .ExecuteAsync(async () => await this._client.GetAsync($"{this._client.BaseAddress}/projects/{project.ArgumentNotEmptyOrWhitespace(nameof(project))}/tasks"));

            return JsonConvert.DeserializeObject<IEnumerable<Models.Task>>(await response.Content.ReadAsStringAsync(), new TasksConverter());
        }

        /// <inheritdoc/>
        public Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId) => throw new NotImplementedException();

        /// <inheritdoc/>
        public Task<IEnumerable<IWorkItem>> GetWorkItemsByQueryIdAsync(string queryId, string project, string team) => throw new NotImplementedException();
    }
}
