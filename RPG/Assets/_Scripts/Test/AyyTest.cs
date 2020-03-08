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

        str2Json();
        buildJsonObj();
    }

    // Update is called once per frame
    void Update()
    {
        //
    }

    void str2Json()
    {
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

        for (int i = 0; i < array.Count; i++)
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
    
    void buildJsonObj()
    {
        JsonWriter jwriter = new JsonWriter();
        jwriter.WriteObjectStart();
        {

            jwriter.WritePropertyName("001");
            {
                jwriter.WriteObjectStart();

                jwriter.WritePropertyName("name");
                jwriter.Write("myl");
                jwriter.WritePropertyName("age");
                jwriter.Write(20);

                jwriter.WriteObjectEnd();
            }

            jwriter.WritePropertyName("002");
            {
                jwriter.WriteObjectStart();

                jwriter.WritePropertyName("name");
                jwriter.Write("syz");
                jwriter.WritePropertyName("age");
                jwriter.Write(19);

                jwriter.WriteObjectEnd();
            }
        }
        jwriter.WriteObjectEnd();

        string strJson = jwriter.ToString();
        //JsonData jd2 = new JsonData();
        //jd2.Add(3);
        //string strJson = jd2.ToString();
        Debug.Log(strJson);
    }
}
