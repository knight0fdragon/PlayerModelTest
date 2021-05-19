using System.Drawing;

namespace PlayerModelTest
{
    public class Texture
    {
        public ushort Width;
        public ushort Height;
        public ushort CharacterGraphicsAddress;
        public ushort Size;
        public int TrueSize => Width * Height;
        public byte[] Data;
        public uint[] Data32;
        public Bitmap ToBitmap()
        {
            var bitmap = new Bitmap(Width, Height);
            var c = 0;
            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                    bitmap.SetPixel(x, y, Color.FromArgb((int)Data32[c++]));

            return bitmap;
        }
        public void Export(string filename) => ToBitmap().Save(filename, System.Drawing.Imaging.ImageFormat.Png);

    }
}
