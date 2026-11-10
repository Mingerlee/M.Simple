using System.Text;
using M.Simple.Extensions.Chinese.Helpers;
namespace M.Simple.Extensions.Chinese
{
    /// <summary>中文日期与农历扩展。</summary>
    public static class ChineseDateTimeExtensions
    {
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>日期格式："yyyy年MM月dd日"</para>
        /// </summary>
        /// <param name="dateTime">日期</param>
        public static string ToChineseDateString(this DateTime dateTime)
        {
            return string.Format("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>日期格式："yyyy年MM月dd日"</para>
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <returns>如果为null返回string.Empty</returns>
        public static string ToChineseDateString(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return string.Format("{0}年{1}月{2}日", dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
            }
            else
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>日期格式："yyyy年MM月dd日"</para>
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="defaultDateTime">默认日期</param>
        /// <returns>如果为null返回默认日期</returns>
        public static string ToChineseDateString(this DateTime? dateTime, DateTime defaultDateTime)
        {
            if (dateTime.HasValue)
            {
                return string.Format("{0}年{1}月{2}日", dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
            }
            else
            {
                return string.Format("{0}年{1}月{2}日", defaultDateTime.Year, defaultDateTime.Month, defaultDateTime.Day);
            }
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>日期时间格式："yyyy年MM月dd日 HH时mm分"</para>
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        /// <returns></returns>
        public static string ToChineseDateTimeString(this DateTime dateTime, bool removeSecond = false)
        {
            StringBuilder result = new StringBuilder();
            result.AppendFormat("{0}年{1}月{2}日", dateTime.Year, dateTime.Month, dateTime.Day);
            result.AppendFormat(" {0}时{1}分", dateTime.Hour, dateTime.Minute);
            if (removeSecond == false)
                result.AppendFormat("{0}秒", dateTime.Second);
            return result.ToString();
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>日期时间格式："yyyy年MM月dd日 HH时mm分"</para>
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        /// <returns>如果为null返回string.Empty</returns>
        public static string ToChineseDateTimeString(this DateTime? dateTime, bool removeSecond = false)
        {
            if (dateTime == null)
                return string.Empty;
            return ToChineseDateTimeString(dateTime.Value, removeSecond);
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>日期时间格式："yyyy年MM月dd日 HH时mm分"</para>
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="defaultDateTime">默认日期</param>
        /// <param name="removeSecond">是否移除秒</param>
        /// <returns>如果为null返回默认日期</returns>
        public static string ToChineseDateTimeString(this DateTime? dateTime, DateTime defaultDateTime, bool removeSecond = false)
        {
            if (dateTime == null)
                return ToChineseDateTimeString(defaultDateTime, removeSecond);
            else
                return ToChineseDateTimeString(dateTime.Value, removeSecond);
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>转换为农历日期</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToLunarCalendarDate(this DateTime dateTime)
        {
            ChineseCalendarHelper chineseCalendar = new(dateTime);
            return chineseCalendar.ChineseDateString;
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>转换为农历日期</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToLunarCalendarDate(this DateTime? dateTime)
        {
            dateTime ??= DateTime.Now;

            ChineseCalendarHelper chineseCalendar = new(dateTime.Value);
            return chineseCalendar.ChineseDateString;
        }
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>转换为农历日期</para>
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="defaultDateTime">默认日期</param>
        /// <returns></returns>
        public static string ToLunarCalendarDate(this DateTime? dateTime, DateTime defaultDateTime)
        {
            dateTime ??= defaultDateTime;

            ChineseCalendarHelper chineseCalendar = new(dateTime.Value);
            return chineseCalendar.ChineseDateString;
        }
    }
}
