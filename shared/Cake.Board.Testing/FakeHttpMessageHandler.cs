// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cake.Board.Testing
{
    /// <summary>
    /// Fake implementation of <see cref="HttpMessageHandler"/>.
    /// </summary>
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeHttpMessageHandler"/> class.
        /// </summary>
        /// <param name="responseMessage">Fake http response message returned by <see cref="HttpClient"/>.</param>
        public FakeHttpMessageHandler(HttpResponseMessage responseMessage) => this.FakeResponse = responseMessage;

        /// <summary>
        /// Gets or sets get or set fake http response message.
        /// </summary>
        public HttpResponseMessage FakeResponse { get; set; }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => await Task.FromResult(this.FakeResponse);
    }
}
