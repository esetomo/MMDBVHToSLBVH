using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Controls.Ribbon;
using WpfApplication1.Models;
using WpfApplication1.ViewModels;

namespace WpfApplication1.Views
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : RibbonWindow
    {
        MainViewModel viewModel;

        public Window1()
        {
            viewModel = new MainViewModel();

            InitializeComponent();

            DataContext = viewModel;
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var command = viewModel.OpenCommand;
            if (command != null)
            {
                e.Handled = true;
                e.CanExecute = command.CanExecute(e.Parameter);
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = viewModel.OpenCommand;
            if (command != null)
            {
                e.Handled = true;
                command.Execute(e.Parameter);
            }
        }

        private void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var command = viewModel.SaveAsCommand;
            if (command != null)
            {
                e.Handled = true;
                e.CanExecute = command.CanExecute(e.Parameter);
            }
        }

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var command = viewModel.SaveAsCommand;
            if (command != null)
            {
                e.Handled = true;
                command.Execute(e.Parameter);
            }
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }
    }
}
