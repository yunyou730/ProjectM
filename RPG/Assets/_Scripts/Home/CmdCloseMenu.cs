using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCloseMenu : BaseCommand
{
    GameObject toCloseGo = null;
    public CmdCloseMenu(object arg):base(arg)
    {
        this.toCloseGo = (GameObject)arg;
    }
    
    public override void Execute()
    {
        GameObject.Destroy(toCloseGo);
    }
}
