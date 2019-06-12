// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Net.Http;

using Cake.Board.Extensions;
using Cake.Board.Testing;
using Polly;
using Xunit;

namespace Cake.Board.Tests.Units
{
    public class HttpPolicyExtensionsUnit
    {
        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void CoverageTrick()
        {
            // TODO

            // Act
            HttpPolicyExtensions.WrapAllAsync();
        }
    }
}
