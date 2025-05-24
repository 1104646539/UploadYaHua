using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace uploadyahua.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title;
        private readonly MainWindow _mainWindow;
      

        public MainViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
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
    }
}
