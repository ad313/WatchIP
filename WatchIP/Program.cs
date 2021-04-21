using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Threading.Tasks;
using WatchIP.Email;

namespace WatchIP
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private static BizService _bizService;
        private static IConfigurationRoot _configurationRoot;

        static async Task Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var hostBuilder = new HostBuilder().ConfigureAppConfiguration((hostContext, config) =>
            {
                _configurationRoot = config.AddJsonFile("config/config.json").AddEnvironmentVariables().Build();
            }).ConfigureServices(async (hostContext, services) =>
            {
                services.AddHttpClient();
                services.AddSingleton<BizService>();
                services.AddSingleton<EmailClient>();
                services.Configure<EmailConfig>(_configurationRoot.GetSection("Email"));

                _serviceProvider = services.BuildServiceProvider();

                _bizService = _serviceProvider.GetRequiredService<BizService>();

                var delay = _configurationRoot.GetValue<int>("Delay");
                delay = delay > 0 ? delay : 600;

                while (true)
                {
                    if (DateTime.Now.Hour < 0 || DateTime.Now.Hour >= 8)
                        await _bizService.CheckIp();

                    var delayTime = DateTime.Now.AddSeconds(delay);

                    Console.WriteLine($"{DateTime.Now} 下次执行时间：{delayTime.ToString("yyyy-M-dd HH:mm:ss")}");
                    Console.WriteLine($"{DateTime.Now} 执行间隔：{delay} 秒");

                    await Task.Delay(delay * 1000);
                }
            });

            //初始化通用主机
            var host = hostBuilder.Build();
            await host.StartAsync();
        }
    }
}