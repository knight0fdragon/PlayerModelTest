using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x10, Pack = 4)]
    public struct Rotation
    {
        public int X;
        public int Y;
        public int Z;
        public int Angle;
        public AxisAngleRotation3D ToRotation3D() => new AxisAngleRotation3D(new Vector(X, Y, Z).ToVector3D(), Global.AngleToDouble(Angle));
    }


}
