using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BingWallpaper
{
    /// <summary>
    /// WaitingControl.xaml 的交互逻辑
    /// </summary>
    public partial class WaitingControl : UserControl
    {
        private Storyboard _stb;
        public WaitingControl()
        {
            InitializeComponent();
            _stb = Resources["RingStoryboard"] as Storyboard;
        }

        public void Start()
        {
            _stb.Begin();
        }

        public void Stop()
        {
            _stb.Stop();
        }
    }
}
