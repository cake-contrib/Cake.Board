// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Core.Diagnostics;

namespace Cake.Board.Testing
{
    /// <summary>
    /// Fake implementation of <see cref="ICakeLog"/>.
    /// </summary>
    public class FakeCakeLog : ICakeLog
    {
        /// <inheritdoc/>
        public Verbosity Verbosity { get; set; }

        /// <inheritdoc/>
        public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
        {
        }
    }
}
