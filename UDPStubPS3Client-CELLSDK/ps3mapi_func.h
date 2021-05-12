#include <stdarg.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <stdio.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include <sys/types.h>
#include <sys/ppu_thread.h>
#include <types.h>
#include <ps3mapi_ps3_lib.h>
#define PS3MAPI_OPCODE_SUPPORT_SC8_PEEK_POKE	0x1000
#define PS3MAPI_OPCODE_SUPPORT_SC8_PEEK_POKE_OK	0x6789
#define PS3MAPI_OPCODE_LV2_PEEK					0x1006
#define PS3MAPI_OPCODE_LV2_POKE					0x1007
#define PS3MAPI_OPCODE_LV1_PEEK					0x1008
#define PS3MAPI_OPCODE_LV1_POKE					0x1009
//taken from https://github.com/aldostools/webMAN-MOD/blob/d34db735fe1f1698f3deffda0c0dde56aee68cfc/vsh/vshmain.h
extern uint32_t vshmain_0624D3AE(void);  // returns game u32 process id
#define GetGameProcessID vshmain_0624D3AE
//end https://github.com/aldostools/webMAN-MOD/blob/d34db735fe1f1698f3deffda0c0dde56aee68cfc/vsh/vshmain.h

int NetStub_PS3MAPI_Available(void);

uint8_t NetStub_PeekLV2Byte(uint32_t addr);

void NetStub_PokeLV2Byte(uint32_t addr, uint8_t val);

uint8_t NetStub_PeekProcessByte(process_id_t pid, uint32_t addr);

void NetStub_PokeProcessByte(process_id_t pid, uint32_t addr, uint8_t val);

process_id_t NetStub_GetCurrentPID(void);

const char* NetStub_GetProcessName(uint32_t pid);

