using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;

namespace Backgammon.AI_Networking
{
    internal class NetworkingInstance
    {
        public bool AwaitingTask => task.IsCompleted;
        private Task task;
        private const int chunkSize = 4096 * 1024;
        private static DropboxClient dbx;
        private static string FileName, Content;
        private static string localPath => AppDomain.CurrentDomain.BaseDirectory + FileName;
        private static string remoteUploadPath => @"/uploads/" + FileName;
        private static string remoteStringPath => @"/string/string.txt"; //+ FileName;
        private static List<string> folderContents;

        private static NetworkingInstance instance;
        internal static NetworkingInstance Instance
        {
            get
            {
                if (instance == null)
                    instance = new NetworkingInstance();
                return instance;
            }
        }

        private NetworkingInstance()
        {// Make constructor private.
            dbx = new DropboxClient("NVeItGV0k-AAAAAAAAAAJVArSdsLd88exx0nPCmSQbmQV4vX7dHcPqsLn0GuNkc0");
            folderContents = new List<string>();
        }

        private void TestNetworking()
        {
            var breakpoint = string.Empty;
            NetworkingInstance.Instance.UploadString("dank 420 blashito");
            breakpoint = string.Empty;
            Console.WriteLine(NetworkingInstance.Instance.DownloadString());
            breakpoint = string.Empty;
            NetworkingInstance.Instance.Upload("Complaint.txt");
            breakpoint = string.Empty;
            NetworkingInstance.Instance.Download("Complaint.txt");
            breakpoint = string.Empty;
            Console.WriteLine(string.Join(",", NetworkingInstance.Instance.ListFiles()));
            breakpoint = string.Empty;
        }

        public List<string> ListFiles()
        {
            task = Task.Run(listFiles);
            task.Wait();
            return folderContents;
        }

        private static async Task listFiles()
        {
            folderContents.Clear();
            FileName = string.Empty;
            var list = await dbx.Files.ListFolderAsync(remoteUploadPath);
            foreach (var item in list.Entries.Where(i => i.IsFile))
                folderContents.Add(item.Name);
        }

        public void Upload(string fileName)
        {
            FileName = fileName;
            task = Task.Run(upload);
            task.Wait();
        }

        private static async Task upload()
        {
            using (var fileStream = File.Open(localPath, FileMode.Open))
                if (fileStream.Length <= chunkSize)
                    await dbx.Files.UploadAsync(remoteUploadPath, body: fileStream);
        }

        public void Download(string fileName)
        {
            FileName = fileName;
            task = Task.Run(download);
            task.Wait();
        }

        private static async Task download()
        {
            using (var response = await dbx.Files.DownloadAsync(remoteUploadPath))
                File.WriteAllText(localPath, await response.GetContentAsStringAsync());
        }

        public void UploadString(string content)
        {
            Content = content;
            task = Task.Run(uploadString);
            task.Wait();
        }

        private static async Task uploadString()
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(Content)))
                await dbx.Files.UploadAsync(remoteStringPath, WriteMode.Overwrite.Instance, body: mem);
        }

        public string DownloadString()
        {
            task = Task.Run(downloadString);
            task.Wait();
            return Content;
        }

        private static async Task downloadString()
        {
            using (var response = await dbx.Files.DownloadAsync(remoteStringPath))
                Content = await response.GetContentAsStringAsync();
        }

    }
}
