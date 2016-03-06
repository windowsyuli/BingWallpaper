using System.Net;

namespace BingWallpaper
{
    public class EasyStart
    {
        public static void Process()
        {
            bool flag = true;
            while(flag)
            {
                try
                {
                    Setting s = new Setting();
                    Crawler c = new Crawler(s);
                    string storePath = c.Download(0);
                    c.SetWallpaper(storePath);
                    flag = false;
                }
                catch(WebException)
                {
                    flag = true;
                }
            }
        }
    }
}
