using InstaCore;
using System;

namespace InstaDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            string PostUrl = Console.ReadLine();
            String Path = "InstaDownload";
            InstaPost post = new InstaPost(PostUrl);
            DownloadPost(post, Path);
            Console.ReadLine();
        }

        private static void DownloadPost(InstaPost post, string Path)
        {
            for (int i = 0; i < post.SidecarData.Length; i++)
            {
                InstaMedia item = post.SidecarData[i];
                Console.Write("Downloading item {0}/{1}...", i + 1, post.SidecarData.Length);
                if (item is InstaPhoto)
                    FileDownloader.Download((item as InstaPhoto).DisplayUrl, Path);
                else
                    FileDownloader.Download((item as InstaVideo).VideoUrl, Path);
                Console.WriteLine("\tCompleted");
            }

            Console.WriteLine("Downloading completed");
        }
    }
}
