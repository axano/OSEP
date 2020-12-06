Private Declare Function GetUserName Lib "advapi32.dll" Alias "GetUserNameA" (ByVal lpBuffer As String, ByRef nSize As Long) As Long


Private Declare Function msg Lib "user32.dll" Alias "MessageBoxA" (ByVal hwnd As Integer, ByVal lpText As String, ByVal lpCaption As String, ByVal hwnd As Integer) As Integer

Function MyMacro()
    Dim res As Long
    Dim MyBuff As String * 256
    Dim MySize As Long
    Dim strlen As Long
    MySize = 256
    res = GetUserName(MyBuff, MySize)
    strlen = InStr(1, MyBuff, vbNullChar) - 1
    MsgBox Left$(MyBuff, strlen)
    msg 0, "test", "test", 4
    End Function
    


Sub AutoOpen()
    MyMacro
End Sub
