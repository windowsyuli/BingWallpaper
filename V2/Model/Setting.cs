using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;

namespace BingWallpaper
{
    public class Setting
    {
        public bool AutoSet { get; set; }

        public bool AutoStart { get; set; }

        public string StoreDirectory { get; set; }

        public string Region { get; set; }

        public string Size { get; set; }

        public Setting()
        {
            ReadFromRegistry();
        }

        private void ReadFromRegistry()
        {
            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Constant.SettingPath))
                {
                    AutoSet = Boolean.Parse((regKey.GetValue("AutoSet") ?? true).ToString());
                    AutoStart = Boolean.Parse((regKey.GetValue("AutoStart") ?? true).ToString());
                    StoreDirectory = (regKey.GetValue("StoreDirectory") ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constant.ProgramName)).ToString();
                    Region = (regKey.GetValue("Region") ?? "zh-cn").ToString();
                    Size = (regKey.GetValue("Size") ?? "1920x1080").ToString();
                }
            }
            catch (Exception)
            {
                SetDefaultValue();
            }
        }

        private void SetDefaultValue()
        {
            AutoSet = true;
            AutoStart = true;
            StoreDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constant.ProgramName);
            Region = "zh-cn";
            Size = "1920x1080";
        }

        public bool Save()
        {
            if (!Constant.Regions.Contains(Region))
                return false;
            try
            {
                if (!Directory.Exists(StoreDirectory))
                {
                    Directory.CreateDirectory(StoreDirectory);
                }
                using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Constant.SettingPath))
                {
                    regKey.SetValue("AutoSet", AutoSet);
                    regKey.SetValue("AutoStart", AutoStart);
                    regKey.SetValue("StoreDirectory", StoreDirectory);
                    regKey.SetValue("Region", Region);
                    regKey.SetValue("Size", Size);
                }
                if (AutoStart)
                {
                    using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Constant.AutoRunPath))
                    {
                        regKey.SetValue(Constant.ProgramName, Process.GetCurrentProcess().MainModule.FileName + " -s");
                    }
                }
                else
                {
                    using (RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Constant.AutoRunPath))
                    {
                        regKey.DeleteValue(Constant.ProgramName);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
