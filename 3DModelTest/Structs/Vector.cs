using System.Runtime.InteropServices;
using System.Windows.Media.Media3D;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x0C, Pack = 4)]
    public struct Vector
    {
        public int X;
        public int Y;
        public int Z;
        public Vector(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3D ToVector3D() => new Vector3D((double)Global.FixedToDouble(X), (double)Global.FixedToDouble(Y), (double)Global.FixedToDouble(Z));

        public static Vector operator +(Vector a, Vector b) => new Vector() { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
        public static Vector operator -(Vector a, Vector b) => new Vector() { X = (a.X - b.X), Y = (a.Y - b.Y), Z = (a.Z - b.Z) };
        public static Vector operator /(Vector a, int b) => new Vector() { X = a.X / b, Y = a.Y / b, Z = a.Z / b };
        public static Vector operator >>(Vector a, int b) => new Vector() { X = (a.X >> b), Y = (a.Y >> b), Z = (a.Z >> b) };

        public void Normalize()
        {

            var length = Length / 26;
            if ((length) == 0)
            {
                X = Y = Z = 0;
                return;
            }
                        
            //Magic number to help improve accuracy with Saturn Fixed point conversion
            var mul = 65078; //65076;
            
            X = (int)(((long)X * mul) / length); //459601582
            Y = (int)(((long)Y * mul) / length);
            Z = (int)(((long)Z * mul) / length);

        }
        public int Length => Global.Sqrt(LengthSquared);

        public int LengthSquared => ((X* X) + (Y* Y) + (Z* Z));
       
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x0C, Pack = 4)]
    public struct Vector16
    {
        public short X;
        public short Y;
        public short Z;
        public Vector3D ToVector3D()
        {
            return new Vector3D(Global.FixedToDouble(X), Global.FixedToDouble(Y), Global.FixedToDouble(Z));
        }
        public static Vector16 operator +(Vector16 a, Vector16 b) => new Vector16() { X = (short)(a.X + b.X), Y = (short)(a.Y + b.Y), Z = (short)(a.Z + b.Z) };
        public static Vector16 operator /(Vector16 a, int b) => new Vector16() { X = (short)(a.X / b), Y = (short)(a.Y / b), Z = (short)(a.Z / b) };

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x0C, Pack = 4)]
    public struct Vector64
    {
        public long X;
        public long Y;
        public long Z;
        public Vector3D ToVector3D()
        {
            return new Vector3D((double)Global.FixedToDecimal(X), (double)Global.FixedToDecimal(Y), (double)Global.FixedToDecimal(Z));
        }

        public Vector ToVector()
        {
            return new Vector() { X = (int)(X >> 16), Y = (int)(Y >> 16), Z = (int)(Z >> 16) };
        }
        public static Vector64 operator +(Vector64 a, Vector64 b) => new Vector64() { X = (long)(a.X + b.X), Y = (long)(a.Y + b.Y), Z = (long)(a.Z + b.Z) };
        public static Vector64 operator -(Vector64 a, Vector64 b) => new Vector64() { X = (long)(a.X - b.X), Y = (long)(a.Y - b.Y), Z = (long)(a.Z - b.Z) };

        public static Vector64 operator /(Vector64 a, int b) => new Vector64() { X = (long)(a.X / b), Y = (long)(a.Y / b), Z = (long)(a.Z / b) };

        public static Vector64 operator >>(Vector64 a, int b) => new Vector64() { X = (long)(a.X >> b), Y = (long)(a.Y >> b), Z = (long)(a.Z >> b) };
        public void Normalize()
        {
            var right = 24;
            var left = 40 - right;
            var length = Length() >> right;
            if ((length) == 0)
            {
                X = 0;
                Y = 0;
                Z = 0;
                return;
            }

            X = (this.X << left) / (length); //459601582
            Y = (this.Y << left) / (length);
            Z = (this.Z << left) / (length);
            X += length >> 2;
            Y += length >> 2;
            Z += length >> 2;


        }

        public long Length()
        {
            return Global.Sqrt(LengthSquared());//>> 8;
        }

        public long LengthSquared()
        {
            var X = this.X >> 16;
            var Y = this.Y >> 16;
            var Z = this.Z >> 16;

            var X2 = (X * X);
            var Y2 = (Y * Y);
            var Z2 = (Z * Z);
            return (X2 + Y2 + Z2);

        }

    }
}
