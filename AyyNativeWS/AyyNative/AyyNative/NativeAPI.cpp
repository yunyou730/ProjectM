#include "NativeAPI.h"
#include <stdio.h>

int AyyLoadLua(char* luaCode)
{
    printf("%s\n",luaCode);
    return 730;
}


void AyyLuaHello()
{
    lua_State* L = luaL_newstate();
    luaL_openlibs(L);
    
    const char* luaCode = "";
    
    lua_close(L);
        
}
