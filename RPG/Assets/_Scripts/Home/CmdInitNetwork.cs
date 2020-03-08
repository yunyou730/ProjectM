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
            GameObject network = new GameObject("gameplay_network");
            network.AddComponent<ayy.AyyNetwork>();
            Home.GetInstance().network = network.GetComponent<ayy.AyyNetwork>();
        }
    }
}
