using System.Text.Json;

namespace M.Simple.Extensions.Json;

/// <summary>JSON 序列化选项扩展。</summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>注册 long 与 long? 字符串转换器；重复调用不重复添加。</summary>
    public static JsonSerializerOptions AddLongAsStringConverters(this JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        if (options.IsReadOnly) throw new InvalidOperationException("序列化选项已只读，无法注册转换器。");
        if (!options.Converters.OfType<LongToStringConverter>().Any()) options.Converters.Add(new LongToStringConverter());
        if (!options.Converters.OfType<NullableLongToStringConverter>().Any()) options.Converters.Add(new NullableLongToStringConverter());
        return options;
    }
}
