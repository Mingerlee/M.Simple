using Microsoft.International.Converters.PinYinConverter;
namespace M.Simple.Extensions.Chinese
{
    /// <summary>中文拼音扩展。</summary>
    public static class ChineseStringExtensions
    {
        /// <summary>
        /// <para>M.Simple.Expand扩展</para>
        /// <para>汉字转化为拼音首字母</para>
        /// </summary>
        /// <param name="chineseStr">汉字</param>
        /// <returns>首字母</returns>
        public static string ToFirstPinyin(this string chineseStr)
        {
            string r = string.Empty;
            foreach (char obj in chineseStr)
            {
                try
                {
                    ChineseChar chineseChar = new(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }
        /// <summary>
        ///<para>M.Simple.Expand扩展</para>
        /// <para>汉字转化为拼音</para>
        /// </summary>
        /// <param name="chineseStr">汉字</param>
        /// <returns>全拼</returns>
        public static string ToPinYin(this string chineseStr)
        {
            string r = string.Empty;
            if (string.IsNullOrWhiteSpace(chineseStr))
            {
                return r;
            }
            foreach (char obj in chineseStr)
            {
                try
                {
                    ChineseChar chineseChar = new(obj);
                    string t = chineseChar.Pinyins[0].ToString();
                    r += t.Substring(0, t.Length - 1);
                }
                catch
                {
                    r += obj.ToString();
                }
            }
            return r;
        }
    }
}
