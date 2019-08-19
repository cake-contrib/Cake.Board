// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace Cake.Board
{
    internal static class Resource
    {
        private static readonly ResourceManager _resourceManager = new ResourceManager("Cake.Board.Resource", typeof(Resource).Assembly);

        internal static string FormatReleaseNotes_Structure(object p0, object p1) => string.Format(CultureInfo.CurrentCulture, Resource.GetString("ReleaseNotes_Structure"), p0, p1);

        internal static string FormatReleaseNotes_WorkitemElement(object p0, object p1, object p2, object p3) => string.Format(CultureInfo.CurrentCulture, Resource.GetString("ReleaseNotes_WorkitemElement"), p0, p1, p2, p3);

        private static string GetString(string name, params string[] formatterNames)
        {
            string value = _resourceManager.GetString(name);

#pragma warning disable SA1405 // Debug.Assert should provide message text
            Debug.Assert(value != null);
#pragma warning restore SA1405 // Debug.Assert should provide message text

            if (formatterNames != null)
            {
                for (int i = 0; i < formatterNames.Length; i++)
                {
                    value = value.Replace("{" + formatterNames[i] + "}", "{" + i + "}");
                }
            }

            return value;
        }
    }
}
