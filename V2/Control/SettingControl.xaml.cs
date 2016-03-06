using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BingWallpaper
{
    /// <summary>
    /// SettingControl.xaml 的交互逻辑
    /// </summary>
    public partial class SettingControl : UserControl
    {
        private Storyboard _stb;

        public Setting SettingInstant;

        private MainWindow _mainWindow;

        public SettingControl()
        {
            InitializeComponent();
        }

        public void InitializeParameter(MainWindow mainWindow)
        {
            XGrid.OpacityMask = this.Resources["OpenBrush"] as LinearGradientBrush;
            CB_Size.ItemsSource = Constant.Sizes;
            CB_Region.ItemsSource = Constant.Regions;
            SettingInstant = new Setting();
            XGrid.DataContext = SettingInstant;
            _mainWindow = mainWindow;
        }

        public void Show()
        {
            XGrid.OpacityMask = this.Resources["OpenBrush"] as LinearGradientBrush;
            _stb = this.Resources["OpenBoard"] as Storyboard;
            _stb.Completed += (s, e) =>
            {
                if (_stb == null)
                    return;
                _stb.Remove();
                _stb = null;
                XGrid.OpacityMask = null;
                _mainWindow.StateSwitch(State.Setting);
            };
            _stb.Begin();
        }

        public void Hide()
        {
            XGrid.OpacityMask = this.Resources["CloseBrush"] as LinearGradientBrush;
            _stb = this.Resources["CloseBoard"] as Storyboard;
            _stb.Completed += (s, e) =>
            {
                if (_stb == null)
                    return;
                _stb.Remove();
                _stb = null;
                XGrid.OpacityMask = this.Resources["OpenBrush"] as LinearGradientBrush;
                SettingInstant.Save();
                _mainWindow.StateSwitch(State.Normal);
            };
            _stb.Begin();
        }

        private void ChangePath_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.ShowDialog();
            if (fbd.SelectedPath != string.Empty)
            {
                SettingInstant.StoreDirectory = SavedPath.Text = fbd.SelectedPath;
            }
        }
    }
}
