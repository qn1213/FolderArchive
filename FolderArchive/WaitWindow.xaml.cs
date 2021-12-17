using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;

namespace FolderArchive
{
    /// <summary>
    /// WaitWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WaitWindow : Window
    {
        private PerformanceCounter ram = new PerformanceCounter("Memory", "Available MBytes");
        private static double angle = 0.0;
        public WaitWindow()
        {
            InitializeComponent();

            t_ram.Content = ram.NextValue().ToString() + "MB";

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromTicks(250000);   // ticks 10000000 = 1초
            timer.Tick += new EventHandler(tick);
            timer.Start();

            //DoWork();
        }

        private void DoWork()
        {
            //while (true)
            //{

            //}
        }

        private void tick(object sender, EventArgs e)
        {
            t_proccc.RenderTransform = new RotateTransform(angle);
            angle += 10.0f;
            if (angle >= 360.0)
                angle = 0;

            t_ram.Content = ram.NextValue().ToString() + "MB";
        }
    }
}
