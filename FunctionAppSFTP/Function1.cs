using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Threading.Tasks;

using Azure.Storage.Files.Shares;


namespace FunctionAppSFTP
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([TimerTrigger("0 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var x = GetShareFilesAsync();
            x.Wait();

            Console.WriteLine("Done!");
        }

        static async Task GetShareFilesAsync(string dirName = "")
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName="+ Environment.GetEnvironmentVariable("AccountName") + ";AccountKey="+Environment.GetEnvironmentVariable("AccountKey");

            // Name of the share, directory
            string shareName = "uploadfileshare-jalex";

             Console.WriteLine("Instantiating file client.");

            // Get a reference to a share
            ShareClient share = new ShareClient(connectionString, shareName);

            ShareDirectoryClient directory = share.GetDirectoryClient(dirName);
            var files = directory.GetFilesAndDirectories();

            foreach (var file in files)
            {
                if (file.IsDirectory)
                {
                    Console.WriteLine("Folder: " + Path.Combine(dirName, file.Name));
                    await GetShareFilesAsync(Path.Combine(dirName, file.Name));

                }

                Console.WriteLine("File:" + Path.Combine(dirName, file.Name));
            }
        }
    }
}
