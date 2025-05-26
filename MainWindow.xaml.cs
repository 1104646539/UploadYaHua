using MahApps.Metro.Controls;
using PdfSharp.Fonts;
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
using uploadyahua.Util;
using uploadyahua.ViewModel;

namespace uploadyahua
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            GlobalFontSettings.FontResolver = new MyFontResolverInfo();
            InitializeComponent();
            DataContext = new MainViewModel(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            if (GlobalConfig.Instance.Minimize == 1)
            {
                e.Cancel = true;
                this.Hide();
            }
            else {
                Application.Current.Shutdown();
            }
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.Activate();
        }

    }
}
