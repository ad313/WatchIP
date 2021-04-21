using System;
using System.Collections.Generic;
using System.Linq;

namespace WatchIP.Email
{
    /// <summary>
    /// 邮件发送消息模型
    /// </summary>
    public class EmailModel
    {
        /// <summary>
        /// 邮件标题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 正文内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 标识正文内容是否是html，默认 false
        /// </summary>
        public bool IsHtml { get; set; }
        
        /// <summary>
        /// 抄送
        /// </summary>
        public List<string> Cc { get; set; }

        /// <summary>
        /// 邮件接收人
        /// </summary>
        public List<string> To { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public EmailModel(){ }

        /// <summary>
        /// 数据校验
        /// </summary>
        public void Check()
        {
            if (string.IsNullOrWhiteSpace(Subject))
                throw new ArgumentNullException(nameof(Subject));

            if (string.IsNullOrWhiteSpace(Body))
                throw new ArgumentNullException(nameof(Body));

            if (To == null || !To.Any())
                throw new ArgumentNullException(nameof(To));
        }

        /// <summary>
        /// 数据校验
        /// </summary>
        public bool CheckValid()
        {
            if (string.IsNullOrWhiteSpace(Subject))
                return false;

            if (string.IsNullOrWhiteSpace(Body))
                return false;

            if (To == null || !To.Any())
                return false;
            
            return true;
        }
    }
}
