# BAMS 通用方法对照

本次只向 M.Simple 增加通用 API，不替换 BAMS 中的调用。接入方须核对命名空间及以下行为差异。

| BAMS 源码位置 | M.Simple 归属 | 方法与边界 |
|---|---|---|
| BAMS.Common/CommonExtend.cs | Core / DateTimeExtensions | BeginOfMonth、EndOfMonth、BeginOfDay、EndOfDay、DiffMonths；月初和月份差复用包内已有实现 |
| BAMS.Common/CommonExtend.cs | Core / DateTimeExtensions | ParseMonthToDateTime 严格接受 yyyy-MM，使用不变区域设置；旧方法先用当前区域解析 |
| BAMS.Common/CommonExtend.cs | Core / StringExtensions | HasValue、ToFormatMatch；匹配格式化同时处理左右中文双引号，补齐旧方法遗漏的右引号 |
| BAMS.Common/ReflectionExtensions.cs | Core / ReflectionExtensions | GetPublicPropertyNames，保留属性原名和区分大小写的集合 |
| BAMS.Common/ObjectExtends.cs | Core / ReflectionExtensions | ToDictionary、GetObjectPropertyValue；默认键使用不变区域小写，跳过索引器与非公共 getter，取值时属性名忽略大小写；可选保留原属性名 |
| BAMS.Repository/PurSuppVisitRepository.cs；IQueryableExtension/PageExtend.cs | Expressions | And、Or 采用参数替换与短路求值；补齐强类型 In、True、False，不复制原 In 的类型构造错误 |
| IQueryableExtension/DatabaseExtends.cs | EntityFrameworkCore | 强类型/动态列表和首条查询；透传当前事务、超时、取消令牌；动态首条不经 JSON 中转 |
| IQueryableExtension/PageExtend.cs | Json | long 与 long? 严格转换器；新增幂等注册扩展；非法输入抛异常，不采用 Program.cs 中静默返回 0 的策略 |

日期边界沿用 BAMS 的 `23:59:59.999` 毫秒精度与 `DateTimeKind.Unspecified`，并非该日最后一个 tick；GetLastDayOfMonth 仍返回月末零点，与 EndOfMonth 用途不同。用于 SQL 时间范围时，接入方按自身精度选择结束条件。

本次不纳入：业务固定日期 Forever、行政区域规则、唯一性数据库校验、业务编码、工作日历、动态页面模型及权限、审批、库存逻辑。LocalAsyncLock 属于服务能力且需要另行完善生命周期，MySqlParameterHelper 的只读属性处理有缺陷，暂不原样移入公共包。

新增功能独立测试，不改变旧包迁入方法的既有行为。数据库验证限于 SQLite，实际 MySQL、Doris 和业务 API 接入未验证。

## 验证结果

- Release 构建随测试完成，47 项测试通过（原 34 项，新增 13 项）。
- 新测试覆盖闰年月末、可空日期、区域无关月份解析、双引号归一化、索引属性、短路表达式、EF 查询翻译、事务回滚、参数绑定、取消以及 long 极值和非法 JSON 输入。
- 统一为 4 个 NuGet 包（1 个扩展包、3 个服务包）；原有源码的编译警告仍保留。
