using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static Home instance = null;

    [HideInInspector]
    public GameObject menuRoot { set; get; }

    [HideInInspector]
    public ayy.AyyNetwork network = null;

    public static Home GetInstance() {
        return instance;
    }
    
    void Awake()
    {
        instance = this;
        menuRoot = GameObject.Find("MenuRoot");
    }
    

    void Start()
    {
        ShowMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowMainMenu()
    {
        Dictionary<string, object> arg = new Dictionary<string, object>();
        arg.Add("menu_path", "Menu/MenuHome");
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu(arg));
    }
}
