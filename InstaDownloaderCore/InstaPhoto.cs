using Newtonsoft.Json.Linq;

namespace InstaCore
{
    public class InstaPhoto : InstaMedia
    {
        public InstaPhoto(JToken token) : base(token)
        {

        }
    }
}
