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

namespace uploadyahua.ViewModel
{
    public partial class MainViewModel : ObservableObject, OnConnectStateListener
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
        public MainViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            // 获取本机 IP 地址
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipAddress in hostEntry.AddressList)
            {
                if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    Ip = ipAddress.ToString();
                    break;
                }
            }
            // 初始化端口为 8866
            Port = "8866";
            StateMsg = "待启动";
            OpenBtnText = "启动";
            InitData();
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
            networkUtil.StartWebSocketServer(Ip,Port,this);
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

        public void onConnectOpenSuccess()
        {
            StateMsg = "启动成功";
            OpenBtnText = "断开";
            Log.Information($"连接成功 onConnectSuccess");
        }

        public void onConnectOpenFailed(string error)
        {
            StateMsg = "启动失败";
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
        public void saveInfo() {
            if (SelectedTestResult != null) { 

            }
        }
        [RelayCommand]
        public void printReport()
        {

        }
    }
}
