using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BingWallpaper
{
    /// <summary>
    /// ImageControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImageControl : UserControl
    {
        private int _maxCount;

        private int _clickCount;

        private int _current;

        private List<string> _imagePathList = new List<string>();

        private Crawler _crawler;

        private Storyboard _stb;

        private MainWindow _mainWindow;

        private bool _autoSet;

        public ImageControl()
        {
            InitializeComponent();
        }

        public void InitializeParameter(Setting s, MainWindow mainWindow)
        {
            _maxCount = 7;
            _clickCount = 0;
            _current = -1;
            _imagePathList = new List<string>();
            _crawler = new Crawler(s);
            _autoSet = s.AutoSet;
            _mainWindow = mainWindow;
        }

        public Thread AsyncDownload()
        {
            Thread t = new Thread(new ThreadStart(Download));
            t.Start();
            return t;
        }

        public void Download()
        {
            for (int i = 0; i < _maxCount; ++i)
            {
                bool flag = true;
                while (flag)
                {
                    try
                    {
                        string storePath = _crawler.Download(i);
                        _imagePathList.Add(storePath);
                        flag = false;
                        if (_autoSet && i == 0)
                            _crawler.SetWallpaper(storePath);
                    }
                    catch (WebException)
                    {
                        flag = true;
                    }
                }
            }
        }

        public void LoadLeftImage()
        {
            Image2.Source = Image1.Source;
            Image1.OpacityMask = this.Resources["OpenBrush"] as LinearGradientBrush;
            Image1.Source = new BitmapImage(new Uri(_imagePathList[_current + 1], UriKind.RelativeOrAbsolute));
            _stb = this.Resources["OpenBoard"] as Storyboard;
            _stb.Completed += (s, e) =>
            {
                if (_stb == null)
                    return;
                _stb.Remove();
                _stb = null;
                Image1.OpacityMask = null;
                _current++;
                _mainWindow.StateSwitch(State.Normal);
                _mainWindow.CheckButton();
            };
            _stb.Begin();
        }

        public void LoadRightImage()
        {
            Image2.Source = new BitmapImage(new Uri(_imagePathList[_current - 1], UriKind.RelativeOrAbsolute));
            Image1.OpacityMask = this.Resources["CloseBrush"] as LinearGradientBrush;
            _stb = this.Resources["CloseBoard"] as Storyboard;
            _stb.Completed += (s, e) =>
            {
                if (_stb == null)
                    return;
                _stb.Remove();
                _stb = null;
                Image1.Source = Image2.Source;
                Image1.OpacityMask = null;
                _current--;
                _mainWindow.StateSwitch(State.Normal);
                _mainWindow.CheckButton();
            };
            _stb.Begin();
        }

        public bool CheckMax()
        {
            return _current.Equals(_maxCount - 1);
        }

        public bool CheckMin()
        {
            return _current.Equals(0);
        }

        public bool CheckCurrent(bool flag)
        {
            int index = _current;
            if (flag)
                index = _current + 1;
            else
                index = _current - 1;
            try
            {
                if (File.Exists(_imagePathList[index]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        public void SetWallpaper()
        {
            if (_current >= 0 && _imagePathList.Count - 1 >= _current && File.Exists(_imagePathList[_current]))
                _crawler.SetWallpaper(_imagePathList[_current]);
        }

        private void DoubleMouseDown(object sender, MouseButtonEventArgs e)
        {
            _clickCount += 1;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            timer.Tick += (s, e1) =>
            {
                timer.IsEnabled = false;
                _clickCount = 0;
            };
            timer.IsEnabled = true;
            if (_clickCount % 2 == 0)
            {
                timer.IsEnabled = false;
                _clickCount = 0;
                if (_current >= 0 && _imagePathList.Count - 1 >= _current && File.Exists(_imagePathList[_current]))
                {
                    string filePath = _imagePathList[_current];
                    if (File.Exists(filePath.Replace(".jpg", ".mp4")))
                        Process.Start(filePath.Replace(".jpg", ".mp4"));
                    else
                        Process.Start(filePath);
                }
            }
        }
    }
}
