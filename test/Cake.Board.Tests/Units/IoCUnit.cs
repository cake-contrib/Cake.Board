// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;

using Cake.Board.Testing;
using Cake.Core.Diagnostics;
using Xunit;

namespace Cake.Board.Tests.Units
{
    public class IoCUnit
    {
        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithFulfilledContainerAndFulfilledContext()
        {
            // Arrange
            var log = new FakeCakeLog();
            var cakeContext = new FakeCakeContext(logBehaviour: () => log);
            var depencencyContainer = new FakeDependencyContainer();

            // Act
            IoC.WireUp(depencencyContainer, cakeContext);

            // Assert
            object field = typeof(IoC).Assembly.GetTypes().First(t => t.Name == nameof(IoC)).GetFields(BindingFlags.NonPublic | BindingFlags.Static).Single().GetValue(null);
            Assert.NotNull(field);
            Assert.IsAssignableFrom<IServiceProvider>(field);

            var serviceProvider = (IServiceProvider)field;
            Assert.NotNull(serviceProvider.GetService(typeof(ICakeLog)));
        }

        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void ScenarioWithAlreadyWired()
        {
            // Arrange
            var log = new FakeCakeLog();
            var cakeContext = new FakeCakeContext(logBehaviour: () => log);
            var depencencyContainer = new FakeDependencyContainer();

            // Act
            IoC.WireUp(depencencyContainer, cakeContext);

            // Assert
            // Todo: assert that returned the instance of IServiceProvider already instantiade.
        }
    }
}
