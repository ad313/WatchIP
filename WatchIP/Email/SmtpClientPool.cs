using MailKit.Net.Smtp;
using Microsoft.Extensions.ObjectPool;

namespace WatchIP.Email
{
    public class SmtpClientPool : IPooledObjectPolicy<SmtpClient>
    {
        private readonly EmailConfig _config;

        public SmtpClientPool(EmailConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Create a SmtpClient
        /// </summary>
        /// <returns></returns>
        public SmtpClient Create()
        {
            var client = new SmtpClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };

            if (_config.DisableOAuth)
            {
                client.AuthenticationMechanisms.Remove("XOAUTH2");
            }

            return client;
        }

        /// <summary>
        /// Return SmtpClient
        /// </summary>
        /// <param name="client">The object to return to the pool.</param>
        /// <returns>SmtpClient</returns>
        public bool Return(SmtpClient client)
        {
            client.Disconnect(false);
            return true;
        }
    }
}
