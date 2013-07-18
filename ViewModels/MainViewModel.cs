using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using WpfApplication1.Models;
using WpfApplication1.Views;

namespace WpfApplication1.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            openCommand = new DelegateCommand(OpenCommandExecute);
            saveAsCommand = new DelegateCommand(SaveAsCommandExecute);

            Duration = 60;
        }

        private void OpenCommandExecute(object parameter)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.DefaultExt = ".bvh";
            dialog.Filter = "BVHファイル(*.bvh)|*.bvh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BVH bvh = new BVH();
                bvh.Load(dialog.OpenFile());
                BVHFrom = bvh;
            }
        }

        private void SaveAsCommandExecute(object parameter)
        {
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.DefaultExt = ".bvh";
            dialog.Filter = "BVHファイル(*.bvh)|*.bvh";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BVHTo.Save(dialog.FileName, Duration);
            }
        }

        #region BVH BVHFrom
        private BVH _BVHFrom;
        public BVH BVHFrom
        {
            get
            {
                return _BVHFrom;
            }
            set
            {
                _BVHFrom = value;
                RaisePropertyChanged();

                _BVHTo = value.Convert();
                RaisePropertyChanged(() => BVHTo);

                UseAll = false;
            }
        }
        #endregion

        #region BVH BVHTo
        private BVH _BVHTo;
        public BVH BVHTo
        {
            get
            {
                return _BVHTo;
            }
        }
        #endregion

        #region bool UseAll
        private bool _UseAll;
        public bool UseAll
        {
            get
            {
                return _UseAll;
            }
            set
            {
                _UseAll = value;
                RaisePropertyChanged();

                if (_BVHFrom == null)
                    return;

                if (_UseAll)
                {
                    foreach (CompositeElement joint in BVHTo.JointList)
                    {
                        JointFrame jf = BVHTo.FrameList[0].GetJointFrame(joint.Name);
                        jf.SetValue("Xrotation", 0.1);
                    }
                }
                else
                {
                    foreach (CompositeElement joint in BVHTo.JointList)
                    {
                        JointFrame jf = BVHTo.FrameList[0].GetJointFrame(joint.Name);
                        jf.SetValue("Xrotation", 0.0);
                    }
                }
            }
        }
        #endregion

        #region int Duration
        private int _Duration;
        public int Duration
        {
            get
            {
                return _Duration;
            }
            set
            {
                if (_Duration != value)
                {
                    _Duration = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region Commands
        private readonly ICommand openCommand;
        public ICommand OpenCommand
        {
            get
            {
                return openCommand;
            }
        }

        private readonly ICommand saveAsCommand;
        public ICommand SaveAsCommand
        {
            get
            {
                return saveAsCommand;
            }
        }
        #endregion
    }
}
