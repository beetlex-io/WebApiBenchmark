# Web api benchmark
## 运行环境
安装有`dotnet core 2.1`更高版本的linux或windows
## 运行
- linux
```
dotnet BeetleX.WebApiBenchmarks.dll
```
or
```
./webapibenchmark.sh
```
 - windows
 ```
 dotnet BeetleX.WebApiBenchmarks.dll
 ```
 or
 ```
 webapibenchmark.bat
 ```
 
 ## 打开工具
 运行工具后会打开一个`http`服务默认服务端口是`9090`,打开浏览器本机访问`http://localhost:9090`远程访问`http://ipaddress:9090`；开始界面如下:
 ![](https://i.imgur.com/EJzPZNE.png)
 ## 9090端口被占用怎办
 修改`HttpConfig.json`文件，配置本机可用的服务端口
 ```
    "Port": 9090,
 ```
 修改后重新运行即可
 
 ## 创建测试用例
 ![](https://i.imgur.com/WbYbJkt.png)
 ## 测试用例
 ![](https://i.imgur.com/vrnTYKX.png)
 选择相应的服务器，点击测试即可查看测试结果
 
 ## 运行所有测试用例
 选择需要测试的用例，然后点击单元测试按钮，工具会运行相关用例并显示对应的执行结果
 ![](https://i.imgur.com/4l1brtE.png)
 ## 性能测试
 选择需要测试的用例，然后点击压力测试按钮，并设置相关压测参数点击开始按钮即可以进行压测
 ![](https://i.imgur.com/uQqBWId.png)
 ## 运行结果
 工具可以实时反映请求状态结果和请求响应延时的分布情况，明细还显示每个api接口的执行情况。
 ![](https://i.imgur.com/odPyDvd.png)
