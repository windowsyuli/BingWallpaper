using System.Collections.Generic;

namespace BingWallpaper
{
    class Constant
    {
        public static string ProgramName = "BingWallpaper";

        public static string BaseUri = "http://global.bing.com";

        public static string XMLUri = "/HPImageArchive.aspx?format=xml&pid=hp&FORM=HPCNEN&video=1";

        public static IList<string> Regions = new List<string>()
        {
            "en-gb",
            "en-us",
            "fr-ca",
            "zh-cn",
            "ja-jp",
            "ru-ru",
            "fi-fi",
            "ko-kr",
        };

        public static IList<string> Sizes = new List<string>()
        {
            "1920x1080",
            "1366x768",
        };

        public static string VideoPath = "/images/image/video/codecs/codec/url";

        public static string CopyrightPath = "/images/image/copyright";

        public static string ImagePath = "/images/image/url";

        public static string AutoRunPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static string SettingPath = @"SOFTWARE\BingWallPaper";

        public static char[] Spechars = new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };
    }

    public enum State
    {
        Busy,
        Setting,
        BeforeSlide,
        BeforeSetting,
        Normal
    }

}
