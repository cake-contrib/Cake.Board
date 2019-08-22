// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Cake.Board.Abstractions;
using Newtonsoft.Json;

namespace Cake.Board.AzureBoards.Models
{
    /// <summary>
    /// Represents an Azure Boards work item.
    /// </summary>
    public class WorkItem : IWorkItem
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Type { get; set; }

        /// <inheritdoc/>
        public string State { get; set; }

        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets work item url.
        /// </summary>
        public string Url { get; set; }
    }
}
