using System;
using System.IO;
using System.Linq;

namespace PlayerModelTest
{
    public class SGLPlayerModel
    {

        public TextureDefinitions TextureDefinitions;
        public Textures Textures;
        public Meshes Meshes;
        public Animations Animations;


        public SGLPlayerModel(string filename)
        {
            using (var f = new FileStream(filename, FileMode.Open))
            using (var br = new BinaryReader(f))
            {
                var chunk = br.ReadBytes(0x800);
                var Header = new Header(chunk);
                br.Position(Header.DefinitionChunk.Offset);
                var def = br.ReadBytes(Header.DefinitionChunk.ChunkSize);

                TextureDefinitions = new TextureDefinitions(def);
                br.Position(Header.TextureChunk.Offset);
                var tex = br.ReadBytes(Header.TextureChunk.ChunkSize);
                Textures = new Textures(tex, Header.TextureChunk.DataSize);
                br.Position(Header.MeshChunk.Offset);

                var mesh = br.ReadBytes(Header.MeshChunk.ChunkSize);
                Meshes = new Meshes(mesh, Header.MeshChunk.DataSize);

                br.Position(Header.UnknownChunk.Offset);
                var bones = br.ReadBytes(Header.UnknownChunk.ChunkSize);
                Animations = new Animations(bones, Header.UnknownChunk.DataSize);

                var textureOffset = 0;

                foreach (var tDef in TextureDefinitions.Definitions)
                {

                    try
                    {
                        tDef.Data = Textures.Uncompressed.ToList().GetRange(textureOffset, tDef.TrueSize * 2).ToArray();
                        tDef.Data32 = Textures.Uncompressed32.ToList().GetRange(textureOffset / 2, tDef.TrueSize).ToArray();
                        textureOffset += tDef.TrueSize * 2;
                    }
                    catch (Exception ex)
                    {
                        ex.ToString(); //TODO: Implement
                    }

                }

            }

        }
    }
 







}
