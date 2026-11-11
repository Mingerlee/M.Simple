namespace M.Simple.Extensions.Collections;

/// <summary>平铺数据生成的树节点，不修改原始对象。</summary>
public sealed record TreeNode<T>(T Value, IReadOnlyList<TreeNode<T>> Children);

/// <summary>同键元素的变更，保留修改前和修改后对象。</summary>
public sealed record ItemChange<T>(T Before, T After);

/// <summary>以新集合为目标的新增、修改、删除结果。</summary>
public sealed record CollectionDifference<T>(
    IReadOnlyList<T> Added,
    IReadOnlyList<ItemChange<T>> Updated,
    IReadOnlyList<T> Removed);

/// <summary>集合差异与树结构扩展。</summary>
public static class CollectionExtensions
{
    /// <summary>按唯一键比较两个集合；重复键抛出异常。默认按 EqualityComparer 比较内容。</summary>
    public static CollectionDifference<T> DiffBy<T, TKey>(this IEnumerable<T> before, IEnumerable<T> after,
        Func<T, TKey> keySelector, IEqualityComparer<T>? valueComparer = null,
        IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);
        ArgumentNullException.ThrowIfNull(keySelector);
        var oldItems = before.ToDictionary(keySelector, keyComparer);
        var newItems = after.ToDictionary(keySelector, keyComparer);
        valueComparer ??= EqualityComparer<T>.Default;
        var added = new List<T>();
        var updated = new List<ItemChange<T>>();
        var removed = new List<T>();
        foreach (var (key, value) in newItems)
        {
            if (!oldItems.TryGetValue(key, out var old)) added.Add(value);
            else if (!valueComparer.Equals(old, value)) updated.Add(new ItemChange<T>(old, value));
        }
        foreach (var (key, value) in oldItems)
            if (!newItems.ContainsKey(key)) removed.Add(value);
        return new CollectionDifference<T>(added.AsReadOnly(), updated.AsReadOnly(), removed.AsReadOnly());
    }

    /// <summary>按显式根条件转树；重复键、孤儿及任意分支环路抛出异常。保留输入的兄弟顺序。</summary>
    public static IReadOnlyList<TreeNode<T>> ToTree<T, TKey>(this IEnumerable<T> source,
        Func<T, TKey> keySelector, Func<T, TKey> parentKeySelector, Func<T, bool> isRoot,
        IEqualityComparer<TKey>? keyComparer = null) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(parentKeySelector);
        ArgumentNullException.ThrowIfNull(isRoot);
        var items = source.ToDictionary(keySelector, keyComparer);
        var roots = new List<TKey>();
        var parents = new Dictionary<TKey, TKey>(keyComparer);
        var children = new Dictionary<TKey, List<TKey>>(keyComparer);
        foreach (var (key, value) in items)
        {
            if (isRoot(value)) roots.Add(key);
            else
            {
                var parent = parentKeySelector(value);
                if (!items.ContainsKey(parent)) throw new ArgumentException("存在找不到父节点的数据。", nameof(source));
                parents.Add(key, parent);
                if (!children.TryGetValue(parent, out var siblings)) children[parent] = siblings = new List<TKey>();
                siblings.Add(key);
            }
        }
        // 检查所有节点，避免无根的循环分支被静默丢弃。
        var verified = new HashSet<TKey>(keyComparer);
        foreach (var key in items.Keys)
        {
            var path = new HashSet<TKey>(keyComparer);
            var current = key;
            while (!verified.Contains(current))
            {
                if (!path.Add(current)) throw new ArgumentException("树数据存在循环引用。", nameof(source));
                if (!parents.TryGetValue(current, out var parent)) break;
                current = parent;
            }
            verified.UnionWith(path);
        }
        // 迭代构造，避免深层树触发递归栈溢出。
        var built = new Dictionary<TKey, TreeNode<T>>(keyComparer);
        var pending = new Stack<(TKey Key, bool Visited)>();
        foreach (var key in roots) pending.Push((key, false));
        while (pending.TryPop(out var entry))
        {
            children.TryGetValue(entry.Key, out var childKeys);
            if (!entry.Visited)
            {
                pending.Push((entry.Key, true));
                if (childKeys is not null)
                    foreach (var child in childKeys) pending.Push((child, false));
            }
            else
            {
                var nodes = childKeys?.Select(key => built[key]).ToArray() ?? Array.Empty<TreeNode<T>>();
                built.Add(entry.Key, new TreeNode<T>(items[entry.Key], Array.AsReadOnly(nodes)));
            }
        }
        return Array.AsReadOnly(roots.Select(key => built[key]).ToArray());
    }
}
