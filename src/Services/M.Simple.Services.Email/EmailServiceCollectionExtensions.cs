using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace M.Simple.Services.Email
{
    /// <summary>服务注册扩展。</summary>
    public static class EmailServiceCollectionExtensions
    {
        /// <summary>
        ///     注入邮件配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="mailSettingsFunc">邮件服务器配置</param>
        /// <returns></returns>
        public static void AddEMailSetup(this IServiceCollection services, Func<IServiceProvider, MailSettings> mailSettingsFunc)
        {
            services.AddSingleton(mailSettingsFunc.Invoke);
            services.AddSingleton<IEMailService, EMailService>();
        }
    }
}
