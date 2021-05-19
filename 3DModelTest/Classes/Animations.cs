using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerModelTest
{

    public class Animations
    {
        private byte[] MatrixData;
        public List<Animation> AnimationEvents = new List<Animation>();

        public Animations(byte[] data, int size)
        {

            MatrixData = data.Take(size).ToArray();
            File.WriteAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "Bones.bin"), data);

            using (var s = new MemoryStream(data))
            using (var br = new BinaryReader(s))
            {

                var check = br.ReadUInt32(Endianness.BigEndian);
                if (check == 0xFFFFFFFF) return;
                br.Position(br.Position() - 4);

                var firstRead = br.ReadUInt32(Endianness.BigEndian); // 0609D09C  (00006219)
                var secondRead = br.ReadUInt32(Endianness.BigEndian); //0609D0A0  (FFF514A0)
                var thirdRead = br.ReadUInt32(Endianness.BigEndian); //0609D0A4   (00002DA0)
                var fourthRead = br.ReadUInt32(Endianness.BigEndian); //0609D0A8  (0004B1F2) <---  Goes t0 0609d0b4
                var fifthRead = br.ReadUInt32(Endianness.BigEndian);  //0609D0AC  (000AEB60)
                var sixthRead = br.ReadUInt32(Endianness.BigEndian); //0609D0B0   (00039283)


                var offsets = br.ReadUInt32(Endianness.BigEndian);  //0609D03C
                //0609D034 = 002E801C
                br.Position(0x1C);

                while (true)

                {
                    var animationID = br.ReadUInt16(Endianness.BigEndian); //value + 002B8000 into 0609D148         //18  -000057A8
                    //  First checks for 0xFFF
                    //Then checks for 0x1000  Maybe Animation number
                    if (animationID == 0xFFFF) break;

                    var numberOfFrames = br.ReadUInt16(Endianness.BigEndian); //value + 002B8000 into 0609D148         //18  -000057A8
                    var unknown2 = br.ReadUInt16(Endianness.BigEndian); //value + 002B8000 into 0609D148         //18  -000057A8
                    var distanceFromEnemy = br.ReadUInt16(Endianness.BigEndian); //value + 002B8000 into 0609D148         //18  -000057A8
                    var addr = br.ReadUInt32(Endianness.BigEndian);


                    var b = new Animation(addr, animationID, numberOfFrames, unknown2, distanceFromEnemy);
                    AnimationEvents.Add(b);

                }

                foreach (var ani in AnimationEvents)
                {
                    br.Position(ani.GetAddress);
                    while (true)
                    {

                        var frame = br.ReadUInt16(Endianness.BigEndian);

                        if (frame == 0xFFFF) break;

                        var eventCode = br.ReadUInt16(Endianness.BigEndian);
                        ani.EventTiming.Add((frame, eventCode));
                    }
                }

                br.Position(offsets);
            }
        }
        public TransformMatrix CalculateMatrix(int frame, int set) => ProcessMatricies(MatrixData, frame, set);

        public int CalculateFrame(int r6, int r5, int r4, byte[] inputData)
        {

            var r0 = 0;
            for (; r0 < r5; r0++)
            {
                var r1 = (inputData[r4] << 8) | (inputData[r4 + 1]);
                if (r1 == r6) break;
                //R1 seems to be used to return a count indicator.
                r4 += 2;
            }
            return r0;
        }
        //0609546C
        public TransformMatrix ProcessMatricies(byte[] inputData, int index, int r6)
        {
            var inputSection = ((inputData[0x18] << 24) | (inputData[1 + 0x18] << 16) | (inputData[2 + 0x18] << 8) | inputData[3 + 0x18]) + (index << 6);

            var r5 = (inputData[inputSection] << 24) | (inputData[inputSection + 1] << 16) | (inputData[inputSection + 2] << 8) | inputData[inputSection + 3];
            var r4 = (inputData[inputSection + 0x0C] << 24) | (inputData[inputSection + 0x0C + 1] << 16) | (inputData[inputSection + 0x0C + 2] << 8) | inputData[inputSection + 0x0C + 3];
            var translateID = CalculateFrame(r6, r5, r4, inputData);

            r5 = (inputData[inputSection + 0x08] << 24) | (inputData[inputSection + 0x08 + 1] << 16) | (inputData[inputSection + 0x08 + 2] << 8) | inputData[inputSection + 0x08 + 3];
            r4 = (inputData[inputSection + 0x14] << 24) | (inputData[inputSection + 0x14 + 1] << 16) | (inputData[inputSection + 0x14 + 2] << 8) | inputData[inputSection + 0x14 + 3];
            var scaleID = CalculateFrame(r6, r5, r4, inputData);

            r5 = (inputData[inputSection + 0x04] << 24) | (inputData[inputSection + 0x04 + 1] << 16) | (inputData[inputSection + 0x04 + 2] << 8) | inputData[inputSection + 0x04 + 3];
            r4 = (inputData[inputSection + 0x10] << 24) | (inputData[inputSection + 0x10 + 1] << 16) | (inputData[inputSection + 0x10 + 2] << 8) | inputData[inputSection + 0x10 + 3];
            var rotationID = CalculateFrame(r6, r5, r4, inputData);

            var translate = new Point();
            var r0 = (inputData[inputSection + 0x18 + 4 * 0] << 24) | (inputData[inputSection + 0x18 + 4 * 0 + 1] << 16) | (inputData[inputSection + 0x18 + 4 * 0 + 2] << 8) | inputData[inputSection + 0x18 + 4 * 0 + 3] + (translateID << 2);
            var r1 = (inputData[r0] << 24) | (inputData[r0 + 1] << 16) | (inputData[r0 + 2] << 8) | inputData[r0 + 3];
            translate.X = r1;

            r0 = (inputData[inputSection + 0x18 + 4 * 1] << 24) | (inputData[inputSection + 0x18 + 4 * 1 + 1] << 16) | (inputData[inputSection + 0x18 + 4 * 1 + 2] << 8) | inputData[inputSection + 0x18 + 4 * 1 + 3] + (translateID << 2);
            r1 = (inputData[r0] << 24) | (inputData[r0 + 1] << 16) | (inputData[r0 + 2] << 8) | inputData[r0 + 3];
            translate.Y = r1;

            r0 = (inputData[inputSection + 0x18 + 4 * 2] << 24) | (inputData[inputSection + 0x18 + 4 * 2 + 1] << 16) | (inputData[inputSection + 0x18 + 4 * 2 + 2] << 8) | inputData[inputSection + 0x18 + 4 * 2 + 3] + (translateID << 2);
            r1 = (inputData[r0] << 24) | (inputData[r0 + 1] << 16) | (inputData[r0 + 2] << 8) | inputData[r0 + 3];
            translate.Z = r1;

            var scale = new Point();


            r0 = (inputData[inputSection + 0x34 + 0x4 * 0] << 24) | (inputData[inputSection + 0x34 + 4 * 0 + 1] << 16) | (inputData[inputSection + 0x34 + 0x4 * 0 + 2] << 8) | inputData[inputSection + 0x34 + 0x4 * 0 + 3] + (scaleID << 2);
            r1 = (inputData[r0] << 24) | (inputData[r0 + 1] << 16) | (inputData[r0 + 2] << 8) | inputData[r0 + 3];
            scale.X = r1;
            r0 = (inputData[inputSection + 0x34 + 0x4 * 1] << 24) | (inputData[inputSection + 0x34 + 4 * 1 + 1] << 16) | (inputData[inputSection + 0x34 + 0x4 * 1 + 2] << 8) | inputData[inputSection + 0x34 + 0x4 * 1 + 3] + (scaleID << 2);
            r1 = (inputData[r0] << 24) | (inputData[r0 + 1] << 16) | (inputData[r0 + 2] << 8) | inputData[r0 + 3];
            scale.Y = r1;
            r0 = (inputData[inputSection + 0x34 + 0x4 * 2] << 24) | (inputData[inputSection + 0x34 + 4 * 2 + 1] << 16) | (inputData[inputSection + 0x34 + 0x4 * 2 + 2] << 8) | inputData[inputSection + 0x34 + 0x4 * 2 + 3] + (scaleID << 2);
            r1 = (inputData[r0] << 24) | (inputData[r0 + 1] << 16) | (inputData[r0 + 2] << 8) | inputData[r0 + 3];
            scale.Z = r1;

            var rotation = new Rotation();


            r0 = (inputData[inputSection + 0x24 + 4 * 0] << 24) | (inputData[inputSection + 0x24 + 4 * 0 + 1] << 16) | (inputData[inputSection + 0x24 + 4 * 0 + 2] << 8) | inputData[inputSection + 0x24 + 4 * 0 + 3] + (rotationID << 1);
            r1 = ((short)((inputData[r0] << 8) | inputData[r0 + 1])) << 2;
            rotation.X = r1;

            r0 = (inputData[inputSection + 0x24 + 4 * 1] << 24) | (inputData[inputSection + 0x24 + 4 * 1 + 1] << 16) | (inputData[inputSection + 0x24 + 4 * 1 + 2] << 8) | inputData[inputSection + 0x24 + 4 * 1 + 3] + (rotationID << 1);
            r1 = ((short)((inputData[r0] << 8) | inputData[r0 + 1])) << 2;
            rotation.Y = r1;
            r0 = (inputData[inputSection + 0x24 + 4 * 2] << 24) | (inputData[inputSection + 0x24 + 4 * 2 + 1] << 16) | (inputData[inputSection + 0x24 + 4 * 2 + 2] << 8) | inputData[inputSection + 0x24 + 4 * 2 + 3] + (rotationID << 1);
            r1 = ((short)((inputData[r0] << 8) | inputData[r0 + 1])) << 2;
            rotation.Z = r1;
            r0 = (inputData[inputSection + 0x24 + 4 * 3] << 24) | (inputData[inputSection + 0x24 + 4 * 3 + 1] << 16) | (inputData[inputSection + 0x24 + 4 * 3 + 2] << 8) | inputData[inputSection + 0x24 + 4 * 3 + 3] + (rotationID << 1);
            r1 = ((short)((inputData[r0] << 8) | inputData[r0 + 1])) << 2;
            rotation.Angle = r1;
            var transform = new TransformMatrix();
            transform.Translate = translate;
            transform.Scale = scale;
            transform.Rotation = rotation;
            transform.TranslateID = translateID + 1; // 060B8094
            transform.ScaleID = scaleID + 1; // 060B8096
            transform.RotateID = rotationID + 1; //060B8098
            return transform;
        }

    }
}
