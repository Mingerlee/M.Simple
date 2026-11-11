# 集合差异与树结构

命名空间：M.Simple.Extensions.Collections。方法会立即枚举并生成只读结果，不修改输入对象。

## 按唯一键比较集合

```csharp
using M.Simple.Extensions.Collections;

var before = new[] { new Item(1, 0, "旧名称"), new Item(2, 0, "删除") };
var after = new[] { new Item(1, 0, "新名称"), new Item(3, 1, "新增") };
var diff = before.DiffBy(after, item => item.Id);
Console.WriteLine(diff.Added.Count);   // 1
Console.WriteLine(diff.Updated.Count); // 1，包含 Before 和 After
Console.WriteLine(diff.Removed.Count); // 1

record Item(int Id, int ParentId, string Name);
```

DiffBy(before, after, keySelector, valueComparer = null, keyComparer = null) 以 after 为目标：新键为 Added、消失的键为 Removed、同键内容变化为 Updated。

- keySelector 产生唯一且非 null 的键。任一集合有重复键时抛 ArgumentException，不自动选取某条记录。
- 内容默认使用 EqualityComparer<T>.Default。record 通常按值比较，普通 class 未重写 Equals 时按引用比较；可传入 IEqualityComparer<T> 定义业务内容相等。
- keyComparer 可以指定大小写等键规则。两个比较器职责独立。
- 新增及修改按 after 顺序，删除按 before 顺序，当前实现使用 .NET 8 字典枚举顺序。
- null 集合或委托抛 ArgumentNullException，空集合正常返回空结果。

## 平铺数据转树

```csharp
using M.Simple.Extensions.Collections;

var rows = new[] { new Item(2, 1, "子部门"), new Item(1, 0, "总部") };
var tree = rows.ToTree(item => item.Id, item => item.ParentId, item => item.ParentId == 0);
Console.WriteLine(tree[0].Value.Name);             // 总部
Console.WriteLine(tree[0].Children[0].Value.Name); // 子部门

record Item(int Id, int ParentId, string Name);
```

ToTree(source, keySelector, parentKeySelector, isRoot, keyComparer = null) 返回 IReadOnlyList<TreeNode<T>>。

- 根节点由 isRoot 显式判断，不预设 0、null 或 Guid.Empty 的含义。根节点不读取 parentKeySelector。
- 键必须唯一且非 null；非根节点的父键必须存在。不支持同一节点归属多个父节点。
- 重复键、孤儿、自环及与根不连通的循环分支均抛 ArgumentException。
- 支持父节点出现在子节点之后；根节点及兄弟节点保持输入顺序。
- 迭代构造树，避免深层树的递归栈溢出。
- 返回独立包装节点，Value 仍引用原始对象；只读列表不代表原始对象不可变。
