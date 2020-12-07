# Proxies
## Remap reg hive
```
New-PSDrive -Name HKU -PSProvider Registry -Root HKEY_USERS | Out-Null
```
