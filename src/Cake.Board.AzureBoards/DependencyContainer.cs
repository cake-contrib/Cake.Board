// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Cake.Board.Abstractions;
using Cake.Board.Extensions;
using Cake.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Cake.Board.AzureBoards
{
    internal class DependencyContainer : IDependencyContainer
    {
        private readonly HttpClient _client;

        public DependencyContainer(string personalAccessToken, string organization)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri($"https://dev.azure.com/{organization.ArgumentNotEmptyOrWhitespace(nameof(organization))}")
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($":{personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken))}")));

            this._client = client;
        }

        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="serviceDescriptors">Todo1.</param>
        /// <returns>Todo3.</returns>
        public IServiceCollection Configure(IServiceCollection serviceDescriptors)
        {
            Ensure.NotNull(serviceDescriptors, nameof(serviceDescriptors));
            serviceDescriptors.AddSingleton(new AzureBoards(this._client));

            return serviceDescriptors;
        }
    }
}
