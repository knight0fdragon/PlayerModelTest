using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerModelTest
{

    public class Meshes
    {
        public uint HeaderSize;
        public uint Offset;
        public uint BodyOffset;
        public uint WeaponOffset;
        //public ulong Padding;
        public List<XPData> BodyXPData = new List<XPData>();
        public List<XPData> WeaponXPData = new List<XPData>();
        public Meshes(byte[] data, int size)
        {
            data = data.Take(size).ToArray();
            using (var s = new MemoryStream(data))
            using (var br = new BinaryReader(s))
            {
                HeaderSize = br.ReadUInt32(Endianness.BigEndian);
                Offset = br.ReadUInt32(Endianness.BigEndian);
                BodyOffset = br.ReadUInt32(Endianness.BigEndian);
                WeaponOffset = br.ReadUInt32(Endianness.BigEndian);

                if (WeaponOffset == 0xFFFFFFFF) WeaponOffset = Offset;

                br.Position(BodyOffset);
                var cnt = 0;
                while (br.Position() < WeaponOffset)
                {
                    var check = br.ReadUInt32(Endianness.BigEndian);
                    if (check == 0xFFFFFFFF) break;

                    br.Position(br.Position() - 4);
                    var bodyXPData = new XPData();
                    var vertexPointer = br.ReadUInt32(Endianness.BigEndian);
                    bodyXPData.NumberOfVerticies = br.ReadUInt32(Endianness.BigEndian);
                    var facePointer = br.ReadUInt32(Endianness.BigEndian);
                    bodyXPData.NumberOfFaces = br.ReadUInt32(Endianness.BigEndian);
                    var attrPointer = br.ReadUInt32(Endianness.BigEndian);
                    var lightPointer = br.ReadUInt32(Endianness.BigEndian);
                    bodyXPData.Fill(vertexPointer, facePointer, attrPointer, lightPointer, data);
                    BodyXPData.Add(bodyXPData);
                    cnt++;
                }

                br.Position(WeaponOffset);

                while (br.Position() < Offset)
                {
                    var check = br.ReadUInt32(Endianness.BigEndian);
                    if (check == 0xFFFFFFFF) break;

                    br.Position(br.Position() - 4);
                    var weaponXPData = new XPData();
                    var vertexPointer = br.ReadUInt32(Endianness.BigEndian);
                    weaponXPData.NumberOfVerticies = br.ReadUInt32(Endianness.BigEndian);
                    var facePointer = br.ReadUInt32(Endianness.BigEndian);
                    weaponXPData.NumberOfFaces = br.ReadUInt32(Endianness.BigEndian);
                    var attrPointer = br.ReadUInt32(Endianness.BigEndian);
                    var lightPointer = br.ReadUInt32(Endianness.BigEndian);
                    weaponXPData.Fill(vertexPointer, facePointer, attrPointer, lightPointer, data);
                    WeaponXPData.Add(weaponXPData);
                }

                //TODO:something with offset
                var unknowns = new List<uint>();
                while (br.Position() < br.Length())
                {

                    var vertexPointer1 = br.ReadUInt32(Endianness.BigEndian);
                    unknowns.Add(vertexPointer1);
                }
            }
        }
    }

}
