using System.IO;
using System.Net;

namespace InstaDownloader
{
    static class FileDownloader
    {
        public static void Download(string URL, string Dest)
        {
            string[] splURL = URL.Split('/');
            string filename = splURL[splURL.Length - 1];
            if (!Directory.Exists(Dest))
                Directory.CreateDirectory(Dest);
            using (WebClient wc = new WebClient())
                wc.DownloadFile(URL, Dest + (Dest.EndsWith("\\") || Dest.Length == 0 ? "" : "\\") + filename);
        }
    }
}
