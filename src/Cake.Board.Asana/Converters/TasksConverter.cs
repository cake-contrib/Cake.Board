// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

using Cake.Board.Asana.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cake.Board.Asana.Converters
{
    internal class TasksConverter : JsonConverter<IEnumerable<Task>>
    {
        public override IEnumerable<Task> ReadJson(JsonReader reader, Type objectType, IEnumerable<Task> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return null;

            JObject root = JObject.Load(reader);

            ICollection<Task> workItems = new List<Task>();

            foreach (JObject item in root["data"].Values<JObject>().AsEnumerable())
            {
                Task workItem = existingValue?.SingleOrDefault(i => i.Id == item["id"].Value<string>()) ?? new Task();
                workItem.Id = string.IsNullOrEmpty(workItem?.Id) ? item["id"].Value<long>().ToString() : workItem.Id;
                workItem.Type = string.IsNullOrEmpty(workItem?.Type) ? item["resource_type"].Value<string>() : workItem.Type;
                workItem.Title = string.IsNullOrEmpty(workItem?.Title) ? item["name"].Value<string>() : workItem.Title;
                workItems.Add(workItem);
            }

            return workItems.AsEnumerable();
        }

        public override void WriteJson(JsonWriter writer, IEnumerable<Task> value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
