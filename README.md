# WatchIP
Monitor changes in the external network ip。监控外网IP变化，并发邮件通知。

# docker
```
docker run -d  -m 128M --name=watchip -e Email:Account=你的服务邮箱账号 -e Email:Password=你的服务邮箱密码 -e Email:To=接收人邮箱地址 -e Delay=定时间隔秒数 --restart=always  ad313/watchip:1.0
```
