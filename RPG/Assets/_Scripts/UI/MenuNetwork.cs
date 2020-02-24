using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;

public class MenuNetwork : MonoBehaviour
{
    public AyyNetwork _network = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickStartServer()
    {
        _network.StartAsServer();
    }

    public void OnClickServerStartGame()
    {
        _network.ServerStartGame();
    }

    public void OnClickStartClient()
    {
        _network.StartAsClient();
    }

    public void OnClickClientSend()
    {
        _network._client.Send();
    }
}
