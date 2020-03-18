using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace crayon
{
    public class Util
    {
        public static void Exchange<T>(ref T v1,ref T v2)
        {
            T tmp = v1;
            v1 = v2;
            v2 = tmp;
        }
    }
}
