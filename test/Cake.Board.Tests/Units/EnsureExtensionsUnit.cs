// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Extensions;
using Cake.Board.Testing;
using Xunit;

namespace Cake.Board.Tests.Units
{
    public class EnsureExtensionsUnit
    {
        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithArgumentNull()
        {
            // Arrange
            var argument = default(Exception);
            string message = "lorem ipsum";

            // Act
            Exception record = Record.Exception(() => argument.NotNull(message));

            // Assert
            Assert.IsType<ArgumentNullException>(record);
            Assert.Equal(new ArgumentNullException(message).Message, record.Message);
        }

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithArgumentNotNull()
        {
            // Arrange
            var argument = new Exception();
            string message = "lorem ipsum";

            // Act
            Exception record = Record.Exception(() => argument.NotNull(message));

            // Assert
            Assert.Null(record);
        }

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithArgumentEmpty()
        {
            // Arrange
            string argument = string.Empty;
            string message = "lorem ipsum";

            // Act
            Exception record = Record.Exception(() => argument.ArgumentNotEmpty(message));

            // Assert
            Assert.IsType<ArgumentException>(record);
            Assert.Equal(new ArgumentException(message).Message, record.Message);
        }

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithArgumentNotEmpty()
        {
            // Arrange
            string argument = "lorem ipsum";
            string message = "lorem ipsum";

            // Act
            Exception record = Record.Exception(() => argument.ArgumentNotEmpty(message));

            // Assert
            Assert.Null(record);
        }

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithArgumentWithWhitespace()
        {
            // Arrange
            string argument = " ";
            string message = "lorem ipsum";

            // Act
            Exception record = Record.Exception(() => argument.ArgumentNotEmptyOrWhitespace(message));

            // Assert
            Assert.IsType<ArgumentException>(record);
            Assert.Equal(new ArgumentException(message).Message, record.Message);
        }

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithArgumentWithoutWhitespace()
        {
            // Arrange
            string argument = "lorem ipsum";
            string message = "lorem ipsum";

            // Act
            Exception record = Record.Exception(() => argument.ArgumentNotEmptyOrWhitespace(message));

            // Assert
            Assert.Null(record);
        }
    }
}
