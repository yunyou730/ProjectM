using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuLanGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickCreateGame()
    {
        
    }

    public void OnClickBack()
    {
        CmdCenter.GetInstance().RunCmd(new CmdCloseMenu(gameObject));
        CmdCenter.GetInstance().RunCmd(new CmdOpenMenu("Menu/MenuHome"));
    }
}
