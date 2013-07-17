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
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Diagnostics;
using WpfApplication1.Models;

namespace WpfApplication1.Views
{
    /// <summary>
    /// BVHView.xaml の相互作用ロジック
    /// </summary>
    public partial class BVHView : UserControl
    {
        public BVHView()
        {
            InitializeComponent();

            Width = double.NaN;
            Height = double.NaN;
            Scale = 1;

            viewport.Children.Add(CreateMarker(100, 0.03));
        }

        public static readonly DependencyProperty BVHProperty =
            DependencyProperty.Register("BVH", typeof(BVH), typeof(BVHView), new FrameworkPropertyMetadata()
            {
                PropertyChangedCallback = OnBVHChanged,
            });

        public BVH BVH {
            get
            {
                return (BVH)this.GetValue(BVHProperty);
            }
            set
            {
                this.SetValue(BVHProperty, value);
                UpdateView();
            }
        }

        private double m_scale;
        public double Scale
        {
            set
            {
                m_scale = value;
                globalScale.ScaleX = value;
                globalScale.ScaleY = value;
                globalScale.ScaleZ = value;
            }
        }

        public Vector3D Offset
        {
            set
            {
                globalOffset.OffsetX = value.X;
                globalOffset.OffsetY = value.Y;
                globalOffset.OffsetZ = value.Z;
            }
        }

        private readonly Storyboard m_storyboard = new Storyboard();

        private static void OnBVHChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BVHView view = d as BVHView;
            view.UpdateView();
        }

        private void UpdateView()
        {
            m_storyboard.Stop();

            m_storyboard.Children.Clear();
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames()
            {
                Duration = new Duration(TimeSpan.FromSeconds(BVH.FrameTime.Value * BVH.Frames.Value)),
                RepeatBehavior = RepeatBehavior.Forever,                
            };
            for (int i = 0; i < BVH.Frames.Value; i++)
            {
                timeline.KeyFrames.Add(new DiscreteDoubleKeyFrame(i));
            }
            m_storyboard.Children.Add(timeline);
            Storyboard.SetTarget(timeline, slider1);
            Storyboard.SetTargetProperty(timeline, new PropertyPath(Slider.ValueProperty));

            modelRoot.Children.Clear();
            modelRoot.Children.Add(CreateMarkerTree(BVH.Root));

            treeView1.ItemsSource = BVH;
            slider1.Maximum = BVH.Frames.Value - 1;
            UpdateFrame();
        }

        private Visual3D CreateMarkerTree(CompositeElement joint)
        {
            ContainerUIElement3D marker = CreateMarker(1, 0.05);
            joint.Visual = marker;
            
            Transform3DGroup transforms = new Transform3DGroup();
            marker.Transform = transforms;

            transforms.Children.Add(new MatrixTransform3D());

            Vector3D offset = joint.Offset.Value;
            transforms.Children.Add(new TranslateTransform3D(offset));

            foreach (CompositeElement child in joint.JointList)
            {
                marker.Children.Add(CreateMarkerTree(child));
            }

            return marker;
        }

