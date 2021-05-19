using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Media3D;

namespace PlayerModelTest
{
    public class XPData
    {
        public Point[] Verticies;
        public uint NumberOfVerticies;
        public Polygon[] Faces;
        public uint NumberOfFaces;

        public Vector16[] LightingVertices;

        public Vector CalculateVectorNormal(Vector V, Vector W)
        {
            var X = (int)((((long)V.Y * W.Z) - ((long)V.Z * W.Y)) >> 16);
            var Y = (int)((((long)V.Z * W.X) - ((long)V.X * W.Z)) >> 16);
            var Z = (int)((((long)V.X * W.Y) - ((long)V.Y * W.X)) >> 16);
            return new Vector() { X = X, Y = Y, Z = Z };
        }

        public Vector3D CalculateSurfaceNormal3D(Point A, Point B, Point C)
        {


            var U = (C.ToVector().ToVector3D() - A.ToVector().ToVector3D());//.ToVector();
            var V = (B.ToVector().ToVector3D() - A.ToVector().ToVector3D());//.ToVector();

            return Vector3D.CrossProduct(U, V);
        }

        public Vector64 CalculateSurfaceNormal64(Point A, Point B, Point C)
        {
            var U = (C - A);//.ToVector();
            var V = (B - A);//.ToVector();

            var UyVz = ((int)(((long)U.Y * V.Z) >> 16));// & 0xFFFFFFFF;
            var UzVy = ((int)(((long)U.Z * V.Y) >> 16));// & 0xFFFFFFFF;

            var UzVx = ((int)(((long)U.Z * V.X) >> 16));// & 0xFFFFFFFF;
            var UxVz = ((int)(((long)U.X * V.Z) >> 16));// & 0xFFFFFFFF;

            var UxVy = ((int)(((long)U.X * V.Y) >> 16));// & 0xFFFFFFFF;
            var UyVx = ((int)(((long)U.Y * V.X) >> 16));// & 0xFFFFFFFF;

            return new Vector64() { X = ((long)(UyVz - UzVy)) << 16, Y = ((long)(UzVx - UxVz)) << 16, Z = ((long)(UxVy - UyVx)) << 16 };

        }
        public Vector CalculateSurfaceNormal(Point A, Point B, Point C)
        {
            var U = (C - A).ToVector();
            var V = (B - A).ToVector();
            var X = ((((long)U.Y * V.Z) - ((long)U.Z * V.Y)));
            var Y = ((((long)U.Z * V.X) - ((long)U.X * V.Z)));
            var Z = ((((long)U.X * V.Y) - ((long)U.Y * V.X)));
            return new Vector() { X = (int)(X >> 16), Y = (int)(Y >> 16), Z = (int)(Z >> 16) };
        }

        public Vector64 CalculateSurfaceNormal(Point A, Point B, Point C, Point D)
        {

            var U = (D - A).ToVector();
            var V = (C - B).ToVector();


            var X = ((((long)U.Y * V.Z) - ((long)U.Z * V.Y)));
            var Y = ((((long)U.Z * V.X) - ((long)U.X * V.Z)));
            var Z = ((((long)U.X * V.Y) - ((long)U.Y * V.X)));
            return new Vector64() { X = X, Y = Y, Z = Z };
        }




        public void Fill(uint vertexAddr, uint faceAddr, uint attrAddr, uint lightAddr, byte[] data)
        {
            using (var s = new MemoryStream(data))
            using (var br = new BinaryReader(s))
            {
                br.Position(vertexAddr);
                var verticies = new List<Point>();
                for (var v = 0; v < NumberOfVerticies; v++)
                {
                    var point = new Point();
                    point.X = br.ReadInt32(Endianness.BigEndian);
                    point.Y = br.ReadInt32(Endianness.BigEndian);
                    point.Z = br.ReadInt32(Endianness.BigEndian);
                    verticies.Add(point);
                }
                Verticies = verticies.ToArray();

                br.Position(faceAddr);
                var faces = new List<Polygon>();
                for (var f = 0; f < NumberOfFaces; f++)
                {
                    var polygon = new Polygon();
                    polygon.Normal = new Vector();
                    polygon.Normal.X = br.ReadInt32(Endianness.BigEndian);
                    polygon.Normal.Y = br.ReadInt32(Endianness.BigEndian);
                    polygon.Normal.Z = br.ReadInt32(Endianness.BigEndian);
                    polygon.Verticies[0] = br.ReadUInt16(Endianness.BigEndian);
                    polygon.Verticies[1] = br.ReadUInt16(Endianness.BigEndian);
                    polygon.Verticies[2] = br.ReadUInt16(Endianness.BigEndian);
                    polygon.Verticies[3] = br.ReadUInt16(Endianness.BigEndian);

                    faces.Add(polygon);
                }
                Faces = faces.ToArray();

                br.Position(attrAddr);
                var attributes = new List<ATTR>();
                for (var a = 0; a < NumberOfFaces; a++)
                {
                    var attr = new ATTR();
                    attr.Flag = br.ReadByte();
                    attr.ZOrder = br.ReadByte();
                    attr.TextureNum = br.ReadUInt16(Endianness.BigEndian);
                    attr.Attribute = br.ReadUInt16(Endianness.BigEndian);
                    attr.ColorNumber = br.ReadUInt16(Endianness.BigEndian);
                    attr.GouraudShading = br.ReadUInt16(Endianness.BigEndian);
                    attr.Dir = br.ReadUInt16(Endianness.BigEndian); //Flip
                    faces[a].Attribute = attr;
                }
                br.Position(lightAddr);
                var light = new List<Vector16>();
                for (var v = 0; v < NumberOfVerticies; v++)
                {
                    var vector = new Vector16();
                    vector.X = br.ReadInt16(Endianness.BigEndian);
                    vector.Y = br.ReadInt16(Endianness.BigEndian);
                    vector.Z = br.ReadInt16(Endianness.BigEndian);
                    light.Add(vector);
                }
                LightingVertices = light.ToArray();
            }
        }
    }
}
