using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AyyTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(AyyNative.TestAdd(5, 3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
