using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLanGame : MenuBase
{
    // Start is called before the first frame update
    void Start()
    {
        // start listen host
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // close listen host
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
