using M.Simple.Samples;

var scenarios = new Dictionary<string, Func<Task>>(StringComparer.OrdinalIgnoreCase)
{
    ["core"] = () => { CoreExamples.Run(); return Task.CompletedTask; },
    ["chinese"] = () => { ChineseExamples.Run(); return Task.CompletedTask; },
    ["datatable"] = () => { DataTableExamples.Run(); return Task.CompletedTask; },
    ["cryptography"] = () => { CryptographyExamples.Run(); return Task.CompletedTask; },
    ["configuration"] = () => { ConfigurationExamples.Run(); return Task.CompletedTask; },
    ["expressions"] = () => { ExpressionExamples.Run(); return Task.CompletedTask; },
    ["database"] = DatabaseExamples.RunAsync,
    ["json"] = () => { JsonExamples.Run(); return Task.CompletedTask; },
    ["di"] = () => { DependencyInjectionExamples.Run(); return Task.CompletedTask; },
    ["email"] = () => { EmailExamples.Run(); return Task.CompletedTask; },
    ["ftp"] = () => { FtpExamples.Run(); return Task.CompletedTask; }
};

var selected = args.Length == 0 ? "all" : args[0];
if (selected != "all" && !scenarios.ContainsKey(selected))
{
    Console.Error.WriteLine("可用示例：all, " + string.Join(", ", scenarios.Keys));
    return 2;
}

foreach (var scenario in scenarios.Where(item => selected == "all" || item.Key.Equals(selected, StringComparison.OrdinalIgnoreCase)))
{
    Console.WriteLine($"[{scenario.Key}]");
    await scenario.Value();
}
Console.WriteLine("示例执行完成。");
return 0;
