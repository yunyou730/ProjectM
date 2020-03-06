using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHome : MonoBehaviour
{
    public GameObject lanGamePrefab = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnClickLanGame()
    {
        Debug.Log("lan game");
    }

    public void OnclickQuit()
    {
        Application.Quit();
    }
}
