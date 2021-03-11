#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include "Connector.h"
#include <string>
#include <iostream>
#include <sstream>
#include <Proc.h>
#include <cstddef>
//thx https://www.geeksforgeeks.org/udp-server-client-implementation-c/
#define PORT 42042
#define MAXLINE 1024
bool connected;
int sockfd;
char buffer[MAXLINE];
char* helloworld = "Linux: \"Hello, Windows!\"\n";
struct sockaddr_in servaddr; 
socklen_t len;
char buf[512];
void UDPConnector::Connect()
{
    printf("Type your Windows PC's IP address reported by Linux Stub's Windows Side.\n>> ");
    std::string winIP;
    std::cin >> winIP;
    std::string cmd;
    cmd = "hostname -I";
    FILE* cmd_pipe = popen(cmd.c_str(), "r");
    fgets(buf, 512, cmd_pipe);
    std::string ip;
    ip = buf;
    printf("Linux: \"My address is %s...\nMake note of this for Windows.\"\n", ip.c_str());
    if ((sockfd = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP)) < 0)
    {
        printf("Failed to create a socket :(\n");
        exit(EXIT_FAILURE);
    }
    memset(&servaddr, 0, sizeof(servaddr));
    servaddr.sin_family = AF_INET;
    servaddr.sin_port = htons(PORT);
    servaddr.sin_addr.s_addr = inet_addr(winIP.c_str());
    //servaddr.sin_addr.s_addr = INADDR_ANY;
    printf("Type \"connect\" without quotes when you are ready and you've clicked \"Connect\" on the windows side...\n>> ");
    std::string connectstr;
    std::cin >> connectstr;

    if (connect(sockfd, (struct sockaddr*)&servaddr, sizeof(servaddr)) < 0 || connectstr != "connect")
    {
        printf("\n Error : Connect Failed \n");
        exit(1);
    }
    connected = true;
    int n;
    len = sizeof(servaddr);
    sendto(sockfd, (const char*)helloworld, strlen(helloworld), 0, (const struct sockaddr*)&servaddr, sizeof(servaddr));
    printf("Linux: \"Hopefully, I said Hello to Windows.\"\n");
    while(1)
    {
        /*n = recvfrom(sockfd, buffer, sizeof(buffer), 0, (struct sockaddr*)NULL, NULL);
        puts(buffer);
        printf("recvfrom returns %i : %s\n", n,  buffer);*/
        ExecuteCommand();
    }
    return;
}
std::string UDPConnector::Recieve()
{
    int len;
    len = sizeof(servaddr);
    //int n = recvfrom(sockfd, buffer, MAXLINE, 0, (struct sockaddr*)&servaddr, (socklen_t*)&len);
    int n = recv(sockfd, buffer, MAXLINE, 0);
    buffer[n] = '\0';
    printf("recvfrom returns %i : %s\n", n, buffer);
    return buffer;
}
std::string UDPConnector::Send(std::string text)
{
    sendto(sockfd, text.c_str(), strlen(text.c_str()), 0, (const struct sockaddr*)&servaddr, sizeof(servaddr));
}
void UDPConnector::Disconnect()
{
    close(sockfd);
}

