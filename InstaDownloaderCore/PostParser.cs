using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace InstaCore
{
    public enum ContentType
    {
        GraphImage, GraphVideo, GraphSidecar
    }

    public static class PostParser
    {
        public static string GetJsonFromPost(string PostUrl)
        {
            string result;
            var req = (HttpWebRequest)WebRequest.Create(PostUrl);
            req.AllowAutoRedirect = false;
            using (var rsp = req.GetResponse())
            {
                using (var stream = rsp.GetResponseStream())
                {
                    HtmlDocument hdoc = new HtmlDocument();
                    hdoc.Load(stream);
                    HtmlNodeCollection hnc = hdoc.DocumentNode.SelectNodes("/*/body/script");
                    result = hnc[0].InnerText;
                }
            }
            return result.Remove(result.Length - 1, 1).Remove(0, result.IndexOf('{'));
        }

        public static ContentType GetContentType(JToken token)
        {
            string cType = (string)token["__typename"];
            switch (cType)
            {
                case "GraphVideo":
                    return ContentType.GraphVideo;
                case "GraphSidecar":
                    return ContentType.GraphSidecar;
                default:
                    return ContentType.GraphImage;
            }
        }

        public static string GetPhotoUrl(JToken token)
        {
            return (string)token["display_url"];
        }

        public static string GetVideoUrl(JToken token)
        {
            return (string)token["video_url"];
        }

        public static string[] GetSidecarUrls(JToken SidecarToken)
        {
            List<string> urls = new List<string>();
            foreach (var item in SidecarToken)
            {
                ContentType curContentType = GetContentType(item["node"]);
                if (curContentType == ContentType.GraphImage)
                {
                    urls.Add(GetPhotoUrl(item["node"]));
                }
                if (curContentType == ContentType.GraphVideo)
                {
                    urls.Add(GetVideoUrl(item["node"]));
                }
            }
            return urls.ToArray();
        }
    }
}
