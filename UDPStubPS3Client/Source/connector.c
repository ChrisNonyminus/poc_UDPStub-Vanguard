//RTC_HIJACK: Custom functions
//thx https://www.geeksforgeeks.org/udp-server-client-implementation-c/
#define PORT 42042
#define MAXLINE 1024

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
#include <sys/thread.h>
#include <ppu-types.h>

#include <main.h>
#include <connector.h>
#include <ps3mapi_func.h>
bool connected;
int sockfd;
char buffer[MAXLINE];
struct sockaddr_in servaddr;
socklen_t len;
char buf[512];

void Connector_Connect()
{
	if ((sockfd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) < 0)
	{
		return;
	}
	memset(&servaddr, 0, sizeof(servaddr));
	servaddr.sin_family = AF_INET;
	servaddr.sin_port = htons(PORT);
	servaddr.sin_addr.s_addr = INADDR_ANY;

	if (bind(sockfd, (struct sockaddr*)&servaddr, sizeof(struct sockaddr_in)) < 0)
	{
		return;
	}
	connected = true;
	int n;
	len = sizeof(servaddr);
	sendto(sockfd, (const char*)"PS3 says hello?\n", 17, 0, (const struct sockaddr*)&servaddr, sizeof(servaddr));
	//(SYS_PPU_THREAD_NONE, Connector_ExecuteCommand, (u64)webman_config->poll, THREAD_PRIO_POLL, THREAD_STACK_SIZE_POLL_THREAD, SYS_PPU_THREAD_CREATE_JOINABLE, THREAD_NAME_POLL);
    Connector_ExecuteCommand();
	return;
}

