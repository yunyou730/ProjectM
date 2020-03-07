using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ayy;

public class MenuLanGame : MenuBase
{
    AyyHostListener hostListener = new AyyHostListener();

    void Start()
    {
        hostListener.Start();
    }

    
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // close listen host
        hostListener.Stop();
        hostListener = null;
    }

    public void OnClickCreateGame()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));

        Dictionary<string, object> arg = new Dictionary<string, object>();
        Dictionary<string, object> enterArg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuLobby");
        arg.Add("enter_arg", enterArg);
        enterArg.Add("self_host", true);
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }

    public void OnClickBack()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));

        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuHome");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }
}
