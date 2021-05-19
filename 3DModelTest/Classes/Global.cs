using System;
using System.Runtime.InteropServices;

namespace PlayerModelTest
{

    public static class Global
    {
        public static long Sqrt(long r)
        {
            long t, q, b;
            b = 0x4000000000000000;
            q = 0;
            while (b > 0x40)
            {
                t = q + b;
                if (r >= t)
                {
                    r -= t;
                    q = t + b; // equivalent to q += 2*b
                }
                r <<= 1;
                b >>= 1;
            }
            q >>= 8;
            return q;

        }
        public static int Sqrt(int r)
        {
            int t, q, b;

            b = 0x40000000;
            q = 0;
            while (b > 0x40)
            {
                t = q + b;
                if (r >= t)
                {
                    r -= t;
                    q = t + b; // equivalent to q += 2*b
                }
                r <<= 1;
                b >>= 1;
            }
            q >>= 8;
            return q;

        }


        public static float AngleToFloat(short fix) => (float)AngleToDouble(fix);

        public static double AngleToDouble(short fix) => (fix / (double)(1 << 16)) * 360;// Math.PI * 2;

        public static float AngleToFloat(int fix) => (float)AngleToDouble(fix);
        public static double AngleToDouble(int fix) => ((fix / (double)(1 << 16)) * 360 - 180) % 360;//Math.PI * 2;


        public static float FixedToFloat(short fix) => fix;
        public static double FixedToDouble(short fix) => fix;
        public static decimal FixedToDecimal(short fix) => fix;

        public static float FixedToFloat(int fix) => (fix / (float)(1 << 16));
        public static double FixedToDouble(int fix) => (fix / (double)(1 << 16));
        public static decimal FixedToDecimal(int fix) => (fix / (decimal)(1 << 16));


        public static double FixedToFloat(long fix) => (fix / (float)(((long)1) << 32));
        public static double FixedToDouble(long fix) => (fix / (double)(((long)1) << 32));
        public static decimal FixedToDecimal(long fix) => (fix / (decimal)(((long)1) << 32));



        public static T ArrayToStruct<T>(byte[] data)
        {
            IntPtr ptPoit = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, ptPoit, data.Length);
                return (T)Marshal.PtrToStructure(ptPoit, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptPoit);
            }

        }

        public static System.Windows.Media.Color ToColor(ushort color)
        {

            var b = (color >> 10) & 0x1F;
            var g = (color >> 05) & 0x1F;
            var r = (color >> 00) & 0x1F;

            var rPerc = r / 31f;
            var gPerc = g / 31f;
            var bPerc = b / 31f;

            var red = (byte)((r << 3) | (byte)(0x7 * rPerc));
            var green = (byte)((g << 3) | (byte)(0x7 * gPerc));
            var blue = (byte)((b << 3) | (byte)(0x7 * bPerc));

            return System.Windows.Media.Color.FromArgb(0xFF, red, green, blue);
        }
    }
}