void Connector_ExecuteCommand()
{
	while(1)
	{
		
    const char* func = "";
    const char* arg1 = "";
    const char* arg2 = "";
    const char* arg3 = "";
    const char* arg4 = "";
    printf("Waitin\' for command...\n");
    const char* cmd = Connector_Receive();
    printf("%s\n", cmd);
    //thanks https://stackoverflow.com/questions/14265581/parse-split-a-string-in-c-using-string-delimiter-standard-c
    // if (cmd.find("lv2_read8") != const char*::npos)
    // {
    //     int args = 0;
    //     size_t pos = 0;
    //     while ((pos = cmd.find('|')) != const char*::npos)
    //     {
    //         if (args == 0)
    //         {
    //             func = cmd.substr(0, pos);
    //         }
    //         if (args == 1)
    //         {
    //             arg1 = cmd.substr(0, pos);
    //         }
    //         if (args == 2)
    //         {
    //             arg2 = cmd.substr(0, pos);
    //         }
    //         if (args == 3)
    //         {
    //             arg3 = cmd.substr(0, pos);
    //         }
    //         if (args == 4)
    //         {
    //             arg4 = cmd.substr(0, pos);
    //         }
    //         cmd.erase(0, pos + 1);
    //         args++;
    //     }
    char* token = strtok(cmd, "|");
    int args = 0;
    while(token != NULL)
    {
        if(args == 0)
        {
            func = token;
        }
        if (args == 1)
        {
            arg1 = token;
        }
        if (args == 2)
        {
            arg2 = token;
        }
        if (args == 3)
        {
            arg3 = token;
        }
        if (args == 4)
        {
            arg4 = token;
        }
        token = strtok(NULL, "|");
        args += 1;
    }
    if(func == "lv2_read8")
    {
        uint32_t decAddr = atoi(arg1);
        uint8_t decVal = atoi(arg2);
        int map = atoi(arg3);
        printf("Calling %s(%X)...\n", func, decAddr);
        printf("%X\n", NetStub_PeekLV2Byte(decAddr));
        const char* result;
        sprintf(result, "lv2_read8: %d", NetStub_PeekLV2Byte(decAddr));
        Connector_Send(result);
    }
    if (func == "lv2_write8")
    {
        uint32_t decAddr = atoi(arg1);
        uint8_t decVal = atoi(arg2);
        int map = NetStub_GetCurrentPID();
        printf("Calling %s(%X, %X)...\n", func, decAddr, decVal);
        NetStub_PokeLV2Byte(decAddr, decVal);
    }
    if (func == "proc_read8")
    {
        
        uint32_t decAddr = atoi(arg1);
        int map = NetStub_GetCurrentPID();
        printf("Calling %s(%X, %X)...\n", func, map, decAddr);
        printf("%X\n", NetStub_PeekProcessByte(map, decAddr));
        const char* result;
        sprintf(result, "proc_read8: %d", NetStub_PeekProcessByte(map, decAddr));
        Connector_Send(result);
    }
    if (func == "proc_write8")
    {
        uint32_t decAddr = atoi(arg1);
        uint8_t decVal = atoi(arg2);
        int map = NetStub_GetCurrentPID();
        printf("Calling %s(%X, %X, %X)...\n", func, map, decAddr, decVal);
        NetStub_PokeProcessByte(map, decAddr, decVal);
    }

    // if (cmd.find("GetProcMapSize") != const char*::npos)
    // {
    //     int args = 0;
    //     size_t pos = 0;
    //     while ((pos = cmd.find('|')) != const char*::npos)
    //     {
    //         if (args == 0)
    //         {
    //             func = cmd.substr(0, pos);
    //         }
    //         if (args == 1)
    //         {
    //             arg1 = cmd.substr(0, pos);
    //         }
    //         if (args == 2)
    //         {
    //             arg2 = cmd.substr(0, pos);
    //         }
    //         if (args == 3)
    //         {
    //             arg3 = cmd.substr(0, pos);
    //         }
    //         if (args == 4)
    //         {
    //             arg4 = cmd.substr(0, pos);
    //         }
    //         cmd.erase(0, pos + 1);
    //         args++;
    //     }
    //     uint32_t map = atoi(arg1.c_str());
    //     printf("Calling %s(%d)...\n", func.c_str(), map);
    //     Send("GetProcMapSize: " + ProcessManager::GetProcMapSize(map));
    // }
    // if (cmd.find("GetProcMapStartAddr") != const char*::npos)
    // {
    //     int args = 0;
    //     size_t pos = 0;
    //     while ((pos = cmd.find('|')) != const char*::npos)
    //     {
    //         if (args == 0)
    //         {
    //             func = cmd.substr(0, pos);
    //         }
    //         if (args == 1)
    //         {
    //             arg1 = cmd.substr(0, pos);
    //         }
    //         if (args == 2)
    //         {
    //             arg2 = cmd.substr(0, pos);
    //         }
    //         if (args == 3)
    //         {
    //             arg3 = cmd.substr(0, pos);
    //         }
    //         if (args == 4)
    //         {
    //             arg4 = cmd.substr(0, pos);
    //         }
    //         cmd.erase(0, pos + 1);
    //         args++;
    //     }
    //     uint32_t map = atoi(arg1.c_str());
    //     printf("Calling %s(%d)...\n", func.c_str(), map);
    //     Send("GetProcMapStartAddr: " + ProcessManager::GetProcMapSize(map));
    // }
    if (func == "GetProcName")
    {
		uint32_t map = atoi(arg1);
        printf("Calling %s(%d)...\n", func, map);
        const char* name = NetStub_GetProcessName(NetStub_GetCurrentPID());
		const char* result;
		sprintf(result, "GetProcName: %s", name);
        Connector_Send(result);
    }
    // if (cmd.find("GetMaxMaps") != const char*::npos)
    // {
    //     func = "GetMaxMaps";
    //     printf("Calling %s()...\n", func.c_str());
    //     Send("GetMaxMaps: " + ProcessManager::GetMaxMaps());
    // }
	}
}

const char* Connector_Receive()
{
    int len;
    len = sizeof(servaddr);
    int n = recvfrom(sockfd, buffer, MAXLINE, 0, (struct sockaddr*)&servaddr, (socklen_t*)&len);
    buffer[n] = '\0';
    printf("recvfrom returns %i : %s\n", n, buffer);
    return buffer;
}

void Connector_Send(const char* text)
{
    sendto(sockfd, text, strlen(text), 0, (const struct sockaddr*)&servaddr, sizeof(servaddr));
}
void Connector_Disconnect()
{
    close(sockfd);
}
//RTC_HIJACK END