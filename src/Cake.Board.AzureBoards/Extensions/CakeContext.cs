// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using Cake.Board.Extensions;
using Cake.Core;

namespace Cake.Board.AzureBoards.Extensions
{
    /// <summary>
    /// Todo.
    /// </summary>
    internal static class CakeContext
    {
        /// <summary>
        /// Todo.
        /// </summary>
        /// <param name="context">Todo1.</param>
        /// <param name="personalAccessToken">Todo2.</param>
        /// <param name="organization">Todo3.</param>
        public static void WireUp(this ICakeContext context, string personalAccessToken, string organization) => IoC.WireUp(
            new DependencyContainer(
                personalAccessToken.ArgumentNotEmptyOrWhitespace(nameof(personalAccessToken)),
                organization.ArgumentNotEmptyOrWhitespace(nameof(organization))),
            context);
    }
}