void UDPConnector::ExecuteCommand()
{
    std::string func = "";
    std::string arg1 = "";
    std::string arg2 = "";
    std::string arg3 = "";
    std::string arg4 = "";
    printf("Waitin\' for command...\n");
    std::string cmd = Recieve();
    printf("%s\n", cmd.c_str());
    //thanks https://stackoverflow.com/questions/14265581/parse-split-a-string-in-c-using-string-delimiter-standard-c
    if (cmd.find("read8") != std::string::npos)
    {
        int args = 0;
        size_t pos = 0;
        while ((pos = cmd.find('|')) != std::string::npos)
        {
            if (args == 0)
            {
                func = cmd.substr(0, pos);
            }
            if (args == 1)
            {
                arg1 = cmd.substr(0, pos);
            }
            if (args == 2)
            {
                arg2 = cmd.substr(0, pos);
            }
            if (args == 3)
            {
                arg3 = cmd.substr(0, pos);
            }
            if (args == 4)
            {
                arg4 = cmd.substr(0, pos);
            }
            cmd.erase(0, pos + 1);
            args++;
        }
        uint32_t decAddr = atoi(arg1.c_str());
        uint8_t decVal = atoi(arg2.c_str());
        int map = atoi(arg3.c_str());
        printf("Calling %s(%d, %d, %d)...\n", func.c_str(), decAddr, decVal, map);
        printf("%X\n", ProcessManager::read8(decAddr, decVal, map));
        Send("read8: " + std::to_string(ProcessManager::read8(decAddr, decVal, map)));
    }
    if (cmd.find("write8") != std::string::npos)
    {
        int args = 0;
        size_t pos = 0;
        while ((pos = cmd.find('|')) != std::string::npos)
        {
            if (args == 0)
            {
                func = cmd.substr(0, pos);
            }
            if (args == 1)
            {
                arg1 = cmd.substr(0, pos);
            }
            if (args == 2)
            {
                arg2 = cmd.substr(0, pos);
            }
            if (args == 3)
            {
                arg3 = cmd.substr(0, pos);
            }
            if (args == 4)
            {
                arg4 = cmd.substr(0, pos);
            }
            cmd.erase(0, pos + 1);
            args++;
        }
        uint32_t decAddr = atoi(arg1.c_str());
        uint8_t decVal = atoi(arg2.c_str());
        int map = atoi(arg3.c_str());
        printf("Calling %s(%d, %d, %d)...\n", func.c_str(), decAddr, decVal, map);
        ProcessManager::write8(decAddr, decVal, map);
    }
    if (cmd.find("GetProcMapSize") != std::string::npos)
    {
        int args = 0;
        size_t pos = 0;
        while ((pos = cmd.find('|')) != std::string::npos)
        {
            if (args == 0)
            {
                func = cmd.substr(0, pos);
            }
            if (args == 1)
            {
                arg1 = cmd.substr(0, pos);
            }
            if (args == 2)
            {
                arg2 = cmd.substr(0, pos);
            }
            if (args == 3)
            {
                arg3 = cmd.substr(0, pos);
            }
            if (args == 4)
            {
                arg4 = cmd.substr(0, pos);
            }
            cmd.erase(0, pos + 1);
            args++;
        }
        uint32_t map = atoi(arg1.c_str());
        printf("Calling %s(%d)...\n", func.c_str(), map);
        Send("GetProcMapSize: " + ProcessManager::GetProcMapSize(map));
    }
    if (cmd.find("GetProcMapStartAddr") != std::string::npos)
    {
        int args = 0;
        size_t pos = 0;
        while ((pos = cmd.find('|')) != std::string::npos)
        {
            if (args == 0)
            {
                func = cmd.substr(0, pos);
            }
            if (args == 1)
            {
                arg1 = cmd.substr(0, pos);
            }
            if (args == 2)
            {
                arg2 = cmd.substr(0, pos);
            }
            if (args == 3)
            {
                arg3 = cmd.substr(0, pos);
            }
            if (args == 4)
            {
                arg4 = cmd.substr(0, pos);
            }
            cmd.erase(0, pos + 1);
            args++;
        }
        uint32_t map = atoi(arg1.c_str());
        printf("Calling %s(%d)...\n", func.c_str(), map);
        Send("GetProcMapStartAddr: " + ProcessManager::GetProcMapSize(map));
    }
    if (cmd.find("GetProcMapName") != std::string::npos)
    {
        int args = 0;
        size_t pos = 0;
        while ((pos = cmd.find('|')) != std::string::npos)
        {
            if (args == 0)
            {
                func = cmd.substr(0, pos);
            }
            if (args == 1)
            {
                arg1 = cmd.substr(0, pos);
            }
            if (args == 2)
            {
                arg2 = cmd.substr(0, pos);
            }
            if (args == 3)
            {
                arg3 = cmd.substr(0, pos);
            }
            if (args == 4)
            {
                arg4 = cmd.substr(0, pos);
            }
            cmd.erase(0, pos + 1);
            args++;
        }
        uint32_t map = atoi(arg1.c_str());
        printf("Calling %s(%d)...\n", func.c_str(), map);
        Send ("GetProcMapName: "+ProcessManager::GetProcMapName(map));
    }
    if (cmd.find("GetMaxMaps") != std::string::npos)
    {
        func = "GetMaxMaps";
        printf("Calling %s()...\n", func.c_str());
        Send("GetMaxMaps: " + ProcessManager::GetMaxMaps());
    }
}