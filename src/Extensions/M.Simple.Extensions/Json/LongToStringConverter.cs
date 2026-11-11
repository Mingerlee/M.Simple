using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace M.Simple.Extensions.Json;

/// <summary>将 long 写为字符串，读取整数或整数字符串；非法输入抛出 JsonException。</summary>
public sealed class LongToStringConverter : JsonConverter<long>
{
    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));

    /// <inheritdoc />
    public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var number)) return number;
        if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(),
            NumberStyles.Integer, CultureInfo.InvariantCulture, out var textNumber)) return textNumber;
        throw new JsonException("无法将 JSON 值转换为 long。");
    }
}

/// <summary>可空 long 转换器，另支持 null 和空字符串。</summary>
public sealed class NullableLongToStringConverter : JsonConverter<long?>
{
    private static readonly LongToStringConverter Converter = new();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value.HasValue) Converter.Write(writer, value.Value, options);
        else writer.WriteNullValue();
    }

    /// <inheritdoc />
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null ||
            reader.TokenType == JsonTokenType.String && reader.GetString() == string.Empty) return null;
        return Converter.Read(ref reader, typeof(long), options);
    }
}
