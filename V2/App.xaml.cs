using System;
using System.Windows;

namespace BingWallpaper
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow win = new MainWindow();
            MainWindow = win;
            win.Show();
        }
    }

    //<summary>
    //Entry point class to handle single instance of the application
    //</summary>
    public static class EntryPoint
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                EasyStart.Process();
            }
            else
            {
                App app = new App();
                app.Run();
            }
        }
    }
}
