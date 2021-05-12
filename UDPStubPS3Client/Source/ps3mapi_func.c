
#include <stdarg.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/types.h>
#include <sys/thread.h>
#include <ppu-types.h>

#include <main.h>
#include <ps3mapi.h>
#include <ps3mapi_func.h>

bool NetStub_PS3MAPI_Available()
{
    if (has_ps3mapi() == 1)
        if(ps3mapi_support_sc8_peek_poke() == PS3MAPI_OPCODE_SUPPORT_SC8_PEEK_POKE_OK)
            return true;
        return false;
    return false;
}

uint8_t NetStub_PeekLV2Byte(uint32_t addr)
{
    uint64_t data;
    if(NetStub_PS3MAPI_Available() == true)
    {
        data = ps3mapi_lv2_peek((uint64_t)addr);
        return data >> 8;
    }
    return 0xFF; //0xFF means peek/poke in the region is unimplemented or an error occured
}

void NetStub_PokeLV2Byte(uint32_t addr, uint8_t val)
{
    if(NetStub_PS3MAPI_Available() == true)
    {
        ps3mapi_set_process_mem(2, (uint64_t)addr, val, 1);
    }
}

uint8_t NetStub_PeekProcessByte(process_id_t pid, uint32_t addr)
{

    uint8_t val;
    if(NetStub_PS3MAPI_Available() == true)
    {
    ps3mapi_get_process_mem(pid, (uint64_t)addr, val, 1);
    return val;
    }
    return 0xFF
}

void NetStub_PokeProcessByte(process_id_t pid, uint32_t addr, uint8_t val)
{
    if(NetStub_PS3MAPI_Available() == true)
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