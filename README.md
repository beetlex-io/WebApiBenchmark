# Web api benchmark
说到WebApi管理和测试工具其实已经非常多的了，Postman、Swagger等在管理和维护上都非常出色；在性能测试方面也有不少的工具如:wrk,bombardier,http_load和ab等等。不过这些工具都具有单一性，管理和维护好的在性能测试上比较低效，对于性能测试好的在管理和维护上不理想！以下主要介绍一款基于`dotnet core`开发的`WebApiBenchmarks`工具，这个工具可以对webapi进行管理和维护并提供高效的性能测试能力，接下来来先预览一下这个小工具再进行详细介绍。
#### 0.7
添加测试用例名称，优先显示名称；添加返回结果json格式化；添加导出和导入功能；

#### 0.6.8

调整测试用例树刷新问题，添加状态明细查看，修复测试后没有释放连接问题

#### 0.6

添加明细百分比显示

![](https://i.imgur.com/Di9A292.png)


## 功能
-  支持简单的服务管理，可以随时对不同服务的API进行单元和压力测试
-  支持分类的方式管理测试用例，用例支持定义GET,POST,DELETE和PUT等操作的定义
-  提供高效的性能测试支持，在4核的PC上可以达到200k rps的测试效能；
-  支持多API同时压测，并显示相关性能指标数据进行参考和对比

## 部署
工具可以运行在安装有.net core 2.1或更高版本的Linux和Windows下，工具以http服务的方式启动，通过浏览器访问进行相关操作。
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
## 端口冲突
工具默认使用9090端口，如果端口被占则无法启用服务，这个时候需要修改`HttpConfig.json`文件中的端口配置
```
    "Host": "",
    "Port": 9090,
    "SSL": false,
```
## 打开工具
工具运行后可以浏览器访问相关地址打开工具，本机访问`http://localhost:9090/`其他电脑访问`http://ipaddress:9090/`
## 添加服务地址
工具可以维护多个服务地址，所有的测试都必须选择对应的地址才能运行测试。


![](https://i.imgur.com/21RxUqG.png)

地址必须是一个可用的http服务Url
## 添加测试用例
工具支持GET,POST,DELETE和PUT请求定义，可以根据实际情况定义QueryString和Header值，并针对POST和PUT设置相应的Body内容。具体操作界面如下:


![](https://i.imgur.com/uEy21gA.png)

在编辑界面下面有个测试按钮，可以即刻测试API的调用情况；选择相应的服务地址点击测试即可在下方看到完整的返回结果：


![](https://i.imgur.com/09D4kOS.png)


## 批量单元测试
工具支持批量执行测试用例，并在测试用例上显示具体的执行结果；只要选择需要测试的用例点击单元测试即可:


![](https://i.imgur.com/dLFurrb.png)

## 性能测试
性能测试是组件提供的最重要功能，为了确保性能测试的效率；组件重写了一个轻量化的HttpClient,通过这个HttpClient即使在低配置的电脑上也可以进行高效率的压力测试。测试前需要选择相应的服务地址和单元测试用例


![](https://i.imgur.com/rfEBobs.png)

### 测试参数设置
工具提供两种测试方式，分别是基于时间和总请求数据，选择对应的测试方式设置相应的测试数值即可；用户数是指同时请求的数量，工具限制设置最大2000，设置完成后点击开始按钮即可进行测试


![](https://i.imgur.com/k7yuUeR.png)

### 测试结果
工具会实时反映测试的情况，主要包括HTTP响应状态和响应延时分布情况，如果同时压测多个API，则明细里会实时显示每个API的响应状态和响应延时情况。具体如下：

![](https://i.imgur.com/vo2iBzO.png)
![](https://i.imgur.com/EVQvfOb.png)
