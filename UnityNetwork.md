
[手把手教你实现Unity网络同步](https://blog.csdn.net/clb929/article/details/102616711)

[Lockstep Implementation in Unity3D](http://clintonbrennan.com/2013/12/lockstep-implementation-in-unity3d/)

[https://gameinstitute.qq.com/community/detail/104156](https://gameinstitute.qq.com/community/detail/104156)


## Lockstep 在 Unity3D 中的实现  

### Part1

在 lockstep 模式中 ，每个客户端模拟运行整个游戏逻辑。这种方式的好处是，减少了需要发送的数据量。只有用户输入需要被互相发送。与之相反的是 authoritative 模式 ,需要把服务器上面每个单位的位置数据需要被尽可能快的发出去。  

举个例子，想象你想要在游戏世界里面移动一个角色。在 authoritative 模式中，物理模拟只能在服务器上运行。客户端之和服务器通信，知道角色移动到哪里了即可。服务器将会做寻路处理，并且开始移动角色。服务器将要尽可能快的把角色位置数据发送给每个客户端，以使得各个客户端获取到角色最新的位置。必须给游戏世界里面的每个角色都做这样的操作。在即时战略游戏里面，可能回拥有上千个游戏单位，所以 authoritative 模式并不合适。  

在 lockstep 模式，一旦用户决定移动一个角色，这条信息会通信给每个客户端。每个客户端将会各自做寻路和移动角色的处理。只有第一次通信将会通过网络发送，之后每个客户端会有自己的物理模拟，可以在各个客户端中各自更新角色的位置。  

这种 lockstep 的模式的确面临一些挑战：每个客户端必须互相同步。这意味着，每个客户端在模拟的时候必须有相同的帧数，以相同的顺序模拟每一个操作。如果不是这样，某个客户端就可能在处理某个操作时候表现得超前或者落后，运动的路径可能会不一致。这些差异将会导致各个客户端有着不同的表现。  

另一个问题是需要考虑到跨机器和跨平台。小小的计算差异可能会导致蝴蝶效应，使得最终每个客户端呈现很大不同。这个问题将会在将来的文章里更详细的做解释。  

这里的实现收到了[1500 Archers](https://www.gamasutra.com/view/feature/3094/#comments) 这篇文章的启发。

针对当前的实现，我们有如下几个定义. 

####Lockstep 帧

一个 lockstep 帧由多个游戏帧组成。玩家的每一个操作，豆浆会被处理到一个 lockstep 帧中。lockstep帧的时长取决于性能。在这个例子中，我们设为 200ms.  

####游戏帧

一个游戏帧中将更新游戏逻辑和物理模拟逻辑。每个 lockstep帧跨越多少个游戏帧取决于性能。在这个例子中设置为 50ms,或者说 每个 lockstep帧 对应 4个游戏帧。即美妙会执行 20 个游戏帧。  

####行为
一次行为是由玩家下达的指令产生的。举个例子，选择范围内的单位，或者移动单位到目标位置。  

####游戏主循环   

Unity3D 的循环在一个单线程的环境下的。有两个函数可以用于实现我们自定义的代码：

 - Update()   
 - FixedUpdate()   

Unity主循每帧会调用 Update(). 此函数的调用频率会尽可能快，或者依赖于设置的 fps. FixedUpdate() 将会以一个固定的时间间隔被调用，这个时间间隔依赖于项目设置。在每一帧中，FixedUpdate() 可能会被调用一次或多次，这取决于当前帧消耗了多长时间。  

我们需要我们的 lockstep 帧以一个固定的时间间隔来运行，FixedUpdate() 的特性正是我们想要的。但是，FixedUpdate() 的调用频率必须在运行之前做设置，在这个例子中，我们需要根据性能做调整（而不依赖于 FixedUpdate 来做实现)  

####游戏帧   

这个实现类似于将 FixedUpdate() 放到 Update() 中调用。区别是，这样做我们可以调整调用频率。我们通过一个 “累计时间” accumulative time 来实现这一点。 前一帧的时间将会在 Update() 函数中，被累加到 accumulative time 上。这就是 Time.deltaTime. 如果累计时间大雨我们设置的固定游戏帧率(50ms) ,我们做一次 gameframe() 函数的调用。我们会继续调用 gameframe() ，并从 accumulative time 减去 50ms ,直到 accumulative time 小于 50ms 为止。

	private float AccumilatedTime = 0f;
	 
	private float FrameLength = 0.05f; //50 miliseconds
	 
	//called once per unity frame
	public void Update() {
	    //Basically same logic as FixedUpdate, but we can scale it by adjusting FrameLength
	    AccumilatedTime = AccumilatedTime + Time.deltaTime;
	 
	    //in case the FPS is too slow, we may need to update the game multiple times a frame
	    while(AccumilatedTime > FrameLength) {
	        GameFrameTurn ();
	        AccumilatedTime = AccumilatedTime - FrameLength;
	    }
	}

我们要掌握当前 lockstep 帧所对应的游戏帧的数量。当我们达到"每 lockstep 帧所对应的游戏帧数" 时，我们需要在下一个游戏帧里面，更新 lockstep 帧。 如果 lockstep 还没有为执行到下一轮做好准备，我们将不会增加游戏帧的数量，并且我们继续在下一帧中处理 lockstep 帧。 

	private void GameFrameTurn() {
	    //first frame is used to process actions
	    if(GameFrame == 0) {
	        if(LockStepTurn()) {
	            GameFrame++;
	        }
	    } else {
	        //update game
	 
	        //...
	         
	        GameFrame++;
	        if(GameFrame == GameFramesPerLocksetpTurn) {
	            GameFrame = 0;
	        }
	    }
	}

在游戏帧中，我们更新物理和游戏逻辑。拥有游戏逻辑的物体，要实现 IHasGameFrame 接口。我们把这些物体放到一个集合中，以便于我们可以遍历。 
	
	private void GameFrameTurn() {
	    //first frame is used to process actions
	    if(GameFrame == 0) {
	        if(LockStepTurn()) {
	            GameFrame++;
	        }
	    } else {
	        //update game
	        SceneManager.Manager.TwoDPhysics.Update (GameFramesPerSecond);
	         
	        List<IHasGameFrame> finished = new List<IHasGameFrame>();
	        foreach(IHasGameFrame obj in SceneManager.Manager.GameFrameObjects) {
	            obj.GameFrameTurn(GameFramesPerSecond);
	            if(obj.Finished) {
	                finished.Add (obj);
	            }
	        }
	         
	        foreach(IHasGameFrame obj in finished) {
	            SceneManager.Manager.GameFrameObjects.Remove (obj);
	        }
	         
	        GameFrame++;
	        if(GameFrame == GameFramesPerLocksetpTurn) {
	            GameFrame = 0;
	        }
	    }
	}

接口 IHasGameFrame 包含一个叫做 GameFrameTurn 的方法，该方法有一个参数:"每秒执行多少游戏帧数"。拥有游戏逻辑的物体，每帧计算时需要给予 GameFramePerSecond 这个参数。比如，一个单位公基另一个单位，每秒钟造成10点伤害。你应该在每帧处理伤害值的时候，用 10 除以 GameFramePerSecond 。参数 GameFramePerSecond 取决于性能。  

IHasGameFrame 接口需要明确的指出何时逻辑完成。这可以使得物体能够通知住循环，自己不再需要被更新了。举个例子，一个物体要沿着一条路径走，一旦走到目的地，这个物体就不再需要在住循环里被更新了。  

####Lockstep 帧 

为了能和其他客户端同步，每个 lockstep 帧必须问如下问题:  

 - 我们是否收已经到了每个客户端下一帧的行为 ?
 - 每个客户端都确认收到了我们的行为了吗？ 

 我们有两个对象，分别是 ConfirmActions 和 PendingActions. 每个对象都有一个集合，用于存储可能收到的消息。 我们在进行到下一轮的时候，要检查这两个物体是否已经准备完毕。    
 
		 
		private bool NextTurn() {       
		    if(confirmedActions.ReadyForNextTurn() && pendingActions.ReadyForNextTurn()) {
		        //increment the turn ID
		        LockStepTurnID++;
		        //move the confirmed actions to next turn
		        confirmedActions.NextTurn();
		        //move the pending actions to this turn
		        pendingActions.NextTurn();
		         
		        return true;
		    }
		     
		    return false;
		}
			 
####行为(Actions)   

Actions 或者 Commands 通过实现 IAction 接口进行通信。 IAction 有一个不带参数的方法 ProcessAction(). 这个类必须可序列化。这意味着这个类的每个字段也都可序列化。当用户和 UI 交互时，创建一个 action 的实例，并放松到我们的额 lockstep manager 的一个队列里。这个队列用于这样的情形：游戏速度太慢了，用户可以在一个 lockstep 帧里发送 超过一条以上的 command. 同时在发送的 command 只有 1个，并且每条 command 都不能被忽略 。  

我们发送 action 给其他玩家，这个 action 的实例将会序列话到一个 byte 数组里，并由其他玩家反序列化。当玩家在某一帧没做任何操作时，默认发送一个 "NoAction" 对象。其他的 action 将会被指定到游戏逻辑中。这里给出一个创建新单位的 action 的例子： 

	using System;
	using UnityEngine;
	 
	[Serializable]
	public class CreateUnit : IAction
	{
	    int owningPlayer;
	    int buildingID;
	     
	    public CreateUnit (int owningPlayer, int buildingID) {
	        this.owningPlayer = owningPlayer;
	        this.buildingID = buildingID;
	    }
	     
	    public void ProcessAction() {
	        Building b = SceneManager.Manager.GamePieceManager.GetBuilding(owningPlayer, buildingID);
	        b.SpawnUnit();
	    }
	}

这个 action 依赖于 SceneManager 有一个静态的引用。如果你不喜欢这样的实现，IAction 接口可以修改为参数接收一个 SceneManager 的实例。  


####补充 :  
下面有一些评论，似乎作者在 判断 GameFrame == 0 时，只执行 LockStepTurn() 这一段 是作者的失误，需要改正。

Daniel on November 16, 2015   
Hey first of all, thanks for writing this. This has helped me a lot with my own game.
	
I have a question regarding the GameFrameTurn() method.
	
Why is it what when GameFrame ==0 that you only want to process the lockstep actions but not update the game? To me it makes sense you would still want to update the game inside of “if(LockStepTurn()) { }” or your kind of throwing out a frame for no reason arent you?
	
Or is there a specific reason you have for not updating the game model on the same frame as the lock step frame?
	
reply
	
Clinton on January 25, 2016  
No specific reason, just a miss when I first wrote the code

----------

tobias on December 23, 2013  
Oh and by the way I found a small mistake in your code. In your LockstepManager you increment “GameFrame++” and you say you achieve 4 Gameframe calls but because you increment it in and don’t execute the gameframe there,

	if(GameFrame == 0) {
		if(LockStepTurn()) {
			GameFrame++;
	}
	
it will only be called 3 times per Lockstep tick. Or 15 times per second.
	
### Part2 


