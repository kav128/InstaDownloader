using Newtonsoft.Json.Linq;

namespace InstaCore
{
    public struct DisplayResources
    {
        public readonly string _640pUrl;
        public readonly string _750pUrl;
        public readonly string _1080pUrl;

        public DisplayResources(JToken displayResourcesToken)
        {
            _640pUrl = (string)displayResourcesToken[0]["src"];
            _750pUrl = (string)displayResourcesToken[1]["src"];
            _1080pUrl = (string)displayResourcesToken[2]["src"];
        }
    }

    public abstract class InstaMedia
    {
        public string DisplayUrl { get; private set; }
        public DisplayResources DisplayResources { get; private set; }

        public InstaMedia(JToken mediaToken)
        {
            DisplayUrl = (string)mediaToken["display_url"];
            DisplayResources = new DisplayResources(mediaToken["display_resources"]);
        }
    }
}
