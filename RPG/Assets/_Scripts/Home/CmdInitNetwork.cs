using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdInitNetwork : BaseCommand
{
    public CmdInitNetwork(object arg):base(arg)
    {
        
    }

    public override void Execute()
    {
        if (Home.GetInstance().network == null)
        {
            GameObject prefab = Resources.Load<GameObject>("Gameplay/NetworkManager");
            GameObject network = GameObject.Instantiate(prefab);
            Home.GetInstance().network = network.GetComponent<ayy.AyyNetwork>();
        }
    }
}
