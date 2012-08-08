using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Media.Media3D;

namespace WpfApplication1
{
    public class BVHConverter
    {
        private readonly BVH m_src;

        public BVHConverter(BVH src)
        {
            m_src = src;
        }

        public BVH Convert()
        {
            BVH dest = SLTemplate();

            dest.Frames.Value = m_src.Frames.Value + 1;
            dest.FrameTime.Value = m_src.FrameTime.Value;
            dest.FrameList.Clear();

            FrameElement firstFrame = new FrameElement(dest);
            firstFrame.GetJointFrame("hip").SetValue("Yposition", 43.5285);
            dest.FrameList.Add(firstFrame);

            foreach (FrameElement frame in m_src.FrameList)
            {
                dest.FrameList.Add(ConvertFrame(dest, frame));
            }

            return dest;
        }

        private FrameElement ConvertFrame(BVH dest, FrameElement srcFrame)
        {
            FrameElement destFrame = new FrameElement(dest);
            foreach (CompositeElement joint in dest.JointList)
            {
                JointFrame jfDest = destFrame.GetJointFrame(joint.Name);
                JointFrame jfSrc = srcFrame.GetJointFrame(GetSrcJointName(joint.Name));
                if (jfSrc == null)
                    continue;

                foreach (string channel in new string[]{"Zrotation", "Xrotation", "Yrotation"})
                {
                    jfDest.SetValue(channel, jfSrc.GetValue(channel));
                }

                ConvertJointFrame(jfDest, joint.Name, srcFrame);
            }
            return destFrame;
        }

