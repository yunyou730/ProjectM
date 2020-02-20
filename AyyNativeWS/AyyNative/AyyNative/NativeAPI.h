#pragma once

extern "C"
{
#include "lua.h"
#include "lauxlib.h"
#include "lualib.h"
    
}

extern "C"
{
int AyyLoadLua(char* luaCode);

void AyyLuaHello();

}
