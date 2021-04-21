using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WatchIP.Email;

namespace WatchIP
{
    public class BizService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EmailClient _emailClient;

        private static string _url = "http://pv.sohu.com/cityjson";

        public BizService(IHttpClientFactory httpClientFactory, EmailClient emailClient)
        {
            _httpClientFactory = httpClientFactory;
            _emailClient = emailClient;
        }

        public async Task<bool> CheckIp()
        {
            var localIp = await GetLocalIp();

            WriteLog($"获取本地IP：{localIp?.Ip}");
            WriteLog($"获取本地IP-上次是否成功：{localIp?.GetIpSuccess}");

            var remoteIp = "";
            try
            {
                remoteIp = await GetRemoteIp();
            }
            catch (Exception e)
            {
                WriteLog(e.Message);

                //第一次报错，save and email
                if (localIp == null || localIp.GetIpSuccess)
                {
                    await SaveIp(new IpStatus() { Ip = localIp?.Ip, GetIpSuccess = false });

                    await SendEmail(localIp?.Ip, remoteIp, e.Message);
                }

                return false;
            }

            WriteLog($"获取远程IP：{remoteIp}，对比本地IP：{localIp?.Ip}，{(localIp?.Ip != remoteIp ? "有" : "无")}变化");

            if (localIp?.Ip != remoteIp)
            {
                await SaveIp(new IpStatus() { Ip = remoteIp, GetIpSuccess = true });
                await SendEmail(localIp?.Ip, remoteIp, null);
                return true;
            }

            if (localIp?.GetIpSuccess != true)
            {
                await SaveIp(new IpStatus() { Ip = localIp?.Ip, GetIpSuccess = true });
            }

            return false;
        }

        private async Task<string> GetRemoteIp()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var result = await client.GetStringAsync(_url);

                WriteLog($"{_url} 获取到结果：{result}");

                if (string.IsNullOrWhiteSpace(result) || result.IndexOf('=') <= -1)
                    throw new Exception($"返回格式不正确：{result}");

                var regex = new Regex(@"(\d+\.\d+\.\d+\.\d+)");
                var match = regex.Match(result);
                if (match.Success)
                {
                    var ip = match.Groups[0].Value;
                    WriteLog($"解析IP：{ip}");

                    return ip;
                }

                throw new Exception($"正则匹配失败：{result}");
            }
            catch (Exception e)
            {
                throw new Exception($"获取远程IP失败：{e.Message}");
            }
        }

        private async Task<IpStatus> GetLocalIp()
        {
            var file = Path.Combine(AppContext.BaseDirectory, "ip.txt");
            if (!File.Exists(file))
            {
                //File.Create(file);
                return null;
            }

            var json = await File.ReadAllTextAsync(file);
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonSerializer.Deserialize<IpStatus>(json);
        }

        private async Task SaveIp(IpStatus ipStatus)
        {
            var json = JsonSerializer.Serialize(ipStatus);

            WriteLog($"写入本地ip.txt => {json}");

            var file = Path.Combine(AppContext.BaseDirectory, "ip.txt");

            await using var stream = File.Open(file, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //获得字节数组
            var data = System.Text.Encoding.Default.GetBytes(json);
            //开始写入
            stream.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            stream.Flush();
            stream.Close();
        }

        private async Task SendEmail(string oldIp, string newIp,string error)
        {
            if (string.IsNullOrWhiteSpace(_emailClient.Config.To))
            {
                WriteLog("未配置收件人邮箱地址");
                return;
            }
            var result = await _emailClient.SendEmail(new EmailModel()
            {
                Subject = $"本地IP变化！{(string.IsNullOrWhiteSpace(error) ? "" : "错误")}",
                Body = $"{DateTime.Now} 本地IP变化，旧的IP：{oldIp}。新的IP：{newIp}。{error}",
                To = _emailClient.Config.To.Split(',').ToList()
            });

            WriteLog(result.Success ? "发送邮件：成功" : $"发送邮件：失败。{result.ErrorMessage}");
        }

        private void WriteLog(string log)
        {
            Console.WriteLine($"{DateTime.Now} {log}");
        }
    }

    public class IpStatus
    {
        public string Ip { get; set; }


        public bool GetIpSuccess { get; set; }
    }
}
