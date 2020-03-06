using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    private GameObject menuRoot = null;
    
    void Awake()
    {
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
        GameObject prefab = Resources.Load<GameObject>("Menu/MenuHome");
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.SetParent(menuRoot.transform);
    }


    void EnterLanGame()
    {
        ShowLanGameMenu();
    }
    
    void ExitLanGame()
    {
        
    }
    
    void ShowLanGameMenu()
    {
        // Show UI
        GameObject prefab = Resources.Load<GameObject>("Menu/MenuNetwork");
        GameObject go = GameObject.Instantiate(prefab);
        go.transform.SetParent(menuRoot.transform);
    }
    
    void SwitchMenu(GameObject menuGo)
    {
        
        menuGo.transform.SetParent(menuRoot.transform);
    }

}
