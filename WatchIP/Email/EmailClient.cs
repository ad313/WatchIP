using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WatchIP.Email
{
    public class EmailClient
    {
        private readonly ILogger<EmailClient> _logger;
        public readonly EmailConfig Config;
        private static DefaultObjectPool<SmtpClient> ConnectionPool { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public EmailClient(IOptionsMonitor<EmailConfig> config, ILogger<EmailClient> logger)
        {
            _logger = logger;
            Config = config.CurrentValue;

            if (Config == null)
                throw new ArgumentNullException(nameof(Config));

            Config.Check();
            
            if (Config.ConnectionPoolSize <= 0)
                Config.ConnectionPoolSize = 5;

            var policy = new SmtpClientPool(Config);

            ConnectionPool = new DefaultObjectPool<SmtpClient>(policy, Config.ConnectionPoolSize >= 0 ? Config.ConnectionPoolSize : 5);

            logger.LogInformation("邮件服务初始化成功，连接池大小 {0}", Config.ConnectionPoolSize);
        }

        /// <summary>
        /// 直接调用发邮件
        /// </summary>
        /// <param name="mailBoxModel"></param>
        /// <returns></returns>
        public async Task<EmailResult> SendEmail(EmailModel mailBoxModel)
        {
            if (mailBoxModel == null)
                return new EmailResult("mailBoxModel 不能为空", new ArgumentNullException(nameof(mailBoxModel)));

            (MimeMessage message, Exception ex) convert = ConvertMessage(mailBoxModel);

            if (convert.ex != null)
                return new EmailResult("MailBoxModel 模型转换失败", convert.ex);

            return await SendMail(convert.message);
        }

        /// <summary>
        /// 调用api发邮件
        /// </summary>
        /// <param name="mimeMessage"></param>
        /// <returns></returns>
        private async Task<EmailResult> SendMail(MimeMessage mimeMessage)
        {
            if (mimeMessage == null)
                return new EmailResult("mimeMessage 不能为空", new ArgumentNullException(nameof(mimeMessage)));

            var client = ConnectionPool.Get();

            try
            {
                await client.ConnectAsync(Config.Host, Config.Port, false);

                if (!client.IsAuthenticated)
                    await client.AuthenticateAsync(Config.Account, Config.Password);

                await client.SendAsync(mimeMessage);

                return new EmailResult();
            }
            catch (Exception ex)
            {
                return new EmailResult(ex.Message, ex);
            }
            finally
            {
                ConnectionPool.Return(client);
            }
        }

        /// <summary>
        /// message 模型转换
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private (MimeMessage, Exception) ConvertMessage(EmailModel box)
        {
            try
            {
                var from = InternetAddress.Parse(Config.Account);
                from.Name = Config.DisplayName;

                var message = new MimeMessage { Subject = box.Subject };
                message.From.Add(from);
                message.To.AddRange(box.To.Select(MailboxAddress.Parse));

                if (box.Cc != null && box.Cc.Any())
                    message.Cc.AddRange(box.Cc.Select(MailboxAddress.Parse));

                var builder = new BodyBuilder();

                if (box.IsHtml)
                    builder.HtmlBody = box.Body;
                else
                    builder.TextBody = box.Body;
                
                message.Body = builder.ToMessageBody();
                return (message, null);
            }
            catch (Exception e)
            {
                return (null, e);
            }
        }
    }
}