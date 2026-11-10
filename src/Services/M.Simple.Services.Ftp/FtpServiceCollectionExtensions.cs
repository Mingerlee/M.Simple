using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace M.Simple.Services.Ftp
{
    /// <summary>服务注册扩展。</summary>
    public static class FtpServiceCollectionExtensions
    {
        /// <summary>
        ///     注入ftp配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="ftpClientSettingsFunc">ftp配置</param>
        /// <returns></returns>
        public static void AddFtpClientSetup(this IServiceCollection services, Func<IServiceProvider, FtpClientSettings> ftpClientSettingsFunc)
        {
            services.AddSingleton(ftpClientSettingsFunc.Invoke);
            services.AddSingleton<IFtpClientService, FtpClientService>();
        }
    }
}
