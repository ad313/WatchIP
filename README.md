# WatchIP
Monitor changes in the external network ip。监控外网IP变化，并发邮件通知。
使用场景：有公网IP，但是会变化。此时可以邮件通知。或者使用DDNS。

## 默认配置文件
```
{
  "Email": {
    "DisableOAuth": true,
    "DisplayName": "WatchIP",
    "Host": "smtp.exmail.qq.com",
    "Account": "你的服务邮箱账号",
    "Password": "你的服务邮箱密码",
    "Port": 25,
    "ConnectionPoolSize": 1,
    "Enable": true,
    "To": "接收人邮箱地址"
  },
  "Delay": 600
}
```

## docker
```
docker run -d  -m 128M --name=watchip -e Email:Account=你的服务邮箱账号 -e Email:Password=你的服务邮箱密码 -e Email:To=接收人邮箱地址 -e Delay=定时间隔秒数 --restart=always  ad313/watchip:1.0
```
