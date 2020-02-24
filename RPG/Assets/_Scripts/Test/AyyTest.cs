using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

public class AyyTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*
        Debug.Log(AyyNative.TestAdd(5, 3));
        int r = AyyNative.AyyLoadLua("aaa");
        Debug.Log(r);
        */

        string jsonStr = @"{
            'name':'miao',
            'gender':'male',
            'age':31,
            'girlfriend':'shenyizhi',
            'array':[1,'bbb','432'],
            'map':{
                'key1':true,
                'key2':2333
            }
        }";

        JsonData jd = JsonMapper.ToObject(jsonStr);
        JsonData array = jd["array"];
        
        for (int i = 0;i < array.Count;i++)
        {
            Debug.Log(array[i]);
        }

        JsonData map = jd["map"];
        ICollection<string> keys = map.Keys;
        foreach (var key in keys)
        {
            Debug.Log(map[key]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
    }
}
