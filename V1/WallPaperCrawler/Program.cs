using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml.Linq;
using System.Xml.XPath;

namespace WallPaperCrawler
{
    static class Constant
    {
        public static string Copyright = "Wallpaper Copyright YULI 2014";
        public static string StartUp = "Starting Application ...";
        public static string RegSet = "Setting Application Autorun ...";
        public static string LoadXML = "Getting XML From BaseUrl : cn.bing.com ...";
        public static string Download = "Downloading Bing wallpaper...";
        public static string SetWallpaper = "Setting Desktop Wallpaper...";
        public static string WorkDone = "Work Complete!";
        public static string Close360 = "Please close 360 ...";
        public static string TryAgain = "Program will try in 10s later.";
        public static string WIN8 = "Please use windows 8";
        public static string UnexpectedError = "UnexpectedError has orrored. Please Contact YULI! ";
        public static string RegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        public static string ProgramName = "WallPaperCrawler";
        public static string DocumentPath = "";
        public static string ImagePath = "";
        //http://global.bing.com/az/hprichbg/rb/TrafalgarSquareMenorah_EN-US9040687922_1920x1080.jpg
        public static string BaseUrl = "http://global.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&pid=hp&FORM=HPCNEN&setmkt=en-us&setlang=en-us&video=1";//"http://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&pid=hp&FORM=HPCNEN";
        //http://global.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&pid=hp&FORM=HPCNEN
        //http://global.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&pid=hp&FORM=HPCNEN&setmkt=en-us&setlang=en-us&video=1
        public static string PathUrl = "http://global.bing.com";
    }
    class Program
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        static extern Int32 SystemParametersInfo(Int32 uAction, Int32 uParam, string lpvparam, Int32 fuwinIni);
        static void logging(string str)
        {
            string content = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + " : " + str;
            Console.WriteLine(content);
            Console.WriteLine();
            Constant.DocumentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constant.ProgramName);
            if (!Directory.Exists(Constant.DocumentPath))
                Directory.CreateDirectory(Constant.DocumentPath);
            using (StreamWriter log = new StreamWriter(Path.Combine(Constant.DocumentPath, "log.txt"), true))
                log.WriteLine(content);
            Thread.Sleep(500);
        }
        static void Main(string[] args)
        {
            logging(Constant.Copyright);
            logging(Constant.StartUp);
            if (!Environment.CurrentDirectory.ToLower().Contains("system32"))
            {
                using (RegistryKey Run = Registry.CurrentUser.OpenSubKey(Constant.RegKey, true))
                {
                    string StartPath = Path.Combine(Environment.CurrentDirectory, Constant.ProgramName) + ".exe";
                    var obj = Run.GetValue("WallPaperCrawler");
                    if (obj == null || obj.ToString().ToLower().Contains("system32"))
                    {
                        try
                        {
                            Run.SetValue("WallPaperCrawler", StartPath);
                            logging(Constant.RegSet);
                        }
                        catch (Exception)
                        {
                            logging(Constant.Close360);
                        }
                    }
                }
            }
            while (true)
            {
                bool exit = false;
                WebResponse response = null;
                try
                {
                    logging(Constant.LoadXML);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Constant.BaseUrl);
                    request.Method = "GET";
                    request.Timeout = 1000;
                    response = request.GetResponse();
                    Stream xstream = response.GetResponseStream();
                    XDocument doc = null;
                    try
                    {
                        doc = XDocument.Load(xstream);
                    }
                    catch (Exception ex)
                    {
                        logging(ex.ToString());
                        Console.Clear();
                        logging(Constant.TryAgain);
                        exit = false;
                    }


                    logging(Constant.Download);
                    string ImageUrl = Constant.PathUrl + doc.XPathSelectElement("/images/image/url").Value.Replace("1366x768", "1920x1080");
                    string CopyRight = doc.XPathSelectElement("/images/image/copyright").Value;
                    if (CopyRight.Contains('('))
                        CopyRight = CopyRight.Split('(')[0];
                    Constant.ImagePath = Path.Combine(Constant.DocumentPath, "Images");
                    if (!Directory.Exists(Constant.ImagePath))
                        Directory.CreateDirectory(Constant.ImagePath);
                    string StorePath = Path.Combine(Constant.ImagePath, CopyRight) + ".jpg";
                    if (!File.Exists(StorePath))
                        new WebClient().DownloadFile(ImageUrl, StorePath);
                    //////
                    //string BMPFiles = “D://BMP//qq.bmp”;
                    Image img = Image.FromFile(StorePath);
                    string destPath = Path.Combine(Constant.ImagePath, CopyRight) + ".bmp";
                    //File.Create(destPath);//create dest picture as path u want
                    using (MemoryStream memStream = new MemoryStream())//create a instance of memorystream
                    {
                        img.Save(memStream, ImageFormat.Bmp);//save stream from origin pic as format bmp
                        Bitmap destBmp = new Bitmap(memStream);//create bmp instance from stream above
                        destBmp.Save(destPath);//save bmp to the path you want
                    }
                    logging(Constant.SetWallpaper);
                    if (SystemParametersInfo(20, 0, destPath, 1) != 0)
                        logging(Constant.WorkDone);
                    else
                        logging(Constant.WIN8);
                    exit = true;
                }
                catch (WebException ex)
                {
                    logging(ex.ToString());
                    Console.Clear();
                    logging(Constant.TryAgain);
                    exit = false;
                }
                catch (Exception ex)
                {
                    logging(ex.ToString());
                    Console.Clear();
                    logging(Constant.UnexpectedError);
                    exit = true;
                }
                finally
                {
                    if (response != null)
                        response.Close();
                }

                if (exit)
                    break;
                Thread.Sleep(10000);
            }
        }
    }
}
