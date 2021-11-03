﻿using BinarySerializer;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Ray1Map
{
    public class PointerJsonConverter : JsonConverter {
        const string PointerPattern = @"^(?<file>[^\|]*)\|0x(?<offset>[a-fA-F0-9]{8})(\[0x(?<offsetInFile>[a-fA-F0-9]{8})\])?$";
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(Pointer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            if (value == null) {
                writer.WriteNull();
            } else {
                writer.WriteValue(value.ToString());
            }
        }

        public override bool CanRead {
            get { return true; }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.Value is string str) {
                Match match = Regex.Match(str, PointerPattern);
                if (match.Success) {
                    string offset = match.Groups["offset"].Value;
                    string file = match.Groups["file"].Value;
                    if (Int64.TryParse(offset, System.Globalization.NumberStyles.HexNumber, default, out long result)) {
                        BinaryFile f = LevelEditorData.MainContext.GetFile(file);
                        if (f != null) {
                            return new Pointer(result, f);
                        }
                    }
                }
            }
            return existingValue;
        }
    }
}
