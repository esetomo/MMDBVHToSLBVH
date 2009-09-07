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

namespace WpfApplication1
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            bvhFrom.Offset = new System.Windows.Media.Media3D.Vector3D(0, -10, 0);
            bvhTo.Scale = 0.25;
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.DefaultExt = ".bvh";
            dialog.Filter = "BVHファイル(*.bvh)|*.bvh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BVH bvh = new BVH();
                bvh.Load(dialog.OpenFile());
                bvhFrom.BVH = bvh;
                bvhTo.BVH = bvh.Convert();
                menuItemUseAll.IsChecked = false;
            }
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.DefaultExt = ".bvh";
            dialog.Filter = "BVHファイル(*.bvh)|*.bvh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bvhTo.BVH.Save(dialog.FileName);
            }
        }

        private void UseAll_Checked(object sender, RoutedEventArgs e)
        {
            foreach (CompositeElement joint in bvhTo.BVH.JointList)
            {
                JointFrame jf = bvhTo.BVH.FrameList[0].GetJointFrame(joint.Name);
                jf.SetValue("Xrotation", 0.1);
            }
        }

        private void UseAll_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach(CompositeElement joint in bvhTo.BVH.JointList){
                JointFrame jf = bvhTo.BVH.FrameList[0].GetJointFrame(joint.Name);
                jf.SetValue("Xrotation", 0.0);
            }
        }
    }
}
