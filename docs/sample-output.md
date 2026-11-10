# 示例运行输出

在仓库根目录运行 `dotnet run --project samples/M.Simple.Samples --configuration Release -- all`。以下为一次已验证输出；显示格式可能随当前区域设置改变。

```text
[core]
匹配文本：("样例")；月末：2024-02-29 23:59:59.999
分批：2,1；金额：12.35
变化字段：Name,Price
枚举：待处理；属性：原名称
[chinese]
拼音：ZHONGWEN；首字母：ZW
中文日期：2024年2月10日 9时30分
农历：农历二零二四年正月初一
[datatable]
源表 2 行；筛选后 1 行：产品 B
[cryptography]
AES、RSA 往返成功；未输出密钥。
[configuration]
配置：M.Simple 使用示例
[expressions]
符合条件的产品：1
[database]
事务内查询：事务内产品
回滚后记录数：0
[json]
{"Uid":"9223372036854775807","ParentUid":null}
[di]
你好，M.Simple
[email]
邮件服务注册成功；此示例不发送邮件。
[ftp]
FTP 服务注册成功；此示例不建立 FTP 连接。
示例执行完成。
```
