using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerModelTest
{
    public class Textures
    {
        public byte[] Uncompressed;
        public uint[] Uncompressed32;
        public Textures(byte[] data, int size)
        {

            Uncompressed = DecompressTextures(data.Take(size).ToList()).SelectMany(x => x).ToArray();
            var colors32 = new List<uint>();
            for (var i = 0; i < Uncompressed.Length; i += 2)
            {
                var high = Uncompressed[i];
                var low = Uncompressed[i + 1];
                var color = (high << 8) | low;
                var r = (color >> 10) & 0x1F;
                var g = (color >> 05) & 0x1F;
                var b = (color >> 00) & 0x1F;

                var rPerc = r / 31f;
                var gPerc = g / 31f;
                var bPerc = b / 31f;

                var red = (byte)((r << 3) | (byte)(0x7 * rPerc));
                var green = (byte)((g << 3) | (byte)(0x7 * rPerc));
                var blue = (byte)((b << 3) | (byte)(0x7 * rPerc));
                var bpp32 = 0xFF000000 | ((blue << 16) | (green << 8) | red);
                colors32.Add((uint)bpp32);

            }
            Uncompressed32 = colors32.ToArray();

        }

        public static List<List<byte>> DecompressTextures(List<byte> data, bool pad = false)
        {

            List<List<byte>> outputs = new List<List<byte>>();

            using (var br = new BinaryReader(new MemoryStream(data.ToArray())))
            {
                while (br.Position() < br.Length())
                {
                    var exit = false;
                    var output = new List<byte>();
                    if ((br.Position() & 0x7FF) > 0 && pad)
                    {
                        var fix = (br.Position() >> 11);
                        br.Position((++fix) << 11);
                    }

                    while (br.Position() < br.Length() && !exit)
                    {
                        var test = br.ReadUInt16(Endianness.BigEndian) << 16;
                        byte[] section = br.ReadBytes(0x20);
                        using (var b = new BinaryReader(new MemoryStream(section)))
                        {
                            if (test == 0)
                            {
                                while (b.Position() < b.Length())
                                {
                                    ushort read = b.ReadUInt16(Endianness.BigEndian);
                                    output.Add((byte)(read >> 8));
                                    output.Add((byte)read);
                                    System.Diagnostics.Debug.WriteLine($"{br.Position()}-{read >> 8:X2}{(byte)(read):X2}");
                                }
                            }
                            else
                            {
                                var encodeflags = (uint)(test | 0x00008000);

                                while (b.Position() < b.Length() && encodeflags != 0x80000000)
                                {
                                    if (output.Count() >= 0x2c32)
                                    {
                                        var r = output.ToList();
                                        r.Reverse();

                                    }
                                    var isCode = (0x80000000 & encodeflags) > 0;
                                    encodeflags <<= 1;
                                    ushort read = b.ReadUInt16(Endianness.BigEndian);
                                    if (isCode)
                                    {
                                        if (read == 0)
                                        {
                                            exit = true;
                                            br.Position(br.Position() - b.Length() + b.Position());
                                            break;
                                        }
                                        var cnt = (ushort)(read & 0x1F);
                                        read ^= cnt;
                                        read >>= 4;
                                        cnt++;
                                        var goback = output.Count() - read;
                                        for (var i = 0; i <= cnt * 2; i += 2)
                                        {
                                            var position = goback + i;
                                            var extra = (position - output.Count() & 3);
                                            var byte0 = (byte)(position < 0 || position == output.Count() ? extra == 1 ? 0xF6 : 0xFF : output[position]);
                                            var byte1 = (byte)((position + 1) < 0 || position == output.Count() ? extra == 1 ? 0xF6 : 0xFF : output[position + 1]);

                                            output.Add(byte0);
                                            output.Add(byte1);
                                        }
                                    }
                                    else
                                    {
                                        output.Add((byte)(read >> 8));
                                        output.Add((byte)read);
                                    }
                                }
                            }
                        }

                    }
                    if (output.Count() > 0) outputs.Add(output);
                }
                return outputs;
            }

        }
    }


}
