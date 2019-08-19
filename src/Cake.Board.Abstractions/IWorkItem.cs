// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

namespace Cake.Board.Abstractions
{
    /// <summary>
    /// Represents a part of your team's development history.
    /// </summary>
    public interface IWorkItem
    {
        /// <summary>
        /// Gets or sets work item Id.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets work item type.
        /// </summary>
        string Type { get; set; }

        /// <summary>
        /// Gets or sets work item state.
        /// </summary>
        string State { get; set; }

        /// <summary>
        /// Gets or sets work item title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets work item description.
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Add work item to release note.
        /// </summary>
        /// <returns>A string rappresenting the content of work item.</returns>
        string ToReleaseNotes();
    }
}
