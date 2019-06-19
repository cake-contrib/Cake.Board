// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

using Cake.Board.AzureBoards.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cake.Board.AzureBoards.Converters
{
    internal class WorkItemsConverter : JsonConverter<IEnumerable<WorkItem>>
    {
        public override IEnumerable<WorkItem> ReadJson(JsonReader reader, Type objectType, IEnumerable<WorkItem> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return null;

            JObject root = JObject.Load(reader);
            IEnumerable<JToken> items = root["workItemRelations"].Values<JObject>().AsEnumerable().Select(item => item["target"]);

            ICollection<WorkItem> workItems = new List<WorkItem>();

            foreach (JObject item in items)
            {
                WorkItem workItem = existingValue?.SingleOrDefault(i => i.Id == item["id"].Value<string>()) ?? new WorkItem();
                workItem.Id = string.IsNullOrEmpty(workItem?.Id) ? item["id"].Value<string>() : workItem.Id;
                workItem.Url = string.IsNullOrEmpty(workItem?.Url) ? item["url"].Value<string>() : workItem.Url;

                workItems.Add(workItem);
            }

            return workItems.AsEnumerable();
        }

        public override void WriteJson(JsonWriter writer, IEnumerable<WorkItem> value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
