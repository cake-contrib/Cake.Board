// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Net.Http;

using Polly;
using Polly.Wrap;

namespace Cake.Board.Extensions
{
    /// <summary>
    /// Todo.
    /// </summary>
    public static class HttpPolicy
    {
        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> UnauthorizedPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.Unauthorized)
            .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> NotFoundPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GatewayTimeoutPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.GatewayTimeout)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> BadGatewayPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.BadGateway)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> InternalServerErrorPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> RequestTimeoutPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static IAsyncPolicy<HttpResponseMessage> ServiceUnavailablePolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Todo.
        /// </summary>
        /// <returns>Todo1.</returns>
        public static AsyncPolicyWrap<HttpResponseMessage> WrapAllAsync() => Policy
            .WrapAsync(
                HttpPolicy.BadGatewayPolicy(),
                HttpPolicy.GatewayTimeoutPolicy(),
                HttpPolicy.InternalServerErrorPolicy(),
                HttpPolicy.NotFoundPolicy(),
                HttpPolicy.RequestTimeoutPolicy(),
                HttpPolicy.ServiceUnavailablePolicy(),
                HttpPolicy.UnauthorizedPolicy());
    }
}
