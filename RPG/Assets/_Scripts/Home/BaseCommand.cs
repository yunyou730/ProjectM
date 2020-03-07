using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCommand
{
    protected object arg;
    public BaseCommand(object arg)
    {
        this.arg = arg;
    }

    public abstract void Execute();
}


public class CmdCenter
{
    private static CmdCenter instance = null;


    public static CmdCenter GetInstance()
    {
        if (CmdCenter.instance == null)
        {
            CmdCenter.instance = new CmdCenter();
        }
        return instance;
    }

    private CmdCenter()
    {
        
    }

    public void RunCmd(BaseCommand cmd)
    {
        cmd.Execute();
    }
}

