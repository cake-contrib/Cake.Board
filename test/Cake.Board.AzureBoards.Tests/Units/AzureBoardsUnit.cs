// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Reflection;
using System.Text;

using Cake.Board.Testing;
using Xunit;

namespace Cake.Board.AzureBoards.Tests.Units
{
    public class AzureBoardsUnit
    {
        [Fact]
        [Trait(TraitNames.TEST_CATEGORY, TraitValues.UNIT_TEST)]
        public void Scenario_CtorWithPersonalAccessTokenAndOrganization()
        {
            // Arrange
            string pat = "someone-pat";
            string organization = "someone-organization";

            // Act
            var board = new AzureBoards(pat, organization);
            var client = (HttpClient)typeof(AzureBoards).GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(board);

            // Assert
            Assert.Equal(new Uri($"https://dev.azure.com/{organization}"), client.BaseAddress);
            Assert.Equal(Convert.ToBase64String(Encoding.UTF8.GetBytes($":{pat}")), client.DefaultRequestHeaders.Authorization.Parameter);
        }
    }
}
