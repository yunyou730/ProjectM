# ProjectM  

## 集成 Lua   

编写一个 C++ 库,用于给 Unity 做 Lua 绑定  

Lua 用于充当配置文件   
不必再去解析 json , 并且可以尝试做到 不重启游戏随时修改  

## 六边形 TileMap  

可以显示了

## LockStep   

Server 创建主机   
Client 进入主机
Server 点击开始游戏  
Client 加载成功给服务器通知   
Server 开始游戏，开启 LockStep 机制 

 - 需要增加  lobby ,game 的判断，判断是否在游戏中  **done**  
 - 需要增加 加入游戏，开始游戏的按钮  **done**  
 - 需要增加加载地图的流程   **done**
 - 加入主机时 需要有 是否在 lobby 中的判断。如果游戏已经开始，则不能加入  **done**   

LockStep 机制基本完成. 后续计划 


 - 将操作移动机制从 逐格移动,改为连续移动 
 - 抽象网络相关的代码,做到和 gameplay 耦合度尽可能小  
 - 增加 lobby 界面,用于显示 当前网络内可见的主机 ,client 加入 server 的流程界面化
 - server 建立主机 增加局域网广播机制  
 - 客户端如何看到现有的 server ?
 - Lobby 界面化,用于显示 当前所有客户端  
 - 增加 Client 掉线 的 处理 和 显示  



客户端如何看到现有的 server ?  
	

 >很简单，IP地址当中有一种地址叫做广播地址，包括全局广播地址（255.255.255.255)和子网广播地址（一般为子网最后一个地址）两种。向这个地址发udp包则所有连接在同一个路由器上的或者该子网当中的任意IP地址都可以收到这个包。  


[http://www.cs.ubbcluj.ro/~dadi/compnet/labs/lab3/udp-broadcast.html](http://www.cs.ubbcluj.ro/~dadi/compnet/labs/lab3/udp-broadcast.html)  

尝试方案: 

lobby 阶段的服务器,每帧往 255.255.255.255 地址 发送  UDP 消息,来解决 客户端显示所有现有主机的问题   







 
 


