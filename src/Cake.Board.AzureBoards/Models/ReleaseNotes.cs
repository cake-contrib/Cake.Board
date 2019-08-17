// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Cake.Board.Abstractions;
using Cake.Core.IO;

namespace Cake.Board.AzureBoards.Models
{
    internal class ReleaseNotes : IReleaseNotes<WorkItem>
    {
        public IEnumerable<WorkItem> Enhancements { get; private set; }

        public IEnumerable<WorkItem> BugFixes { get; private set; }

        public Task<byte[]> GenerateAsync()
        {
            throw new NotImplementedException();
        }

        public Task GenerateAsync(FilePath path)
        {
            throw new NotImplementedException();
        }
    }
}
