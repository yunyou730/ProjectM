using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdOpenMenu : BaseCommand
{
    string menuPath = null;
    Dictionary<string, object> enterArg = null;

    public CmdOpenMenu(object arg):base(arg)
    {
        Dictionary<string, object> dicArg = (Dictionary<string, object>)arg;
        menuPath = (string)(dicArg["menu_path"]);
        if (dicArg.ContainsKey("enter_arg"))
        {
            enterArg = (Dictionary<string, object>)(dicArg["enter_arg"]);
        }
    }

    public override void Execute()
    {
        // create menu
        GameObject prefab = Resources.Load<GameObject>(menuPath);
        GameObject go = GameObject.Instantiate(prefab);

        // set arg
        MenuBase menu = go.GetComponent<MenuBase>();
        if (menu != null)
        {
            menu.enterArg = enterArg;
        }

        // show menu
        go.transform.SetParent(Home.GetInstance().menuRoot.transform);
    }
}
