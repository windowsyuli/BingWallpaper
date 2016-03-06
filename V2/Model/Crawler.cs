using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using System.Xml.XPath;

namespace BingWallpaper
{
    class Crawler
    {
        private XDocument _imageXml;

        private string _region;

        private string _storeDirectory;

        private string _size;

        public Crawler(Setting s)
        {
            _region = s.Region;
            _storeDirectory = s.StoreDirectory;
            if (!Directory.Exists(_storeDirectory))
            {
                try
                {
                    Directory.CreateDirectory(_storeDirectory);
                }
                catch (Exception)
                {
                    _storeDirectory = ".\\";
                }
            }
            _size = s.Size;
        }

        public string Download(int wallPaperIndex)
        {

            string uri = Constant.BaseUri + Constant.XMLUri + "&idx=" + wallPaperIndex.ToString() + "&n=1" + "&setmkt=" + _region + "&setlang=" + _region;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            request.Timeout = 2000;
            using (WebResponse response = request.GetResponse())
            {
                Stream xstream = response.GetResponseStream();
                _imageXml = XDocument.Load(xstream);
            }
            string name = ParseName();
            DownloadVideo(name);
            return DownloadImage(name);
        }

        private string ParseName()
        {
            XElement copyrightEle = _imageXml.XPathSelectElement(Constant.CopyrightPath);
            if (copyrightEle == null)
                return Guid.NewGuid().ToString();
            string nameTmp = copyrightEle.Value.Split('(')[0];
            foreach (char c in Constant.Spechars)
                nameTmp = nameTmp.Replace(c,'-');
            return nameTmp.Trim();
        }

        private void DownloadVideo(string name)
        {
            XElement videoEle = _imageXml.XPathSelectElement(Constant.VideoPath);
            if (videoEle == null)
                return;
            Downloader("https://" + videoEle.Value.Trim('/'), Path.Combine(_storeDirectory, name + ".mp4"));
        }

        private string DownloadImage(string name)
        {
            XElement imageEle = _imageXml.XPathSelectElement(Constant.ImagePath);
            if (imageEle == null)
                return string.Empty;
            string imageUrl = Constant.BaseUri + imageEle.Value.Replace("1366x768", _size);
            string storePath = Path.Combine(_storeDirectory, name + ".jpg");
            Downloader(imageUrl, storePath);
            return storePath;
        }

        private void Downloader(string url, string storePath)
        {
            if (File.Exists(storePath))
                return;
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(url, storePath);
            }
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]

        static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, string lpvparam, Int32 fuwinIni);

        public void SetWallpaper(string filePath)
        {
            Image img = Image.FromFile(filePath);
            string bitmapPath = Path.Combine(System.Environment.GetEnvironmentVariable("TEMP"), Guid.NewGuid() + ".bmp");
            using (MemoryStream memStream = new MemoryStream())
            {
                img.Save(memStream, ImageFormat.Bmp);
                Bitmap Bmp = new Bitmap(memStream);
                Bmp.Save(bitmapPath);
            }
            int a = SystemParametersInfo(20, 0, bitmapPath, 1);
        }
    }
}
