using System.Net;

namespace InstaDownloader
{
    static class FileDownloader
    {
        public static void Download(string URL, string Dest)
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(URL, Dest);
            }
        }
    }
}
