using Newtonsoft.Json.Linq;

namespace InstaDownloader
{
    class InstaPhoto: InstaMedia
    {
        public InstaPhoto(JToken token): base(token)
        {

        }
    }
}
