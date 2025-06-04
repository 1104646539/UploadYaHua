using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using uploadyahua.Util;

namespace uploadyahua
{
    public partial class App : Application
    {
        private static Mutex _mutex = new Mutex(false, "{8F6F0AC4-B9A1-45FD-A8CF-72F04E6BDE8F}"); // 唯一GUID

        protected override void OnStartup(StartupEventArgs e)
        {
            
            // 尝试获取互斥体，如果获取成功，表示当前是第一个实例
            if (_mutex.WaitOne(TimeSpan.FromSeconds(1), false))
            {
                try
                {
                    base.OnStartup(e);
                    if (IsStartupLaunch())
                    {
                        SystemGlobal.Statup = true;
                    }
                    else {
                        SystemGlobal.Statup = false;
                    }
                    Init();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"应用程序启动失败：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                }
            }
            else
            {
                // 已有实例在运行，查找该实例并激活
                try
                {
                    // 查找已经运行的实例的主窗口
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            // 发送消息给已经运行的实例，让它显示主窗口
                            NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // 如果无法激活现有实例，只是提示并退出
                    MessageBox.Show("应用程序已在运行！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                Shutdown();
            }
        }
        /// <summary>
        /// 判断当前是否是开机启动
        /// </summary>
        /// <returns>如果是开机启动返回true，否则返回false</returns>
        private bool IsStartupLaunch()
        {
            try
            {
                // 检查命令行参数中是否包含 --startup 参数
                string[] args = Environment.GetCommandLineArgs();
                bool isStartup = args.Any(arg => arg.Equals("--startup", StringComparison.OrdinalIgnoreCase));

                Log.Information($"命令行参数: {string.Join(" ", args)}, 是否开机启动: {isStartup}");
                return isStartup;
            }
            catch (Exception ex)
            {
                Log.Error($"判断开机启动失败: {ex.Message}");
                return false;
            }
        }
        private void Init()
        {
            try
            {
                SqliteHelper.init();
                Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                   .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                   .CreateLogger();
                
                Log.Information("应用程序启动成功");
            }
            catch (Exception ex)
            {
                // 记录初始化失败信息
                Console.WriteLine($"初始化失败: {ex.Message}");
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                _mutex.ReleaseMutex();
                Log.Information("应用程序正常退出");
                Log.CloseAndFlush();
            }
            catch (Exception)
            {
                // 忽略退出时的错误
            }
            
            base.OnExit(e);
        }
    }

    // 用于调用Windows API的类
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
