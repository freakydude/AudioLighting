using AudioLighting.Views;
using System;
using System.ComponentModel;
using System.Windows;

namespace AudioLighting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private System.Windows.Forms.NotifyIcon notifyIcon;
        private bool isExit;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;
            MainWindow.Show();

            //notifyIcon = new System.Windows.Forms.NotifyIcon();
            //notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            //notifyIcon.Icon = Analyzer.Properties.Resources.AppIcon;
            //notifyIcon.Visible = true;

            //CreateContextMenu2();
            //ShowMainWindow();
        }

        private void CreateContextMenu2()
        {
            //var m = new System.Windows.Forms.ContextMenu();
            //m.MenuItems.Add("Dashboard").Click += (s, e) => ShowMainWindow();
            //m.MenuItems.Add("Enable All").Click += (s, e) => MyUtils.EnableAll();
            //m.MenuItems.Add("Disable All").Click += (s, e) => MyUtils.DisableAll();
            //var mItems = new List<System.Windows.Forms.MenuItem>();
            //for (var i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            //{
            //    var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
            //    if (device.IsEnabled && device.IsLoopback)
            //    {
            //        var x = new System.Windows.Forms.MenuItem(string.Format("{0} - {1}", i, device.name), AudioSwitching);
            //        mItems.Add(x);
            //    }
            //}
            //var mi = new System.Windows.Forms.MenuItem("Audio Device", mItems.ToArray());

            //m.MenuItems.Add(mi);
            ////tsddb.DropDown = _notifyIcon.ContextMenuStrip;


            //m.MenuItems.Add("Exit").Click += (s, e) => ExitApplication();
            //notifyIcon.ContextMenu = m;
        }

        public void ExitApplication()
        {
            isExit = true;
            MainWindow.Close();
            //notifyIcon.Visible = false;
            //notifyIcon.Dispose();
            //notifyIcon = null;
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void AudioSwitching(object sender, EventArgs e)
        {
            //var element = sender as System.Windows.Forms.MenuItem;
            //MyUtils.SwitchDeviceFromString(element.Text);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }
    }
}