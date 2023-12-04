using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ObjectPropertySearch.Domain.Formatters
{

    /// <summary>
    /// The MS System.Text.Json JsonConverter does not handle objects like the NewtonSoft Json does
    /// This will attempt to convert the value as best as possible
    /// </summary>
    /// <remarks>
    /// Idea came from the following sources:
    /// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-7-0#deserialize-inferred-types-to-object-properties
    /// https://github.com/dotnet/runtime/issues/29960
    /// </remarks>
    public class JsonObjectConverter : JsonConverter<object>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object? ret = null;

            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                case JsonTokenType.None:
                    ret = null;
                    break;

                case JsonTokenType.True:
                case JsonTokenType.False:
                    ret = reader.GetBoolean();
                    break;

                case JsonTokenType.Number:
                    if (reader.TryGetInt32(out int i))
                        ret = i;
                    else if (reader.TryGetInt64(out long lng))
                        ret = lng;
                    else if (reader.TryGetDecimal(out decimal dml))
                        ret = dml;
                    else
                        ret = reader.GetDouble();
                    break;

                case JsonTokenType.String:
                    if (reader.TryGetDateTime(out DateTime dt))
                        ret = dt;
                    else
                        ret = reader.GetString();
                    break;

                case JsonTokenType.StartArray:
                    bool breakArrayLoop = false;
                    IList<object> list = new List<object>();
                    while (!breakArrayLoop && reader.Read())    // Make sure to check to exit loop before reading in data
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.EndArray:
                                ret = list;
                                breakArrayLoop = true;
                                break;

                            default:
                                list.Add(Read(ref reader, typeof(object), options));
                                break;
                        }
                    }
                    break;

                case JsonTokenType.StartObject:
                    bool breakObjectLoop = false;
                    IDictionary<string, object> dict = new Dictionary<string, object>();
                    while (!breakObjectLoop && reader.Read())   // Make sure to check to exit loop before reading in data
                    {
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.EndObject:
                                ret = dict;
                                breakObjectLoop = true;
                                break;

                            case JsonTokenType.PropertyName:
                                string key = reader.GetString();
                                reader.Read();
                                dict.Add(key, Read(ref reader, typeof(object), options));
                                break;

                            default:
                                throw new JsonException();
                        }
                    }
                    break;

                default:
                    ret = JsonDocument.ParseValue(ref reader).RootElement.Clone();
                    break;
            }

            return ret;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
