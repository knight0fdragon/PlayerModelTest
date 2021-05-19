using System.IO;
using System.Runtime.InteropServices;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x10, Pack = 4)]
    public struct ChunkEntry
    {
        public uint Offset;
        public int DataSize;
        public int ChunkSize;
        public uint Padding;
        public ChunkEntry(byte[] data)
        {
            using (var s = new MemoryStream(data))
            using (var br = new BinaryReader(s))
            {
                Offset = br.ReadUInt32(Endianness.BigEndian);
                DataSize = br.ReadInt32(Endianness.BigEndian);
                ChunkSize = br.ReadInt32(Endianness.BigEndian);
                Padding = br.ReadUInt32(Endianness.BigEndian);
            }
        }

        public byte[] Buffer => _Buffer();

        private byte[] _Buffer()
        {
            using (var s = new MemoryStream())
            using (var bw = new BinaryWriter(s))
            {
                bw.Write(Offset, Endianness.BigEndian);
                bw.Write(DataSize, Endianness.BigEndian);
                bw.Write(ChunkSize, Endianness.BigEndian);
                bw.Write(Padding, Endianness.BigEndian);
                return s.ToArray();
            }
        }
    }
}
