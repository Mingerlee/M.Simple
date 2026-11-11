using System;

namespace M.Simple.Extensions.Core.Attributes
{
    /// <summary>
    /// <para>M.Simple.Extensions.Core 扩展</para>
    /// <para>需要对比的属性</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor, AllowMultiple = true, Inherited = false)]
    public class CompareValueAttribute : Attribute
    {

    }
}
