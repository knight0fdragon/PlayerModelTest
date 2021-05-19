using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x0C, Pack = 4)]
    public struct TransformMatrix
    {
        public Point Translate;
        public Point Scale;
        public Rotation Rotation;
        public int TranslateID;
        public int ScaleID;
        public int RotateID;


        public TranslateTransform3D ToTranslateTransform3D() => new TranslateTransform3D(Translate.ToVector().ToVector3D()); 
        public ScaleTransform3D ToScaleTransform3D() => new ScaleTransform3D(Scale.ToVector().ToVector3D());   
        public RotateTransform3D ToRotateTransform3D() => new RotateTransform3D(Rotation.ToRotation3D());
        

    }

}
