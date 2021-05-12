#include "main.h"
#include <stdint.h>
#include <ps3mapi.h>
//taken from https://github.com/aldostools/webMAN-MOD/blob/d34db735fe1f1698f3deffda0c0dde56aee68cfc/vsh/vshmain.h
extern uint32_t vshmain_0624D3AE(void);  // returns game u32 process id
#define GetGameProcessID vshmain_0624D3AE
//end https://github.com/aldostools/webMAN-MOD/blob/d34db735fe1f1698f3deffda0c0dde56aee68cfc/vsh/vshmain.h

bool NetStub_PS3MAPI_Available(void);

uint8_t NetStub_PeekLV2Byte(uint32_t addr);

void NetStub_PokeLV2Byte(uint32_t addr, uint8_t val);

uint8_t NetStub_PeekProcessByte(process_id_t pid, uint32_t addr);

void NetStub_PokeProcessByte(process_id_t pid, uint32_t addr, uint8_t val);

process_id_t NetStub_GetCurrentPID(void);

const char* NetStub_GetProcessName(process_id_t pid);