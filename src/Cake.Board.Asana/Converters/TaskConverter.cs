// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.Asana.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cake.Board.Asana.Converters
{
    internal class TaskConverter : JsonConverter<Task>
    {
        public override Task ReadJson(JsonReader reader, Type objectType, Task existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return null;

            JObject data = JObject.Load(reader)["data"].Value<JObject>();

            return new Task
            {
                Id = string.IsNullOrEmpty(existingValue?.Id) ? data["id"].Value<long>().ToString() : existingValue.Id,
                Type = string.IsNullOrEmpty(existingValue?.Type) ? data["resource_type"].Value<string>() : existingValue.Type,
                Title = string.IsNullOrEmpty(existingValue?.Title) ? data["name"].Value<string>() : existingValue.Title,
                Description = string.IsNullOrEmpty(existingValue?.Description) ? data["notes"].Value<string>() : existingValue.Description,
                State = string.IsNullOrEmpty(existingValue?.State) ? data["assignee_status"].Value<string>() : existingValue.State
            };
        }

        public override void WriteJson(JsonWriter writer, Task value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
