using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buzz.TxLeague.Women.Config.Utils
{
    public static class SyncEventsToDGS
    {
        public static void RecreateGamesBulkTask(string arrayOfEventIds)
        {
            var eventsIdsArray = arrayOfEventIds.Split('\u002C');
            var result = string.Empty;
            var hostname = "10.0.0.181";
            var username = "sysusr";
            var password = "Qev5?AA.";
            using (var client = new SshClient(hostname, username, password))
            {
                client.Connect();
                var iter = 0;
                foreach (var item in eventsIdsArray)
                {
                    var cmd = client.CreateCommand($"cd Test/LineshouseSnapshot; dotnet Ls-dgs-sync.dll {Int32.Parse(item)}");
                    var response = cmd.BeginExecute();

                    using (var reader =
                               new StreamReader(cmd.OutputStream, Encoding.UTF8, true, 1024, true))
                    {
                        while (!response.IsCompleted || !reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (line != null)
                            {
                                result += line + " | ";
                            }
                        }
                        iter++;
                    }

                    decimal percent = (decimal)((decimal)(iter / (decimal)eventsIdsArray.Length) * 100);
                    Console.WriteLine($"Advance: {Math.Round(percent, 2)}");

                    result.TrimEnd('|');
                }
                client.Disconnect();
            }
        }
    }
}