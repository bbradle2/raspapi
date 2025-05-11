using System.Text.Json;
using System.Text.Json.Serialization;

namespace raspapi.Utils
{
    internal class DictionaryInt32StringKeyValueConverter : JsonConverter<Dictionary<int, string>>
    {
        private JsonConverter<KeyValuePair<int, string>> _intToStringConverter;

        public DictionaryInt32StringKeyValueConverter(JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            _intToStringConverter = (JsonConverter<KeyValuePair<int, string>>)options.GetConverter(typeof(KeyValuePair<int, string>));

            // KeyValuePair<> converter is built-in.
            //Debug.Assert(_intToStringConverter != null);
        }

        public override Dictionary<int, string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var value = new Dictionary<int, string>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return value;
                }

                KeyValuePair<int, string> kvpair = _intToStringConverter.Read(ref reader, typeToConvert, options);

                value.Add(kvpair.Key, kvpair.Value);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Dictionary<int, string> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (KeyValuePair<int, string> item in value)
            {
                _intToStringConverter.Write(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }
}