        private static void ConvertJointFrame(JointFrame jfDest, string jointName, FrameElement srcFrame)
        {
            Matrix3D matrix;

            switch (jointName)
            {
                case "hip":
                    matrix = jfDest.Matrix;
                    JointFrame jfRoot = srcFrame.GetJointFrame("root");
                    Matrix3D rootMatrix = jfRoot.Matrix;
                    rootMatrix.OffsetX *= 4;
                    rootMatrix.OffsetY *= 4;
                    rootMatrix.OffsetZ *= 4;
                    rootMatrix.OffsetY += 12;
                    matrix.Append(rootMatrix);
                    jfDest.Matrix = matrix;
                    break;
                case "abdomen":
                    matrix = jfDest.Matrix;
                    JointFrame jfLower = srcFrame.GetJointFrame("lower_body");
                    Matrix3D lowerMatrix = jfLower.Matrix;
                    lowerMatrix.Invert();
                    matrix.Append(lowerMatrix);
                    jfDest.Matrix = matrix;
                    break;
                // cast "chest" はなにもしない。MMDBVHで指定されている数値をそのままSLBVHとして利用
                case "lShldr":
                    matrix = jfDest.Matrix;
                    matrix.RotatePrepend(new Quaternion(new Vector3D(0, 0, 1), -35));
                    jfDest.Matrix = matrix;
                    break;
                case "lForeArm":
                case "lHand":
                    matrix = jfDest.Matrix;
                    matrix.RotatePrepend(new Quaternion(new Vector3D(0, 0, 1), -35));
                    matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), 35));
                    jfDest.Matrix = matrix;
                    break;
                case "rShldr":
                    // jfDest.AddValue("Zrotation", 35);                
                    matrix = jfDest.Matrix;
                    matrix.RotatePrepend(new Quaternion(new Vector3D(0, 0, 1), 35));
                    jfDest.Matrix = matrix;
                    break;
                case "rForeArm":
                case "rHand":
                    matrix = jfDest.Matrix;
                    matrix.RotatePrepend(new Quaternion(new Vector3D(0, 0, 1), 35));
                    matrix.Rotate(new Quaternion(new Vector3D(0, 0, 1), -35));
                    jfDest.Matrix = matrix;
                    break;
            }
        }

        private string GetSrcJointName(string destName)
        {
            switch (destName)
            {
                case "hip":
                    return "lower_body";
                case "abdomen":
                    return "upper_body";
                //added------------------
                case "chest": 
                    return "upper_body2";
                //-----------------------
                case "lCollar":
                    return "shoulder.L";
                case "lShldr":
                    return "arm.L";
                case "lForeArm":
                    return "elbow.L";
                case "lHand":
                    return "wrist.L";
                case "lThigh":
                    return "leg.L";
                case "lShin":
                    return "knee.L";
                case "lFoot":
                    return "ankle.L";
                case "rCollar":
                    return "shoulder.R";
                case "rShldr":
                    return "arm.R";
                case "rForeArm":
                    return "elbow.R";
                case "rHand":
                    return "wrist.R";
                case "rThigh":
                    return "leg.R";
                case "rShin":
                    return "knee.R";
                case "rFoot":
                    return "ankle.R";
            }
            return destName;
        }

        private static BVH SLTemplate()
        {
            BVH result = new BVH();
            string template = @"
                HIERARCHY
                ROOT hip
                {
	                OFFSET 0.000000 0.000000 0.000000
	                CHANNELS 6 Xposition Yposition Zposition Xrotation Zrotation Yrotation 
	                JOINT abdomen
	                {
		                OFFSET 0.000000 3.422050 0.000000
		                CHANNELS 3 Xrotation Zrotation Yrotation 
		                JOINT chest
		                {
			                OFFSET 0.000000 8.486693 -0.684411
			                CHANNELS 3 Xrotation Zrotation Yrotation 
			                JOINT neck
			                {
				                OFFSET 0.000000 10.266162 -0.273764
				                CHANNELS 3 Xrotation Zrotation Yrotation 
				                JOINT head
				                {
					                OFFSET 0.000000 3.148285 0.000000
					                CHANNELS 3 Xrotation Zrotation Yrotation 
					                End Site
					                {
						                OFFSET 0.000000 3.148289 0.000000
					                }
				                }
			                }
			                JOINT lCollar
			                {
				                OFFSET 3.422053 6.707223 -0.821293
				                CHANNELS 3 Yrotation Zrotation Xrotation 
				                JOINT lShldr
				                {
					                OFFSET 3.285171 0.000000 0.000000
					                CHANNELS 3 Zrotation Yrotation Xrotation 
					                JOINT lForeArm
					                {
						                OFFSET 10.129278 0.000000 0.000000
						                CHANNELS 3 Yrotation Zrotation Xrotation 
						                JOINT lHand
						                {
							                OFFSET 8.486692 0.000000 0.000000
							                CHANNELS 3 Zrotation Yrotation Xrotation 
							                End Site
							                {
								                OFFSET 4.106464 0.000000 0.000000
							                }
						                }
					                }
				                }
			                }
			                JOINT rCollar
			                {
				                OFFSET -3.558935 6.707223 -0.821293
				                CHANNELS 3 Yrotation Zrotation Xrotation 
				                JOINT rShldr
				                {
					                OFFSET -3.148289 0.000000 0.000000
					                CHANNELS 3 Zrotation Yrotation Xrotation 
					                JOINT rForeArm
					                {
						                OFFSET -10.266159 0.000000 0.000000
						                CHANNELS 3 Yrotation Zrotation Xrotation 
						                JOINT rHand
						                {
							                OFFSET -8.349810 0.000000 0.000000
							                CHANNELS 3 Zrotation Yrotation Xrotation 
							                End Site
							                {
								                OFFSET -4.106464 0.000000 0.000000
							                }
						                }
					                }
				                }
			                }
		                }
	                }
	                JOINT lThigh
	                {
		                OFFSET 5.338403 -1.642589 1.368821
		                CHANNELS 3 Xrotation Zrotation Yrotation 
		                JOINT lShin
		                {
			                OFFSET -2.053232 -20.121670 0.000000
			                CHANNELS 3 Xrotation Zrotation Yrotation 
			                JOINT lFoot
			                {
				                OFFSET 0.000000 -19.300380 -1.231939
				                CHANNELS 3 Xrotation Yrotation Zrotation 
				                End Site
				                {
					                OFFSET 0.000000 -2.463878 4.653993
				                }
			                }
		                }
	                }
	                JOINT rThigh
	                {
		                OFFSET -5.338403 -1.642589 1.368821
		                CHANNELS 3 Xrotation Zrotation Yrotation 
		                JOINT rShin
		                {
			                OFFSET 2.053232 -20.121670 0.000000
			                CHANNELS 3 Xrotation Zrotation Yrotation 
			                JOINT rFoot
			                {
				                OFFSET 0.000000 -19.300380 -1.231939
				                CHANNELS 3 Xrotation Yrotation Zrotation 
				                End Site
				                {
					                OFFSET 0.000000 -2.463878 4.653993
				                }
			                }
		                }
	                }
                }
                MOTION
                Frames:	1
                Frame Time:	0.033333
                0.000000 43.528519 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 0.000000 
            ";
            byte[] bytes = Encoding.Default.GetBytes(template);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                result.Load(stream);
            }

            foreach (CompositeElement joint in result.JointList)
            {
                switch (joint.Channels.ChannelList.Length)
                {
                    case 3:
                        joint.Channels.ChannelList[0] = "Zrotation";
                        joint.Channels.ChannelList[1] = "Xrotation";
                        joint.Channels.ChannelList[2] = "Yrotation";
                        break;
                    case 6:
                        joint.Channels.ChannelList[0] = "Xposition";
                        joint.Channels.ChannelList[1] = "Yposition";
                        joint.Channels.ChannelList[2] = "Zposition";
                        joint.Channels.ChannelList[3] = "Zrotation";
                        joint.Channels.ChannelList[4] = "Xrotation";
                        joint.Channels.ChannelList[5] = "Yrotation";
                        break;
                }
            }

            return result;
        }
    }
}
