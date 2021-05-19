using System.Runtime.InteropServices;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x0C, Pack = 4)]
    public struct ATTR
    {
        public byte Flag; //Front back
        public byte ZOrder;
        public ushort TextureNum;
        public ushort Attribute;
        public ushort ColorNumber;
        public ushort GouraudShading;
        public ushort Dir; //Flip

        public bool IsLine => (Dir & 0x01) > 0;
        public bool IsTexture => (Dir & 0x02) > 0;
        public bool IsPolygon => (Dir & 0x04) > 0;
        public bool IsPolyLine => (Dir & 0x05) > 0;

        public bool IsHorizontalFlip => (Dir & 0x10) > 0;
        public bool IsVerticalFlip => (Dir & 0x20) > 0;
        public bool IsHalfTransparent => (Dir & 0x80) > 0;
        public bool IsDoubleSided => Flag > 0;
        public byte ColorMode => (byte)((Attribute >> 3) & 7);
    }


}
