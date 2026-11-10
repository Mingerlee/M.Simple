using System.Globalization;
using System.Reflection;

namespace M.Simple.Tests;

[TestClass]
public class BamsCoreExtensionsTests
{
    [TestMethod]
    public void DateBoundariesHandleLeapYearAndNullableDates()
    {
        var date = new DateTime(2024, 2, 15, 12, 30, 0, DateTimeKind.Utc);
        Assert.AreEqual(new DateTime(2024, 2, 1), date.BeginOfMonth());
        Assert.AreEqual(new DateTime(2024, 2, 29, 23, 59, 59, 999), date.EndOfMonth());
        Assert.AreEqual(new DateTime(2024, 2, 15), date.BeginOfDay());
        Assert.AreEqual(new DateTime(2024, 2, 15, 23, 59, 59, 999), date.EndOfDay());
        Assert.AreEqual(DateTimeKind.Unspecified, date.EndOfDay().Kind);
        Assert.AreEqual(date.EndOfDay(), ((DateTime?)date).EndOfDay());
        Assert.IsNull(((DateTime?)null).EndOfDay());
    }

    [TestMethod]
    public void MonthParsingDoesNotDependOnCurrentCulture()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
            Assert.AreEqual(new DateTime(2024, 2, 1), "2024-02".ParseMonthToDateTime());
            Assert.ThrowsException<FormatException>(() => "2024-13".ParseMonthToDateTime());
            Assert.ThrowsException<FormatException>(() => "2024/02".ParseMonthToDateTime());
        }
        finally { CultureInfo.CurrentCulture = original; }
    }

    [TestMethod]
    public void MonthDifferencesRetainBamsDirectionAndDayRule()
    {
        var earlier = new DateTime(2024, 1, 31);
        var later = new DateTime(2024, 2, 1);
        Assert.AreEqual(1, later.DiffMonths(earlier));
        Assert.AreEqual(0, later.DiffMonths(earlier, true));
        Assert.AreEqual(-1, earlier.DiffMonths(later));
    }

    [TestMethod]
    public void MatchFormattingHandlesBothChineseQuotesAndKeepsBlankInput()
    {
        Assert.AreEqual("(\"Ab C\")", " （“Ab C”） ".ToFormatMatch());
        Assert.AreEqual("  ", "  ".ToFormatMatch());
        Assert.IsNull(((string)null).ToFormatMatch());
        Assert.IsFalse(((string)null).HasValue());
        Assert.IsFalse(" \t".HasValue());
        Assert.IsTrue("0".HasValue());
    }

    [TestMethod]
    public void PropertyDictionarySkipsIndexersAndNonPublicGetters()
    {
        var model = new PropertyProbe { Name = "样例" };
        var values = model.ToDictionary();
        Assert.AreEqual("样例", values["name"]);
        Assert.IsNull(values["optional"]);
        Assert.IsFalse(values.ContainsKey("item"));
        Assert.IsFalse(values.ContainsKey("writeonly"));
        Assert.AreEqual("样例", model.GetObjectPropertyValue("NAME"));
        Assert.IsTrue(model.ToDictionary(false).ContainsKey("Name"));
        Assert.ThrowsException<KeyNotFoundException>(() => model.GetObjectPropertyValue("missing"));
    }

    [TestMethod]
    public void PropertyNamesKeepOriginalCaseAndRejectNullType()
    {
        var names = typeof(PropertyProbe).GetPublicPropertyNames();
        Assert.IsTrue(names.Contains("Name"));
        Assert.IsFalse(names.Contains("name"));
        Assert.ThrowsException<ArgumentNullException>(() => ((Type)null).GetPublicPropertyNames());
    }

    public class PropertyProbe
    {
        public string Name { get; set; }
        public string Optional { get; set; }
        public string WriteOnly { private get; set; }
        public string this[int index] => throw new InvalidOperationException("不应读取索引器");
    }
}
