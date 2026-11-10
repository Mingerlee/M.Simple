using System.Globalization;
using M.Simple.Extensions.Core.Enums;

namespace M.Simple.Extensions.Core;

public static partial class DateTimeExtensions
{
    /// <summary>获取月初零点，与 GetFirstDayOfMonth 行为一致。</summary>
    public static DateTime BeginOfMonth(this DateTime time) => time.GetFirstDayOfMonth();

    /// <summary>获取月末 23:59:59.999，沿用毫秒精度和 Unspecified 时间类型。</summary>
    public static DateTime EndOfMonth(this DateTime time) =>
        new(time.Year, time.Month, DateTime.DaysInMonth(time.Year, time.Month), 23, 59, 59, 999);

    /// <summary>获取当天零点，时间类型为 Unspecified。</summary>
    public static DateTime BeginOfDay(this DateTime time) => new(time.Year, time.Month, time.Day);

    /// <summary>获取当天 23:59:59.999，时间类型为 Unspecified。</summary>
    public static DateTime EndOfDay(this DateTime time) =>
        new(time.Year, time.Month, time.Day, 23, 59, 59, 999);

    /// <summary>获取当天末尾；空日期仍返回 null。</summary>
    public static DateTime? EndOfDay(this DateTime? time) => time?.EndOfDay();

    /// <summary>严格按 yyyy-MM 解析月份，返回该月第一天零点，不依赖当前区域设置。</summary>
    public static DateTime ParseMonthToDateTime(this string yearMonth)
    {
        ArgumentNullException.ThrowIfNull(yearMonth);
        return DateTime.ParseExact(yearMonth, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None);
    }

    /// <summary>计算当前日期减去目标日期的月份差；可按日扣除未满一个月的部分。</summary>
    public static int DiffMonths(this DateTime date, DateTime other, bool isIncludedDay = false) =>
        date.DiffMonth(other, isIncludedDay ? DiffMonthWayEnum.精确 : DiffMonthWayEnum.简单);
}
