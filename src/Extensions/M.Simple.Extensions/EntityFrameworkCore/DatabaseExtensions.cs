using System.Dynamic;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace M.Simple.Extensions.EntityFrameworkCore;

/// <summary>通过 EF Core 当前连接执行参数化 Dapper 查询；连接和事务由宿主管理。</summary>
public static class DatabaseExtensions
{
    /// <summary>查询强类型记录，自动携带当前事务、命令超时和取消令牌。</summary>
    public static Task<IEnumerable<T>> QueryAsync<T>(this DatabaseFacade database, string sql, object? param = null,
        int? commandTimeout = null, CancellationToken cancellationToken = default) =>
        database.GetDbConnection().QueryAsync<T>(CreateCommand(database, sql, param, commandTimeout, cancellationToken));

    /// <summary>查询动态记录，自动携带当前事务、命令超时和取消令牌。</summary>
    public static Task<IEnumerable<dynamic>> QueryAsync(this DatabaseFacade database, string sql, object? param = null,
        int? commandTimeout = null, CancellationToken cancellationToken = default) =>
        database.GetDbConnection().QueryAsync(CreateCommand(database, sql, param, commandTimeout, cancellationToken));

    /// <summary>查询首条强类型记录，无记录时返回该类型默认值。</summary>
    public static Task<T?> QueryFirstOrDefaultAsync<T>(this DatabaseFacade database, string sql, object? param = null,
        int? commandTimeout = null, CancellationToken cancellationToken = default) =>
        database.GetDbConnection().QueryFirstOrDefaultAsync<T>(CreateCommand(database, sql, param, commandTimeout, cancellationToken));

    /// <summary>查询首条动态记录，无记录时返回 null；直接复制列值，保留驱动返回的类型。</summary>
    public static async Task<ExpandoObject?> QueryFirstOrDefaultAsync(this DatabaseFacade database, string sql, object? param = null,
        int? commandTimeout = null, CancellationToken cancellationToken = default)
    {
        object? row = await database.GetDbConnection().QueryFirstOrDefaultAsync(
            CreateCommand(database, sql, param, commandTimeout, cancellationToken));
        if (row == null) return null;
        // 无需经 JSON 序列化中转，避免数字精度或日期类型发生变化。
        var result = new ExpandoObject();
        foreach (var column in (IDictionary<string, object?>)row)
            ((IDictionary<string, object?>)result)[column.Key] = column.Value;
        return result;
    }

    private static CommandDefinition CreateCommand(DatabaseFacade database, string sql, object? param,
        int? commandTimeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentException.ThrowIfNullOrWhiteSpace(sql);
        // 仅参与宿主的当前事务，不创建、提交或释放宿主资源。
        return new CommandDefinition(sql, param, database.CurrentTransaction?.GetDbTransaction(),
            commandTimeout: commandTimeout ?? database.GetCommandTimeout(), cancellationToken: cancellationToken);
    }
}
