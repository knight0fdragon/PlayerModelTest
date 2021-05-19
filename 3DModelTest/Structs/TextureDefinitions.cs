using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PlayerModelTest
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x800)]
    public struct TextureDefinitions
    {
        public uint DefinitionOffset;
        public uint NumberOfTextures;
        public uint TextureBankSize;
        public ulong Padding;
        public List<Texture> Definitions;
        public TextureDefinitions(byte[] data)
        {
            Definitions = new List<Texture>();
            using (var s = new MemoryStream(data))
            using (var br = new BinaryReader(s))
            {
                DefinitionOffset = br.ReadUInt32(Endianness.BigEndian);
                NumberOfTextures = br.ReadUInt32(Endianness.BigEndian);
                TextureBankSize = br.ReadUInt32(Endianness.BigEndian);
                Padding = br.ReadUInt64(Endianness.BigEndian);

                for (var t = 0; t < NumberOfTextures; t++)
                {
                    var width = br.ReadUInt16(Endianness.BigEndian);
                    var height = br.ReadUInt16(Endianness.BigEndian);
                    var addr = br.ReadUInt16(Endianness.BigEndian);
                    var size = br.ReadUInt16(Endianness.BigEndian);
                    var tex = new Texture() { Width = width, Height = height, CharacterGraphicsAddress = addr, Size = size };
                    Definitions.Add(tex);
                }
            }
        }

    }
}
