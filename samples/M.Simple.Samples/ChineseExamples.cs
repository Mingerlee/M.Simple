using M.Simple.Extensions.Chinese;

namespace M.Simple.Samples;

public static class ChineseExamples
{
    public static void Run()
    {
        var date = new DateTime(2024, 2, 10, 9, 30, 0);
        var pinyin = "中文".ToPinYin();
        Verify.That(!string.IsNullOrWhiteSpace(pinyin), "拼音转换结果为空");
        Console.WriteLine($"拼音：{pinyin}；首字母：{"中文".ToFirstPinyin()}");
        Console.WriteLine($"中文日期：{date.ToChineseDateTimeString(removeSecond: true)}");
        Console.WriteLine($"农历：{date.ToLunarCalendarDate()}");
    }
}
