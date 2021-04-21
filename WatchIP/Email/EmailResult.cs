using System;

namespace WatchIP.Email
{
    /// <summary>
    /// 发送邮件结果
    /// </summary>
    public class EmailResult
    {
        /// <summary>
        /// 消息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; private set; }

        public EmailResult()
        {
            Success = true;
        }

        public EmailResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Success = false;
        }

        public EmailResult(string errorMessage, Exception exception)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
            Success = false;
        }
    }
}