        private ContainerUIElement3D CreateMarker(double length, double width)
        {
            length /= m_scale;
            width /= m_scale;

            ContainerUIElement3D visual = new ContainerUIElement3D();
            visual.Children.Add(new ModelUIElement3D()
            {
                Model = new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D()
                    {
                        Positions = new Point3DCollection(){
                            new Point3D(length, 0, 0),
                            new Point3D(0, width, 0),
                            new Point3D(0, -width, 0),
                            new Point3D(length, 0, 0),
                            new Point3D(0, 0, width),
                            new Point3D(0, 0, -width),
                        },
                    },
                    Material = new DiffuseMaterial()
                    {
                        Brush = new SolidColorBrush(Colors.Red),
                    },
                    BackMaterial = new DiffuseMaterial()
                    {
                        Brush = new SolidColorBrush(Colors.Red),
                    },
                },
            });
            visual.Children.Add(new ModelUIElement3D()
            {
                Model = new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D()
                    {
                        Positions = new Point3DCollection(){
                            new Point3D(0, length, 0),
                            new Point3D(width, 0, 0),
                            new Point3D(-width, 0, 0),
                            new Point3D(0, length, 0),
                            new Point3D(0, 0, width),
                            new Point3D(0, 0, -width),
                        },
                    },
                    Material = new DiffuseMaterial()
                    {
                        Brush = new SolidColorBrush(Colors.Lime),
                    },
                    BackMaterial = new DiffuseMaterial()
                    {
                        Brush = new SolidColorBrush(Colors.Lime),
                    },
                },
            });
            visual.Children.Add(new ModelUIElement3D()
            {
                Model = new GeometryModel3D()
                {
                    Geometry = new MeshGeometry3D()
                    {
                        Positions = new Point3DCollection(){
                            new Point3D(0, 0, length),
                            new Point3D(width, 0, 0),
                            new Point3D(-width, 0, 0),
                            new Point3D(0, 0, length),
                            new Point3D(0, width, 0),
                            new Point3D(0, -width, 0),
                        },
                    },
                    Material = new DiffuseMaterial()
                    {
                        Brush = new SolidColorBrush(Colors.Blue),
                    },
                    BackMaterial = new DiffuseMaterial()
                    {
                        Brush = new SolidColorBrush(Colors.Blue),
                    },
                },
            });

            return visual;
        }

        private void treeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CompositeElement element;
            ContainerUIElement3D visual;
            ModelUIElement3D x_visual;
            GeometryModel3D x_model;
            DiffuseMaterial x_material;

            element = e.OldValue as CompositeElement;
            if (element != null)
            {
                visual = element.Visual;
                x_visual = visual.Children[0] as ModelUIElement3D;
                x_model = x_visual.Model as GeometryModel3D;
                x_material = x_model.Material as DiffuseMaterial;
                x_material.Brush = new SolidColorBrush(Colors.Red);
                x_material = x_model.BackMaterial as DiffuseMaterial;
                x_material.Brush = new SolidColorBrush(Colors.Red);
            }

            element = e.NewValue as CompositeElement;
            if (element != null)
            {
                visual = element.Visual;
                x_visual = visual.Children[0] as ModelUIElement3D;
                x_model = x_visual.Model as GeometryModel3D;
                x_material = x_model.Material as DiffuseMaterial;
                x_material.Brush = new SolidColorBrush(Colors.White);
                x_material = x_model.BackMaterial as DiffuseMaterial;
                x_material.Brush = new SolidColorBrush(Colors.White);
            }
        }

        private Point m_base_point;        
        private double m_base_angle_y;
        private double m_base_angle_x;
        private double m_base_offset_x;
        private double m_base_offset_y;

        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_base_point = e.GetPosition(viewport);
            m_base_angle_y = cameraRotationY.Angle;
            m_base_angle_x = cameraRotationX.Angle;
            m_base_offset_x = cameraOffset.OffsetX;
            m_base_offset_y = cameraOffset.OffsetY;
        }

        private void DockPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_base_point = e.GetPosition(viewport);
            m_base_angle_y = cameraRotationY.Angle;
            m_base_angle_x = cameraRotationX.Angle;
            m_base_offset_x = cameraOffset.OffsetX;
            m_base_offset_y = cameraOffset.OffsetY;
        }

        private void DockPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
                return;

            Point p = e.GetPosition(viewport);

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                cameraOffset.OffsetX = m_base_offset_x + (m_base_point.X - p.X) * camera.FieldOfView * 0.001;
                cameraOffset.OffsetY = m_base_offset_y + (m_base_point.Y - p.Y) * camera.FieldOfView * -0.001;
            }
            else
            {
                cameraRotationY.Angle = m_base_angle_y + m_base_point.X - p.X;
                cameraRotationX.Angle = m_base_angle_x + m_base_point.Y - p.Y;
                if (cameraRotationX.Angle < -89.0)
                    cameraRotationX.Angle = -89.0;
                if (cameraRotationX.Angle > 89.0)
                    cameraRotationX.Angle = 89.0;
            }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (BVH == null)
                return;
            double v = slider1.Value;
            m_storyboard.Begin();
            m_storyboard.Seek(TimeSpan.FromSeconds(BVH.FrameTime.Value * v));
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            double v = slider1.Value;
            m_storyboard.Stop();
            slider1.Value = v;
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateFrame();
        }

        private void UpdateFrame()
        {
            if (BVH == null)
                return;

            FrameElement frame = BVH.FrameList[(int)slider1.Value];
            foreach (CompositeElement joint in BVH.JointList)
            {
                JointFrame jf = frame.GetJointFrame(joint.Name);
                Transform3DGroup tg = (Transform3DGroup)joint.Visual.Transform;
                MatrixTransform3D transform = (MatrixTransform3D)tg.Children[0];
                transform.Matrix = jf.Matrix;
            }
        }

        private void DockPanel_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            camera.FieldOfView -= e.Delta / 120.0;
            if (camera.FieldOfView < 3.0)
                camera.FieldOfView = 3.0;
            if (camera.FieldOfView > 60.0)
                camera.FieldOfView = 60.0;
        }
    }
}
