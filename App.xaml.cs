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
        private static Mutex _mutex = new Mutex(false, "{8F6F0AC4-B9A1-45FD-A8CF-72F04E6BDE8F}"); // ΨһGUID

        protected override void OnStartup(StartupEventArgs e)
        {
            
            // ���Ի�ȡ�����壬�����ȡ�ɹ�����ʾ��ǰ�ǵ�һ��ʵ��
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
                    MessageBox.Show($"Ӧ�ó�������ʧ�ܣ�{ex.Message}", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                    Shutdown();
                }
            }
            else
            {
                // ����ʵ�������У����Ҹ�ʵ��������
                try
                {
                    // �����Ѿ����е�ʵ����������
                    Process current = Process.GetCurrentProcess();
                    foreach (Process process in Process.GetProcessesByName(current.ProcessName))
                    {
                        if (process.Id != current.Id)
                        {
                            // ������Ϣ���Ѿ����е�ʵ����������ʾ������
                            NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    // ����޷���������ʵ����ֻ����ʾ���˳�
                    MessageBox.Show("Ӧ�ó����������У�", "��ʾ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                Shutdown();
            }
        }
        /// <summary>
        /// �жϵ�ǰ�Ƿ��ǿ�������
        /// </summary>
        /// <returns>����ǿ�����������true�����򷵻�false</returns>
        private bool IsStartupLaunch()
        {
            try
            {
                // ��������в������Ƿ���� --startup ����
                string[] args = Environment.GetCommandLineArgs();
                bool isStartup = args.Any(arg => arg.Equals("--startup", StringComparison.OrdinalIgnoreCase));

                Log.Information($"�����в���: {string.Join(" ", args)}, �Ƿ񿪻�����: {isStartup}");
                return isStartup;
            }
            catch (Exception ex)
            {
                Log.Error($"�жϿ�������ʧ��: {ex.Message}");
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
                
                Log.Information("Ӧ�ó��������ɹ�");
            }
            catch (Exception ex)
            {
                // ��¼��ʼ��ʧ����Ϣ
                Console.WriteLine($"��ʼ��ʧ��: {ex.Message}");
                throw;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                _mutex.ReleaseMutex();
                Log.Information("Ӧ�ó��������˳�");
                Log.CloseAndFlush();
            }
            catch (Exception)
            {
                // �����˳�ʱ�Ĵ���
            }
            
            base.OnExit(e);
        }
    }

    // ���ڵ���Windows API����
    internal static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
