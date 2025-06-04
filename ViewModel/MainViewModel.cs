using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using uploadyahua.Model;
using uploadyahua.Util;
using System.Net;

using System.Collections.ObjectModel;
using System.Web.UI;
using System.Runtime.InteropServices;
using System.Drawing.Printing;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace uploadyahua.ViewModel
{
    public partial class MainViewModel : ObservableRecipient, OnConnectStateListener
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private string ip;

        [ObservableProperty]
        private string port;

        [ObservableProperty]
        private string openBtnText;
        private readonly MainWindow _mainWindow;

        NetworkUtil networkUtil = new NetworkUtil();
        [ObservableProperty]
        private ObservableCollection<TestResult> testResults = new ObservableCollection<TestResult>();
        [ObservableProperty]
        private string stateMsg;
        [ObservableProperty]
        private TestResult selectedTestResult;

        // 分页相关属性
        [ObservableProperty]
        private int currentPage = 1;
        [ObservableProperty]
        private int totalPages = 1;
        [ObservableProperty]
        private int pageSize = 100;
        [ObservableProperty]
        private string pageInfo;

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool minimize;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool sampleMode;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool autoStartup;

        [ObservableProperty]
        private string icoPath;
        [ObservableProperty]
        private ObservableCollection<string> availableIPs;

        [ObservableProperty]
        private ObservableCollection<string> availablePrinters;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private string selectedPrinter;

        public MainViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            // 初始化可用 IP 列表
            AvailableIPs = new ObservableCollection<string>();
            GetAllNetworkIPs();
            string tempIp = GlobalConfig.Instance.IP;
            
            if(AvailableIPs!=null && AvailableIPs.Count > 0) {
             if(AvailableIPs.Contains(tempIp)){
                    Ip = tempIp;
               }else{
                    Ip = availableIPs[0];
               }
            }
            // 初始化端口为 8866
            Minimize = GlobalConfig.Instance.Minimize ;
            SampleMode = GlobalConfig.Instance.SampleMode;
            Port = GlobalConfig.Instance.Port;
            AutoStartup = GlobalConfig.Instance.AutoStartup;
            StateMsg = "待启动";
            icoPath = "pack://application:,,,/DLL/icon.ico";
            UpdateBtnText();
            InitPrinter();
            InitData();
            InitAutoStartupState();
            Title = $"雅华打印报告 {System.Windows.Application.ResourceAssembly.GetName().Version.ToString()}";
        }

        /// <summary>
        /// 检查项目是否已经设置为开机自启动
        /// </summary>
        private void InitAutoStartupState()
        {
            try
            {
                // 快捷方式方式检查
                AutoStartup = IsStartupShortcutExists();
            }
            catch (Exception ex)
            {
                Log.Error($"检查开机启动状态失败: {ex.Message}");
                AutoStartup = false;
            }
        }

        /// <summary>
        /// 开启开机自启动
        /// </summary>
        private void startAutoStartup()
        {
            try
            {
                // 快捷方式方式
                CreateStartupShortcut();
            }
            catch (Exception ex)
            {
                Log.Error($"设置开机启动失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 取消开机自启动
        /// </summary>
        private void stopAutoStartup()
        {
            try
            {
                // 快捷方式方式
                DeleteStartupShortcut();
            }
            catch (Exception ex)
            {
                Log.Error($"取消开机启动失败: {ex.Message}");
            }
        }

        private void InitPrinter()
        {
            AvailablePrinters = new ObservableCollection<string>();
            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                AvailablePrinters.Add(printer);
            }

            string tempPrinter = GlobalConfig.Instance.Printer;
            if (AvailablePrinters.Count > 0)
            {
                if (AvailablePrinters.Contains(tempPrinter))
                {
                    SelectedPrinter = tempPrinter;
                }
                else { 
                    SelectedPrinter = AvailablePrinters[0];
                }
            }
        }

        private void GetAllNetworkIPs()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    AvailableIPs.Add(ipAddress.ToString());
                }
            }
        }

        protected override void Broadcast<T>(T oldValue, T newValue, string? propertyName)
        {
            base.Broadcast(oldValue, newValue, propertyName);

            if (propertyName == nameof(Minimize))
            {
                GlobalConfig.Instance.Minimize = Minimize ;
            }
            else if (propertyName == nameof(SampleMode))
            {
                GlobalConfig.Instance.SampleMode = SampleMode;
            }
            else if (propertyName == nameof(SelectedPrinter)) {
                GlobalConfig.Instance.Printer = SelectedPrinter;
            }
            else if (propertyName == nameof(AutoStartup))
            {
                GlobalConfig.Instance.AutoStartup = AutoStartup;
                if (AutoStartup)
                {
                    startAutoStartup();
                }
                else { 
                    stopAutoStartup();
                }
            }
        }

        private void UpdateConfig()
        {
            GlobalConfig.Instance.Minimize = Minimize  ;
            GlobalConfig.Instance.SampleMode = SampleMode ;
            GlobalConfig.Instance.Printer = SelectedPrinter;
            GlobalConfig.Instance.AutoStartup = AutoStartup ;
        }

        private void UpdateBtnText()
        {
            if (networkUtil.IsOpen)
            {
                OpenBtnText = "断开服务";
            }
            else {
                OpenBtnText = "启动服务";
            }
        }

        private void InitData()
        {
            // 检查是否是开机启动
            bool isStartupLaunch = IsStartupLaunch();
            
            if (isStartupLaunch)
            {
                // 如果是开机启动，延时5秒加载数据
                Log.Information("检测到开机启动，将在5秒后加载数据");
                Task.Delay(5000).ContinueWith(_ => 
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        LoadData();
                        Log.Information("开机启动延时加载数据完成");
                    });
                });
            }
            else
            {
                // 正常启动，直接加载数据
                LoadData();
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

        private async void LoadData()
        {
            // 获取总记录数以计算总页数
            int totalCount = await SqliteHelper.GetTotalCount();
            TotalPages = (totalCount + PageSize - 1) / PageSize; // 向上取整计算总页数
            if (TotalPages == 0) TotalPages = 1; // 确保至少有1页
            
            // 加载当前页数据
            List<TestResult> trs = await SqliteHelper.QueryTestResults(CurrentPage, PageSize);
            TestResults.Clear();
            foreach (var tr in trs)
            {
                TestResults.Add(tr);
            }
            
            // 更新页码信息
            UpdatePageInfo();
        }
        
        private void UpdatePageInfo()
        {
            PageInfo = $"{CurrentPage}/{TotalPages}";
        }
        
        [RelayCommand]
        public void NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                LoadData();
            }
        }
        
        [RelayCommand]
        public void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                LoadData();
            }
        }

        [RelayCommand]
        public void InsertTest() {
            Random random = new Random();
            List< Result > results = new List<Result>();
            results.Add(new Result() {
                TestNum = "测试编号" + random.Next(),
                TestItem = "测试项目" + random.Next(),
                TestValue = "测试值" + random.Next(),
                TestResult = "测试结果" + random.Next(),
                Reference = "参考值" + random.Next()
            });
            TestResult tr = new TestResult() {
                PatientName = "测试姓名" + random.Next(),
                PatientNum = "测试编号" + random.Next(),
                SampleNum = "样本编号" + random.Next(),
                Result = results
            };
            TestResult ret = SqliteHelper.InsertTestResult(tr);
            Log.Information($"ret={JsonConvert.SerializeObject(ret)}");
        }
  
        [RelayCommand]
        public void QueryTest()
        {
            //List<TestResult> tr = SqliteHelper.QueryTestResults();
            //Log.Information($"tr={JsonConvert.SerializeObject(tr)}");
        }
        [RelayCommand]
        public void StartService() {
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(port)) { 

            }
            if (networkUtil.IsOpen)
            {
                ClickOpen = false;
                networkUtil.Close();
            }
            else {
                ClickOpen = true;
                networkUtil.StartWebSocketServer(Ip,Port,this);
            }
        }
        [RelayCommand]
        public void ShowWindow()
        {
            _mainWindow.Show();
            _mainWindow.Activate();
        }

        [RelayCommand]
        public void ExitApp()
        {
            Application.Current.Shutdown();
        }
        bool ClickOpen = false;
        public void onConnectOpenSuccess()
        {
            GlobalConfig.Instance.Port = Port;
            GlobalConfig.Instance.IP = Ip;
            StateMsg = "启动成功";
            UpdateBtnText();
            Log.Information($"连接成功 onConnectSuccess");
        }

        public void onConnectOpenFailed(string error)
        {
            UpdateBtnText();
            if (ClickOpen)
            {
                StateMsg = "启动失败";
            }
            else {
                StateMsg = "已关闭服务";
            }
            Log.Information($"连接失败 onConnectFailed={error}");
        }

        public void onConnectDisConenct(string error)
        {
            StateMsg = "断开连接";
            Log.Information($"断开连接 onConnectDisConenct={error}");
        }

        public void onNewMsg(TestResult testResult)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ParseMsg(testResult);
            });
           
        }

        private void ParseMsg(TestResult testResult)
        {
            Log.Information($"收到信息 onNewMsg={JsonConvert.SerializeObject(testResult)}");
            if (testResult == null) return;
            TestResult temp = SqliteHelper.InsertTestResult(testResult);
            if (temp != null)
            {
                Log.Information($"插入成功");
                // 如果在第1页，直接添加到列表
                if (CurrentPage == 1)
                {
                    TestResults.Insert(0, temp);
                    // 如果超过了每页显示数量，移除最后一条
                    if (TestResults.Count > PageSize)
                    {
                        TestResults.RemoveAt(TestResults.Count - 1);
                    }
                }
                
                if (GlobalConfig.Instance.SampleMode)
                { 
                    PrintTestResult(temp, isAutoPrint:true);
                }
            }
            else
            {
                Log.Information($"插入数据失败 onNewMsg={JsonConvert.SerializeObject(testResult)}" +
                    $"，请检查数据是否正确。");
                MessageBox.Show($"插入数据失败 onNewMsg={JsonConvert.SerializeObject(testResult)}" +
                    $"，请检查数据是否正确。");
            }
        }

        public void onClientAdd(string msg)
        {
            StateMsg = "设备已连接";
            Log.Information($"新设备连接 onClientAdd={msg}");
        }
        [RelayCommand]
        public void SaveInfo() {
            if (SelectedTestResult != null) { 
              int ret = SqliteHelper.UpdateTestResult(SelectedTestResult);
              if(ret > 0){
                Log.Information("更新成功");
                if(ret > 0){
                    TestResult trTemp = SqliteHelper.GetTestResultForId(SelectedTestResult.Id);
                    UpdateTestResultForId(trTemp);
                    SelectedTestResult = trTemp;
                }
              }else{
                Log.Information("更新失败");
              }
             }
        }

        private void UpdateTestResultForId(TestResult trTemp)
        {
            if (TestResults == null || TestResults.Count <= 0) return;
            for (int i = 0; i < TestResults.Count; i++) {
                if (TestResults[i].Id == trTemp.Id) { 
                    TestResults[i] = trTemp;
                }
            }
        }

        [RelayCommand]
        public void PrintReport()
        {
            InitAutoStartupState();
            //PrintTestResult(SelectedTestResult);
        }

        private void PrintTestResult(TestResult tr, bool isAutoPrint = false)
        {
            if (tr == null)
            {
                return;
            }

            ReportUtil reportUtil = new ReportUtil();
            //string path = reportUtil.Print(tr,SelectedPrinter);
            string path = reportUtil.Create(tr);

            if (string.IsNullOrEmpty(path))
            {
            
                if (!isAutoPrint)
                {
                    MessageBox.Show("打印失败 " + path);
                }
                Log.Information("打印失败 " + path);
            }
            else
            {
                if (!isAutoPrint)
                {
                    MessageBox.Show("打印成功，保存在 " + path);
                }
                Log.Information("打印成功，保存在 " + path);
            }
            
        }

        /// <summary>
        /// 创建开机启动快捷方式
        /// </summary>
        private void CreateStartupShortcut()
        {
            try
            {
                // 获取启动文件夹路径
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolderPath, $"{SystemGlobal.KeyName}.lnk");
                string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string appDir = Path.GetDirectoryName(appPath);

                // 创建快捷方式
                Type t = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(t);
                var shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = appPath;
                shortcut.Arguments = "--startup";
                shortcut.WorkingDirectory = appDir;
                shortcut.Description = $"{SystemGlobal.KeyName} 自动启动";
                shortcut.IconLocation = appPath + ",0";
                shortcut.Save();

                Log.Information($"已创建开机启动快捷方式: {shortcutPath}");
            }
            catch (Exception ex)
            {
                Log.Error($"创建开机启动快捷方式失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 删除开机启动快捷方式
        /// </summary>
        private void DeleteStartupShortcut()
        {
            try
            {
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolderPath, $"{SystemGlobal.KeyName}.lnk");

                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                    Log.Information($"已删除开机启动快捷方式: {shortcutPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"删除开机启动快捷方式失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 检查开机启动快捷方式是否存在
        /// </summary>
        private bool IsStartupShortcutExists()
        {
            try
            {
                string startupFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                string shortcutPath = Path.Combine(startupFolderPath, $"{SystemGlobal.KeyName}.lnk");
                bool exists = File.Exists(shortcutPath);
                Log.Information($"检查开机启动快捷方式: {shortcutPath}, 存在: {exists}");
                return exists;
            }
            catch (Exception ex)
            {
                Log.Error($"检查开机启动快捷方式失败: {ex.Message}");
                return false;
            }
        }
    }
}
