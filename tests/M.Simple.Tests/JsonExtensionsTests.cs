using M.Simple.Extensions.Json;
using System.Text.Json;

namespace M.Simple.Tests;

[TestClass]
public class JsonExtensionsTests
{
    [TestMethod]
    public void LongConvertersRoundTripTheFullRangeWithoutPrecisionLoss()
    {
        var options = new JsonSerializerOptions().AddLongAsStringConverters();
        options.AddLongAsStringConverters();
        Assert.AreEqual(2, options.Converters.Count);
        Assert.AreEqual("\"9223372036854775807\"", JsonSerializer.Serialize(long.MaxValue, options));
        Assert.AreEqual(long.MinValue, JsonSerializer.Deserialize<long>("\"-9223372036854775808\"", options));
        Assert.AreEqual(123L, JsonSerializer.Deserialize<long>("123", options));
        Assert.IsNull(JsonSerializer.Deserialize<long?>("null", options));
        Assert.IsNull(JsonSerializer.Deserialize<long?>("\"\"", options));
    }

    [TestMethod]
    public void InvalidLongValuesThrowInsteadOfSilentlyBecomingZero()
    {
        var options = new JsonSerializerOptions().AddLongAsStringConverters();
        foreach (var invalid in new[] { "\"bad\"", "\"9223372036854775808\"", "1.5", "true", "null" })
            Assert.ThrowsException<System.Text.Json.JsonException>(() => JsonSerializer.Deserialize<long>(invalid, options));
        Assert.ThrowsException<InvalidOperationException>(() => options.AddLongAsStringConverters());
    }
}
