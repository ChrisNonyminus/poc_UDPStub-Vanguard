
#include <stdarg.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/types.h>
#include <sys/ppu_thread.h>
#include <types.h>
#include <sys/syscall.h>

#include <main.h>
#include <ps3mapi_ps3_lib.h>
#include <ps3mapi_func.h>
//0xFF means peek/poke in the region is unimplemented or an error occured
int NetStub_PS3MAPI_Available()
{
    if (ps3mapi_get_core_version() >= PS3MAPI_CORE_MINVERSION)
    {
        system_call_2(8, 0x7777, 0x1000);
        if(p1 = 0x6789)
            return 1;
        return 0;
    }
    return 0;
}

uint8_t NetStub_PeekLV2Byte(uint32_t addr)
{
    uint64_t data;
    if(NetStub_PS3MAPI_Available() == 1)
    {
        system_call_3(8, 0x7777, PS3MAPI_OPCODE_LV2_PEEK, (uint64_t)addr);
        return (uint8_t)(((uint64_t)p1) >> 8);
    }
    else
    {
        system_call_1((6), addr);
        return (uint8_t)(((uint64_t)p1) >> 8); 
    }
}

void NetStub_PokeLV2Byte(uint32_t addr, uint8_t val)
{
    if(NetStub_PS3MAPI_Available() == 1)
    {
        ps3mapi_set_process_mem(2, (uint64_t)addr, val, 1);
    }
    else
    {

    }
}

uint8_t NetStub_PeekProcessByte(process_id_t pid, uint32_t addr)
{

    uint8_t val;
    if(NetStub_PS3MAPI_Available() == 1)
    {
        ps3mapi_get_process_mem(pid, (uint64_t)addr, val, 1);
        return val;
    }
    else
    {
        return 0xFF;
    }
}

void NetStub_PokeProcessByte(process_id_t pid, uint32_t addr, uint8_t val)
{
    if(NetStub_PS3MAPI_Available() == 1)
    {
        ps3mapi_set_process_mem(pid, (uint64_t)addr, val, 1);
    }
}

process_id_t NetStub_GetCurrentPID()
{
    return GetGameProcessID();
}

const char* NetStub_GetProcessName(process_id_t pid)
{
    const char* name;
    ps3mapi_get_process_name_by_pid(pid, name);
    return name;
}