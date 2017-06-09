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
        private static string LocalPath => AppDomain.CurrentDomain.BaseDirectory + FileName;
        private static string RemoteUploadPath => @"/uploads/" + FileName;
        private static string RemoteStringPath => @"/string/string.txt"; //+ FileName;
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
            task = Task.Run(TaskListFiles);
            task.Wait();
            return folderContents;
        }

        private static async Task TaskListFiles()
        {
            folderContents.Clear();
            FileName = string.Empty;
            var list = await dbx.Files.ListFolderAsync(RemoteUploadPath);
            foreach (var item in list.Entries.Where(i => i.IsFile))
                folderContents.Add(item.Name);
        }

        public void Upload(string fileName)
        {
            FileName = fileName;
            task = Task.Run(TaskUpload);
            task.Wait();
        }

        private static async Task TaskUpload()
        {
            using (var fileStream = File.Open(LocalPath, FileMode.Open))
                if (fileStream.Length <= chunkSize)
                    await dbx.Files.UploadAsync(RemoteUploadPath, body: fileStream);
        }

        public void Download(string fileName)
        {
            FileName = fileName;
            task = Task.Run(TaskDownload);
            task.Wait();
        }

        private static async Task TaskDownload()
        {
            using (var response = await dbx.Files.DownloadAsync(RemoteUploadPath))
                File.WriteAllText(LocalPath, await response.GetContentAsStringAsync());
        }

        public void UploadString(string content)
        {
            Content = content;
            task = Task.Run(TaskUploadString);
            task.Wait();
        }

        private static async Task TaskUploadString()
        {
            using (var mem = new MemoryStream(Encoding.UTF8.GetBytes(Content)))
                await dbx.Files.UploadAsync(RemoteStringPath, WriteMode.Overwrite.Instance, body: mem);
        }

        public string DownloadString()
        {
            task = Task.Run(TaskDownloadString);
            task.Wait();
            return Content;
        }

        private static async Task TaskDownloadString()
        {
            using (var response = await dbx.Files.DownloadAsync(RemoteStringPath))
                Content = await response.GetContentAsStringAsync();
        }

    }
}
