// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Core.IO;

namespace Cake.Board.AzureBoards.Models
{
    /// <summary>
    /// Todo.
    /// </summary>
    public class ReleaseNotes : IReleaseNotes<IWorkItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseNotes"/> class.
        /// </summary>
        /// <param name="workItems">Todo1.</param>
        public ReleaseNotes(ICollection<IWorkItem> workItems)
        {
            this.Enhancements = new List<IWorkItem>();
            this.BugFixes = new List<IWorkItem>();

            foreach (IWorkItem item in workItems)
            {
                switch (item.Type)
                {
                    case "Bug":
                    case "Issue":
                        this.BugFixes = this.BugFixes.Append(item);
                        break;
                    default:
                        this.Enhancements = this.Enhancements.Append(item);
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IWorkItem> Enhancements { get; private set; }

        /// <inheritdoc/>
        public IEnumerable<IWorkItem> BugFixes { get; private set; }

        /// <inheritdoc/>
        public string Generate() => Resource.FormatReleaseNotes_Structure(string.Join("  ", this.BugFixes.OrderBy(wit => wit.Id).Select(wit => wit.ToReleaseNotes())), string.Join("  ", this.Enhancements.OrderBy(wit => wit.Id).Select(wit => wit.ToReleaseNotes())));

        /// <inheritdoc/>
        public Task GenerateAsync(FilePath path) => throw new NotImplementedException();
    }
}
