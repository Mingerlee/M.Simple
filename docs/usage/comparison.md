# 对象属性差异比较

命名空间：M.Simple.Extensions.Comparison。适用于字段修改记录、编辑前后对比等场景。

## 基本用法

```csharp
using System.ComponentModel;
using M.Simple.Extensions.Comparison;

var before = new Customer("张三", 100m);
var after = new Customer("李四", 120m);
var differences = ObjectComparison.Compare(before, after);
foreach (var diff in differences)
    Console.WriteLine($"{diff.DisplayName}: {diff.Before} → {diff.After}");

record Customer(
    [property: DisplayName("姓名")] string Name,
    [property: DisplayName("额度")] decimal Credit);
```

## 忽略字段与自定义相等判断

```csharp
var options = new ComparisonOptions();
options.IgnoredProperties.Add("UpdatedAt");
options.Comparers["Name"] = (left, right) =>
    StringComparer.OrdinalIgnoreCase.Equals(left, right);
var differences = ObjectComparison.Compare(before, after, options);
```

Compare<T>(before, after, options = null) 返回 IReadOnlyList<PropertyDifference>，每项包含 PropertyName、DisplayName、Before、After。

## 比较规则

- T 必须为引用类型，前后对象均不能为 null。属性值允许为 null。
- 比较 typeof(T) 的公共可读实例属性，包含继承属性；跳过索引器、静态属性和非公开 getter，不比较字段。
- 使用声明类型的属性范围；Compare<object> 不会自动发现运行时派生类型属性。请使用业务模型或其显式接口类型。
- 默认 object.Equals，包含声明为 object 的属性。集合和嵌套对象不会递归遍历，需要时为该属性设置比较器。
- 显示名优先 DisplayNameAttribute，其次 DescriptionAttribute，最后为属性名。
- 属性名区分大小写，结果按属性名的 Ordinal 顺序排列。忽略规则优先于自定义比较器。
- getter 和自定义比较器的异常直接传递；不静默当作“没有变化”。
- 返回的是原始属性值，不复制对象；审计落库或日志输出前应由调用方处理敏感信息。
- 反射相关行为在裁剪、NativeAOT 下尚未验证。
