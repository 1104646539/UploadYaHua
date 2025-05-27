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

        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool minimize;
        [ObservableProperty]
        [NotifyPropertyChangedRecipients]
        private bool sampleMode;

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
            Minimize = GlobalConfig.Instance.Minimize == 1;
            SampleMode = GlobalConfig.Instance.SampleMode == 1;
            Port = GlobalConfig.Instance.Port;
            StateMsg = "待启动";
            icoPath = "pack://application:,,,/DLL/icon.ico";
            UpdateBtnText();
            InitPrinter();
            InitData();
            Title = $"雅华打印报告 {System.Windows.Application.ResourceAssembly.GetName().Version.ToString()}";

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
                GlobalConfig.Instance.Minimize = Minimize ? 1 : 0;
            }
            else if (propertyName == nameof(SampleMode))
            {
                GlobalConfig.Instance.SampleMode = SampleMode ? 1 : 0;
            }
            else if (propertyName == nameof(SelectedPrinter)) {
                GlobalConfig.Instance.Printer = SelectedPrinter;
            }
        }

        private void UpdateConfig()
        {
            GlobalConfig.Instance.Minimize = Minimize ? 1 : 0;
            GlobalConfig.Instance.SampleMode = SampleMode ? 1 : 0;
            GlobalConfig.Instance.Printer = SelectedPrinter;
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
            LoadData();
        }

        private async void LoadData()
        {
           List<TestResult> trs = await SqliteHelper.QueryTestResultsToday();
           TestResults.Clear();
           for (int i = 0; i < trs.Count; i++) { 
                TestResults.Add(trs[i]);
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
                TestResults.Insert(0,temp);
                if (GlobalConfig.Instance.SampleMode == 1) { 
                    PrintTestResult(temp,isAutoPrint:true);
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
            PrintTestResult(SelectedTestResult);
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
    }
}
