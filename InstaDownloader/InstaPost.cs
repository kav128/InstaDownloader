using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace InstaDownloader
{
    class InstaPost
    {
        public InstaPost(string url)
        {
            PostUrl = url;
            Refresh();
        }

        public void Refresh()
        {
            jsonData = PostParser.GetJsonFromPost(PostUrl);
            rootToken = JObject.Parse(jsonData);
            JToken shortcodeMediaToken = rootToken["entry_data"]["PostPage"][0]["graphql"]["shortcode_media"];

            string typename = (string)shortcodeMediaToken["__typename"];
            if (typename == "GraphSidecar")
            {
                List<InstaMedia> SidecarList = new List<InstaMedia>();
                foreach (JToken item in shortcodeMediaToken["edge_sidecar_to_children"]["edges"])
                    SidecarList.Add(ParseSimpleMedia(item["node"]));
                SidecarData = SidecarList.ToArray();
            }
            else
                SidecarData = new InstaMedia[] { ParseSimpleMedia(shortcodeMediaToken) };
        }

        private InstaMedia ParseSimpleMedia(JToken mediaToken)
        {
            string typename = (string)mediaToken["__typename"];
            switch (typename)
            {
                case "GraphImage":
                    return new InstaPhoto(mediaToken);
                case "GraphVideo":
                    return new InstaVideo(mediaToken);
                default:
                    throw new System.Exception("Invalid media type");
            }
        }

        public readonly string PostUrl;
        private string jsonData;
        private JToken rootToken;
        public ContentType ContentType { get; private set; }

        public InstaMedia[] SidecarData { get; private set; }
    }
}
