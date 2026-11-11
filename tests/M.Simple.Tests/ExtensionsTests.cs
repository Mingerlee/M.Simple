using System.ComponentModel;
using Description = System.ComponentModel.DescriptionAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using M.Simple.Extensions.Collections;
using M.Simple.Extensions.Comparison;
using M.Simple.Extensions.Enums;
using M.Simple.Extensions.Strings;

namespace M.Simple.Tests;

[TestClass]
public class ExtensionsTests
{
    public enum Status : long { [Description("已通过")] Approved = 5000000000, Pending = 1, Alias = 1 }
    [Flags] public enum Access { Read = 1, Write = 2 }
    public sealed record Row(int Id, int ParentId, string Name);
    public sealed class Person
    {
        [DisplayName("姓名")] public string? Name { get; init; }
        public object? Value { get; init; }
        public string this[int index] => throw new InvalidOperationException();
        public string Hidden { private get; set; } = "hidden";
    }

    [TestMethod] public void EnumDescriptionPreservesLongValues()
    {
        Assert.AreEqual("已通过", Status.Approved.GetDescription());
        Assert.AreEqual(5000000000L, (long)EnumExtensions.GetOptions<Status>().Single(x => x.Value == Status.Approved).Value);
    }
    [TestMethod] public void EnumUndefinedAndFlagsHaveFallback()
    {
        Assert.AreEqual("42", ((Status)42).GetDescription());
        Assert.AreEqual("Read, Write", (Access.Read | Access.Write).GetDescription());
    }
    [TestMethod] public void EnumOptionsCollapseAliases() => Assert.AreEqual(2, EnumExtensions.GetOptions<Status>().Count);

    [DataTestMethod]
    [DataRow("13812345678", "138****5678")]
    [DataRow("1234567", "*******")]
    [DataRow("12", "**")]
    [DataRow("", "")]
    [DataRow(null, null)]
    public void PhoneMaskDoesNotExposeShortInputs(string? value, string? expected) => Assert.AreEqual(expected, value.MaskPhone());

    [DataTestMethod]
    [DataRow("alice@example.com", "a****@example.com")]
    [DataRow("a@example.com", "*@example.com")]
    [DataRow("invalid", "*******")]
    [DataRow("a@b@c", "*****")]
    [DataRow("@b", "**")]
    public void EmailMaskHandlesMalformedInputs(string value, string expected) => Assert.AreEqual(expected, value.MaskEmail());

