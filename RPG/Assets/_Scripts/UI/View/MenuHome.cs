using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHome : MenuBase
{
    public GameObject lanGamePrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickLanGame()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));

        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuLanGame");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }

    public void OnClickOldNetwork()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));

        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuNetwork");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
