using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x0C, Pack = 4)]
    public struct Point
    {
        public int X;
        public int Y;
        public int Z;


        public Point3D ToPoint3D()=> new Point3D(Global.FixedToDouble(X), Global.FixedToDouble(Y), Global.FixedToDouble(Z));
        
        public static Point operator +(Point a, Point b) => new Point() { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };

        public static Point operator -(Point a, Point b) => new Point() { X = a.X - b.X, Y = a.Y - b.Y, Z = a.Z - b.Z };


        public Vector ToVector() => new Vector() { X = X, Y = Y, Z = Z };
        public Point(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

}
