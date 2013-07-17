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
using System.Windows.Shapes;
using WpfApplication1.Models;

namespace WpfApplication1.Views
{
    /// <summary>
    /// FpsInputWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FpsInputWindow : Window
    {
        public FpsInputWindow()
        {
            InitializeComponent();
        }

        private void CommitButton_Click(object sender, RoutedEventArgs e)
        {
            double frameDuration = (((int) durationSlider.Value) * 30.0) ;

            BVH.FRAMES_PER_FILE = (int) frameDuration;

            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void durationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.durationValueLabel != null)
            {
                this.durationValueLabel.Content = String.Format("{0} sec", (int) this.durationSlider.Value);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.durationSlider.Value = (BVH.FRAMES_PER_FILE) / 30;
            this.durationValueLabel.Content = String.Format("{0} sec", (int)this.durationSlider.Value);
        }
    }
}
