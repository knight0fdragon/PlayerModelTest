using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PlayerModelTest
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Animation
    {
        public byte Type => (byte)(AnimationID >> 12);
        public ushort AnimationID;
        public ushort NumberOfFrames;
        public ushort Flags;// 0x30 close up
        public ushort DistanceFromEnemy;


        public List<(ushort StartFrame, ushort EventCode)> EventTiming;

        private uint Address;
        public Animation(uint address, ushort animationID, ushort numOfFrames, ushort flags, ushort distanceFromEnemy)
        {
            AnimationID = animationID;
            NumberOfFrames = numOfFrames;
            Flags = flags;
            DistanceFromEnemy = distanceFromEnemy;
            EventTiming = new List<(ushort, ushort)>();
            Address = address;
        }

        public uint GetAddress => Address;

    }
}
