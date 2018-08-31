using Newtonsoft.Json.Linq;

namespace InstaDownloader
{
    class InstaVideo: InstaMedia
    {
        public InstaVideo(JToken token): base(token)
        {
            VideoUrl = (string)token["video_url"];
        }

        public string VideoUrl { get; private set; }
    }
}
