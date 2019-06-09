// Copyright (c) Nicola Biancolini, 2019. All rights reserved.
// Licensed under the MIT license. See the LICENSE file in the project root for full license information.

using System;

using Cake.Board.AzureBoards.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cake.Board.AzureBoards.Converters
{
    internal class WorkItemConverter : JsonConverter<WorkItem>
    {
        public override WorkItem ReadJson(JsonReader reader, Type objectType, WorkItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
                return null;

            JObject root = JObject.Load(reader);
            JObject fields = root["fields"].Value<JObject>();

            return new WorkItem
            {
                Id = string.IsNullOrEmpty(existingValue?.Id) ? root["id"].Value<string>() : existingValue.Id,
                Type = string.IsNullOrEmpty(existingValue?.Type) ? fields["System.WorkItemType"].Value<string>() : existingValue.Type,
                Title = string.IsNullOrEmpty(existingValue?.Title) ? fields["System.Title"].Value<string>() : existingValue.Title,
                Description = string.IsNullOrEmpty(existingValue?.Description) ? fields["System.Description"].Value<string>() : existingValue.Description,
                State = string.IsNullOrEmpty(existingValue?.State) ? fields["System.State"].Value<string>() : existingValue.State,
                Url = string.IsNullOrEmpty(existingValue?.State) ? root["url"].Value<string>() : existingValue.State
            };
        }

        public override void WriteJson(JsonWriter writer, WorkItem value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
