using Newtonsoft.Json.Linq;
using System;

namespace InstaDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            string PostId = Console.ReadLine();
            string jsonraw = PostParser.GetJsonFromPost(PostId);
            JToken obj = JObject.Parse(jsonraw)["entry_data"]["PostPage"][0]["graphql"]["shortcode_media"];
            DownloadPost(obj, "F:\\Media\\InstaDownload");
            Console.ReadLine();
        }

        private static void DownloadPost(JToken token, string Path = "")
        {
            ContentType contentType = PostParser.GetContentType(token);
            string url;
            switch (contentType)
            {
                case ContentType.GraphImage:
                    url = PostParser.GetPhotoUrl(token);
                    Console.Write("Downloading photo...");
                    FileDownloader.Download(url, Path);
                    Console.WriteLine("\tCompleted");
                    break;
                case ContentType.GraphVideo:
                    url = PostParser.GetVideoUrl(token);
                    Console.Write("Downloading video...");
                    FileDownloader.Download(url, Path);
                    Console.WriteLine("\tCompleted");
                    break;
                case ContentType.GraphSidecar:
                    JToken sidecarToken = token["edge_sidecar_to_children"]["edges"];
                    string[] urls = PostParser.GetSidecarUrls(sidecarToken);
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