    [TestMethod] public void CustomMaskAndOverflowAreSafe()
    {
        Assert.AreEqual("ab##ef", "abcdef".Mask(2, 2, '#'));
        Assert.AreEqual("******", "abcdef".Mask(int.MaxValue, int.MaxValue));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => "abc".Mask(-1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => ((string?)null).Mask(0, -1));
    }
    [TestMethod] public void CollectionDiffKeepsBeforeAndAfter()
    {
        var old = new[] { new Row(1, 0, "旧"), new Row(2, 0, "删除") };
        var next = new[] { new Row(1, 0, "新"), new Row(3, 0, "新增") };
        var diff = old.DiffBy(next, x => x.Id);
        Assert.AreEqual(3, diff.Added.Single().Id);
        Assert.AreEqual(2, diff.Removed.Single().Id);
        Assert.AreEqual("旧", diff.Updated.Single().Before.Name);
        Assert.AreEqual("新", diff.Updated.Single().After.Name);
    }
    [TestMethod] public void CollectionDiffRejectsDuplicateKeys()
    {
        var rows = new[] { new Row(1, 0, "a"), new Row(1, 0, "b") };
        Assert.ThrowsException<ArgumentException>(() => rows.DiffBy(Array.Empty<Row>(), x => x.Id));
        Assert.ThrowsException<ArgumentException>(() => Array.Empty<Row>().DiffBy(rows, x => x.Id));
    }
    [TestMethod] public void CollectionDiffHonorsBothComparers()
    {
        var before = new[] { "ABC" };
        var diff = before.DiffBy(new[] { "abc" }, x => x, StringComparer.OrdinalIgnoreCase, StringComparer.OrdinalIgnoreCase);
        Assert.AreEqual(0, diff.Added.Count + diff.Removed.Count + diff.Updated.Count);
    }
    [TestMethod] public void TreeHandlesUnorderedInputAndKeepsSiblingOrder()
    {
        var rows = new[] { new Row(3, 1, "b"), new Row(1, 0, "root"), new Row(2, 1, "a"), new Row(4, 0, "root2") };
        var tree = rows.ToTree(x => x.Id, x => x.ParentId, x => x.ParentId == 0);
        CollectionAssert.AreEqual(new[] { 1, 4 }, tree.Select(x => x.Value.Id).ToArray());
        CollectionAssert.AreEqual(new[] { 3, 2 }, tree[0].Children.Select(x => x.Value.Id).ToArray());
    }
    [TestMethod] public void TreeRejectsOrphans() => Assert.ThrowsException<ArgumentException>(() =>
        new[] { new Row(1, 99, "orphan") }.ToTree(x => x.Id, x => x.ParentId, x => x.ParentId == 0));
    [TestMethod] public void TreeRejectsDuplicateKeys() => Assert.ThrowsException<ArgumentException>(() =>
        new[] { new Row(1, 0, "a"), new Row(1, 0, "b") }.ToTree(x => x.Id, x => x.ParentId, x => x.ParentId == 0));
    [TestMethod] public void TreeRejectsDisconnectedCycles() => Assert.ThrowsException<ArgumentException>(() =>
        new[] { new Row(1, 0, "root"), new Row(2, 3, "a"), new Row(3, 2, "b") }
            .ToTree(x => x.Id, x => x.ParentId, x => x.ParentId == 0));
    [TestMethod] public void TreeRejectsSelfCycle() => Assert.ThrowsException<ArgumentException>(() =>
        new[] { new Row(1, 1, "self") }.ToTree(x => x.Id, x => x.ParentId, x => false));
    [TestMethod] public void TreeHandlesDeepChainsWithoutRecursion()
    {
        var tree = Enumerable.Range(1, 10000).Select(id => new Row(id, id - 1, "node"))
            .ToTree(x => x.Id, x => x.ParentId, x => x.ParentId == 0);
        var node = tree.Single();
        var count = 1;
        while (node.Children.Count > 0) { node = node.Children.Single(); count++; }
        Assert.AreEqual(10000, count);
    }
    [TestMethod] public void EmptyCollectionsWork()
    {
        Assert.AreEqual(0, Array.Empty<Row>().ToTree(x => x.Id, x => x.ParentId, x => true).Count);
        var diff = Array.Empty<Row>().DiffBy(Array.Empty<Row>(), x => x.Id);
        Assert.AreEqual(0, diff.Added.Count + diff.Removed.Count + diff.Updated.Count);
    }
    [TestMethod] public void ObjectComparisonIncludesObjectTypedPropertyAndDisplayName()
    {
        var diff = ObjectComparison.Compare(new Person { Name = "旧", Value = 1 }, new Person { Name = "新", Value = null });
        Assert.AreEqual(2, diff.Count);
        Assert.AreEqual("姓名", diff[0].DisplayName);
        Assert.AreEqual(1, diff[1].Before);
        Assert.IsNull(diff[1].After);
    }
    [TestMethod] public void ObjectComparisonSupportsIgnoreAndCustomEquality()
    {
        var options = new ComparisonOptions();
        options.IgnoredProperties.Add(nameof(Person.Value));
        options.Comparers[nameof(Person.Name)] = (a, b) => StringComparer.OrdinalIgnoreCase.Equals(a, b);
        Assert.AreEqual(0, ObjectComparison.Compare(new Person { Name = "ABC", Value = 1 },
            new Person { Name = "abc", Value = 2 }, options).Count);
    }
    [TestMethod] public void ObjectComparisonRejectsNull() => Assert.ThrowsException<ArgumentNullException>(() =>
        ObjectComparison.Compare<Person>(null!, new Person()));
    [TestMethod] public void ObjectComparisonUsesDeclaredType()
    {
        Assert.AreEqual(0, ObjectComparison.Compare<object>(new Person { Name = "a" }, new Person { Name = "b" }).Count);
    }
    [TestMethod] public void PublicInputsRejectNull()
    {
        Assert.ThrowsException<ArgumentNullException>(() => ((IEnumerable<Row>)null!).DiffBy(Array.Empty<Row>(), x => x.Id));
        Assert.ThrowsException<ArgumentNullException>(() => Array.Empty<Row>().DiffBy(null!, x => x.Id));
        Assert.ThrowsException<ArgumentNullException>(() => Array.Empty<Row>().ToTree(x => x.Id, x => x.ParentId, null!));
    }
}
