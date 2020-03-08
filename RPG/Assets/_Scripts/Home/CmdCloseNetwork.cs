using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdCloseNetwork : BaseCommand
{
    public CmdCloseNetwork(object arg) : base(arg)
    {

    }

    public override void Execute()
    {
        if (Home.GetInstance().network != null)
        {
            GameObject.Destroy(Home.GetInstance().network.gameObject);
            Home.GetInstance().network = null;
        }
    }
}
