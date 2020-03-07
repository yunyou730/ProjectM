using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdOpenMenu : BaseCommand
{
    string menuPath = null;

    public CmdOpenMenu(object arg):base(arg)
    {
        menuPath = (string)arg;
    }

    public override void Execute()
    {
        GameObject prefab = Resources.Load<GameObject>(menuPath);
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.SetParent(Home.GetInstance().menuRoot.transform);
    }
}
