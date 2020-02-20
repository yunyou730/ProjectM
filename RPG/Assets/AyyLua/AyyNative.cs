using UnityEngine;
using System.Runtime.InteropServices;

public class AyyNative
{
#if UNITY_IPHONE
    public const string LIB_AYY_NATIVE = "__Internal";
#else
    public const string LIB_AYY_NATIVE = "AyyNative";
#endif

    [DllImport(LIB_AYY_NATIVE)]
    public static extern int TestAdd(int a,int b);

    [DllImport(LIB_AYY_NATIVE)]
    public static extern int AyyLoadLua(string luaCode);

}
