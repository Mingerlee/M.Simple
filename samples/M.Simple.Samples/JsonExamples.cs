using System.Text.Json;
using M.Simple.Extensions.Json;

namespace M.Simple.Samples;

public static class JsonExamples
{
    public static void Run()
    {
        var options = new JsonSerializerOptions().AddLongAsStringConverters();
        var json = JsonSerializer.Serialize(new { Uid = long.MaxValue, ParentUid = (long?)null }, options);
        Verify.That(json.Contains("\"9223372036854775807\""), "长整数未输出为字符串");
        Verify.That(JsonSerializer.Deserialize<long>("\"9223372036854775807\"", options) == long.MaxValue, "长整数读取失败");
        Console.WriteLine(json);
    }
}
