# ProjectM  

## 集成 Lua   

编写一个 C++ 库,用于给 Unity 做 Lua 绑定  

Lua 用于充当配置文件   
不必再去解析 json , 并且可以尝试做到 不重启游戏随时修改  

## 六边形 TileMap  

## LockStep   

Server 创建主机   
Client 进入主机
Server 点击开始游戏  
Client 加载成功给服务器通知    
Server 开始游戏，开启 LockStep 机制 

需要增加  lobby ,game 的判断，判断是否在游戏中  
需要增加 加入游戏，开始游戏的按钮  
需要增加加载地图的流程  
加入主机时 需要有 是否在 lobby 中的判断。如果游戏已经开始，则不能加入 
 
 


