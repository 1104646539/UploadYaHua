using System;
using System.Threading;
using System.Windows;

namespace uploadyahua
{
    public partial class App : Application
    {
        private static Mutex _mutex = new Mutex(true, "{8F6F0AC4-B9A1-45FD-A8CF-72F04E6BDE8F}"); // 唯一GUID

        protected override void OnStartup(StartupEventArgs e)
        {
            if (_mutex.WaitOne(TimeSpan.Zero, true))
            {
                base.OnStartup(e);
            }
            else
            {
                // 已有实例运行，提示并退出
                MessageBox.Show("应用程序已在运行！");
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _mutex.ReleaseMutex();
            base.OnExit(e);
        }
    }
}
