// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Extensions;
using Xunit;

namespace Cake.Board.Tests.Units
{
    /// <summary>
    /// Todo.
    /// </summary>
    public class EnsureUnit
    {
        /// <summary>
        /// Todo.
        /// </summary>
        public void ScenarioWithArgumentNull()
        {
            // Arrange
            var argument = default(Exception);

            // Act
            Exception record = Record.Exception(() => argument.NotNull("lorem ipsum"));

            // Assert
            Assert.IsType<ArgumentNullException>(record);
        }
    }
}
