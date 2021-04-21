using System;

namespace WatchIP.Email
{
    public class EmailConfig
    {
        /// <summary>
        /// 是否禁用OAuth
        /// </summary>
        public bool DisableOAuth { get; set; }
        /// <summary>
        /// 显示的名字
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 邮件主机地址
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 连接池大小
        /// </summary>
        public int ConnectionPoolSize { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string To { get; set; }

        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Host))
                throw new ArgumentNullException(nameof(Host));

            if (string.IsNullOrWhiteSpace(Password))
                throw new ArgumentNullException(nameof(Password));

            if (string.IsNullOrWhiteSpace(Account))
                throw new ArgumentNullException(nameof(Account));

            if (Port <= 0)
                throw new ArgumentNullException(nameof(Port));
        }
    }
}
