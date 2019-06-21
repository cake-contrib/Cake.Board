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
    /// Represents a set of http policies.
    /// </summary>
    public static class HttpPolicyExtensions
    {
        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.Unauthorized"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> UnauthorizedPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.Unauthorized)
            .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.NotFound"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> NotFoundPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.NotFound)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.GatewayTimeout"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> GatewayTimeoutPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.GatewayTimeout)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.BadGateway"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> BadGatewayPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.BadGateway)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.InternalServerError"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> InternalServerErrorPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.RequestTimeout"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> RequestTimeoutPolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.RequestTimeout)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents the policy for <see cref="HttpResponseMessage"/> with <see cref="HttpStatusCode.ServiceUnavailable"/>.
        /// </summary>
        /// <returns>A <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static IAsyncPolicy<HttpResponseMessage> ServiceUnavailablePolicy() => Policy
            .HandleResult<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.ServiceUnavailable)
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        /// <summary>
        /// Represents an set of all http policies.
        /// </summary>
        /// <returns>A <see cref="AsyncPolicyWrap{HttpResponseMessage}"/> with all <see cref="IAsyncPolicy{HttpResponseMessage}"/>.</returns>
        public static AsyncPolicyWrap<HttpResponseMessage> WrapAllAsync() => Policy
            .WrapAsync(
                HttpPolicyExtensions.BadGatewayPolicy(),
                HttpPolicyExtensions.GatewayTimeoutPolicy(),
                HttpPolicyExtensions.InternalServerErrorPolicy(),
                HttpPolicyExtensions.NotFoundPolicy(),
                HttpPolicyExtensions.RequestTimeoutPolicy(),
                HttpPolicyExtensions.ServiceUnavailablePolicy(),
                HttpPolicyExtensions.UnauthorizedPolicy());
    }
}
