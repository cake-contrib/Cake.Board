// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Linq;
using Cake.Board.Abstractions;
using Cake.Board.Extensions;
using Newtonsoft.Json;

namespace Cake.Board.AzureBoards.Models
{
    /// <summary>
    /// Azure board work item.
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

        /// <inheritdoc/>
        public void ToReleaseNotes<T>(ref IReleaseNotes<T> releaseNote)
            where T : IWorkItem
        {
            releaseNote.NotNull(nameof(releaseNote));

            if (this.Type == "Bug" && this.Type == "Issue")
                releaseNote.BugFixes.ToList().Add((T)this);
            else
                releaseNote.Enhancements.ToList().Add(this);
        }
    }
}
