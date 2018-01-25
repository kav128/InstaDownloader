using Newtonsoft.Json.Linq;
using System;

namespace InstaDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            string PostId = Console.ReadLine();
            string jsonraw = InstaParser.GetJsonFromPost(PostId);
            JToken obj = JObject.Parse(jsonraw)["entry_data"]["PostPage"][0]["graphql"]["shortcode_media"];
            DownloadPost(obj, "F:\\Media\\InstaDownload");
            Console.ReadLine();
        }

        private static void DownloadPost(JToken token, string Path = "")
        {
            ContentType contentType = InstaParser.GetContentType(token);
            string url;
            switch (contentType)
            {
                case ContentType.GraphImage:
                    url = InstaParser.GetPhotoUrl(token);
                    Console.Write("Downloading photo...");
                    FileDownloader.Download(url, Path);
                    Console.WriteLine("\tCompleted");
                    break;
                case ContentType.GraphVideo:
                    url = InstaParser.GetVideoUrl(token);
                    Console.Write("Downloading video...");
                    FileDownloader.Download(url, Path);
                    Console.WriteLine("\tCompleted");
                    break;
                case ContentType.GraphSidecar:
                    JToken sidecarToken = token["edge_sidecar_to_children"]["edges"];
                    string[] urls = InstaParser.GetSidecarUrls(sidecarToken);
                    for (int i = 0; i < urls.Length; i++)
                    {
                        Console.Write("Downloading item {0}/{1}...", i + 1, urls.Length);
                        FileDownloader.Download(urls[i], Path);
                        Console.WriteLine("\tCompleted");
                    }
                    break;
            }
            Console.WriteLine("Downloading completed");
        }
    }
}
