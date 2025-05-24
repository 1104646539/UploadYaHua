using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using uploadyahua.ViewModel;

namespace uploadyahua
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // 取消关闭操作，改为隐藏窗口
            e.Cancel = true;
            this.Hide();
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.Activate();
        }
    }

    //public class MainWindowViewModel
    //{
    //    private readonly MainWindow _mainWindow;

    //    public MainWindowViewModel(MainWindow mainWindow)
    //    {
    //        _mainWindow = mainWindow;
    //    }

    //    public ICommand ShowWindowCommand => new RelayCommand(() =>
    //    {
    //        _mainWindow.Show();
    //        _mainWindow.Activate();
    //    });

    //    public ICommand ExitAppCommand => new RelayCommand(() =>
    //    {
    //        Application.Current.Shutdown();
    //    });
    //}

    //public class RelayCommand : ICommand
    //{
    //    private readonly System.Action _execute;

    //    public RelayCommand(System.Action execute)
    //    {
    //        _execute = execute;
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public void Execute(object parameter)
    //    {
    //        _execute();
    //    }

    //    public event System.EventHandler CanExecuteChanged;
    //}
}
