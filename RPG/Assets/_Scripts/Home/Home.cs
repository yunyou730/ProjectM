using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public static Home instance = null;

    [HideInInspector]
    public GameObject menuRoot { set; get; }

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
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu("Menu/MenuHome"));
    }

    /*
    public void SwitchMenu(GameObject menuGo)
    {
        // remove attached children
        for (int i = 0;i < menuRoot.transform.childCount; i++)
        {
            Transform oldMenu = menuRoot.transform.GetChild(i);
            GameObject.Destroy(oldMenu.gameObject);
        }
        // attach new children
        menuGo.transform.SetParent(menuRoot.transform);
    }
    */
}
