// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Cake.Board.Abstractions;

namespace Cake.Board.Asana.Models
{
    /// <summary>
    /// Represents an Asana task.
    /// </summary>
    public class Task : IWorkItem
    {
        /// <summary>
        /// Gets or sets task Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets task type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets task state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets task title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets task description.
        /// </summary>
        public string Description { get; set; }
    }
}
