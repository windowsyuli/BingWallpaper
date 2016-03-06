using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace BingWallpaper
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private State _state;

        private Thread _downloadThread;

        public MainWindow()
        {
            InitializeComponent();
            SettingCtrl.InitializeParameter(this);
            ImageCtrl.InitializeParameter(SettingCtrl.SettingInstant, this);
            _downloadThread = ImageCtrl.AsyncDownload();
            StateSwitch(State.Busy);
            AsyncWaiting(true);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton.Equals(MouseButtonState.Pressed))
            {
                this.DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (!ImageCtrl.CheckCurrent(true))
            {
                StateSwitch(State.Busy);
                AsyncWaiting(true);
            }
            else
            {
                StateSwitch(State.BeforeSlide);
                ImageCtrl.LoadLeftImage();
            }
        }

        private void BtnRight_Click(object sender, RoutedEventArgs e)
        {
            if (!ImageCtrl.CheckCurrent(false))
            {
                StateSwitch(State.Busy);
                AsyncWaiting(false);
            }
            else
            {
                StateSwitch(State.BeforeSlide);
                ImageCtrl.LoadRightImage();
            }
        }

        private void AsyncWaiting(bool flag)
        {
            new Thread(new ParameterizedThreadStart(WaitingThread)).Start(flag);
        }

        private void WaitingThread(object flag)
        {
            while (true)
            {
                if (_downloadThread.ThreadState == ThreadState.Stopped)
                {
                    _downloadThread = ImageCtrl.AsyncDownload();
                }
                if (ImageCtrl.CheckCurrent((bool)flag))
                {
                    this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        if ((bool)flag)
                        {
                            BtnLeft_Click(null, null);
                        }
                        else
                        {
                            BtnRight_Click(null, null);
                        }
                    }));
                    break;
                }
                Thread.Sleep(100);
            }
        }

        public void CheckButton()
        {
            if (ImageCtrl.CheckMax())
                BtnLeft.Visibility = Visibility.Hidden;
            else
                BtnLeft.Visibility = Visibility.Visible;
            if (ImageCtrl.CheckMin())
                BtnRight.Visibility = Visibility.Hidden;
            else
                BtnRight.Visibility = Visibility.Visible;
            BtnSetting.Visibility = Visibility.Visible;
            BtnOK.Visibility = Visibility.Visible;
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            StateSwitch(State.BeforeSetting);
            SettingCtrl.Show();
        }

        public void StateSwitch(State state)
        {
            switch (state)
            {
                case State.Busy:
                    BtnSetting.Visibility = Visibility.Hidden;
                    BtnLeft.Visibility = Visibility.Hidden;
                    BtnRight.Visibility = Visibility.Hidden;
                    BtnOK.Visibility = Visibility.Hidden;
                    ImageCtrl.Visibility = Visibility.Collapsed;
                    SettingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Visibility = Visibility.Visible;
                    WaitingCtrl.Start();
                    break;
                case State.BeforeSetting:
                    BtnLeft.Visibility = Visibility.Hidden;
                    BtnRight.Visibility = Visibility.Hidden;
                    BtnSetting.Visibility = Visibility.Hidden;
                    BtnOK.Visibility = Visibility.Hidden;
                    ImageCtrl.Visibility = Visibility.Visible;
                    SettingCtrl.Visibility = Visibility.Visible;
                    WaitingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Stop();
                    break;
                case State.Setting:
                    BtnSetting.Visibility = Visibility.Hidden;
                    BtnLeft.Visibility = Visibility.Hidden;
                    BtnRight.Visibility = Visibility.Hidden;
                    BtnOK.Visibility = Visibility.Visible;
                    ImageCtrl.Visibility = Visibility.Visible;
                    SettingCtrl.Visibility = Visibility.Visible;
                    WaitingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Stop();
                    break;
                case State.BeforeSlide:
                    BtnLeft.Visibility = Visibility.Hidden;
                    BtnRight.Visibility = Visibility.Hidden;
                    BtnSetting.Visibility = Visibility.Hidden;
                    BtnOK.Visibility = Visibility.Hidden;
                    ImageCtrl.Visibility = Visibility.Visible;
                    SettingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Stop();
                    break;
                default:
                    BtnSetting.Visibility = Visibility.Visible;
                    BtnLeft.Visibility = Visibility.Visible;
                    BtnRight.Visibility = Visibility.Visible;
                    BtnOK.Visibility = Visibility.Visible;
                    ImageCtrl.Visibility = Visibility.Visible;
                    SettingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Visibility = Visibility.Collapsed;
                    WaitingCtrl.Stop();
                    break;
            }
            _state = state;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (_state.Equals(State.Setting))
            {
                ImageCtrl.Visibility = Visibility.Visible;
                SettingCtrl.Hide();
                //Download now
                ImageCtrl.InitializeParameter(SettingCtrl.SettingInstant, this);
                _downloadThread = ImageCtrl.AsyncDownload();
                StateSwitch(State.Busy);
                AsyncWaiting(true);
            }
            else
            {
                ImageCtrl.SetWallpaper();
            }
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            LabelCopyright.Opacity = 1;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            LabelCopyright.Opacity = 0;
        }
    }
}
