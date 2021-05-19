using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace PlayerModelTest
{

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 0x800)]
    public struct Header
    {
        public ChunkEntry DefinitionChunk;
        public ChunkEntry TextureChunk;
        public ChunkEntry MeshChunk;
        public ChunkEntry UnknownChunk;

        public Header(byte[] data)
        {

            using (var s = new MemoryStream(data))
            using (var br = new BinaryReader(s))
            {
                var sizeOf = Marshal.SizeOf<ChunkEntry>();
                var chunk = br.ReadBytes(sizeOf);
                DefinitionChunk = new ChunkEntry(chunk);
                chunk = br.ReadBytes(sizeOf);
                TextureChunk = new ChunkEntry(chunk);
                chunk = br.ReadBytes(sizeOf);
                MeshChunk = new ChunkEntry(chunk);
                chunk = br.ReadBytes(sizeOf);
                UnknownChunk = new ChunkEntry(chunk);

            }
        }

        public byte[] Buffer => DefinitionChunk.Buffer.Concat(TextureChunk.Buffer.Concat(MeshChunk.Buffer.Concat(UnknownChunk.Buffer)).Concat(Enumerable.Repeat((byte)0x00, 0x800))).Take(0x800).ToArray();

    }
}
