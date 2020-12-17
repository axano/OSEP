using System;
using System.Data.SqlClient;

namespace ConsoleApp1
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
            String query = "SELECT distinct b.name FROM sys.server_permissions a INNER JOIN sys.server_principals b ON a.grantor_principal_id = b.principal_id WHERE a.permission_name = 'IMPERSONATE';";
            SqlCommand command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read() == true) { Console.WriteLine("Logins that canbe impersonated: " + reader[0]); }
        }
    }
}
