using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class FLIC_ByteRun : BinarySerializable
    {
        public FLIC Flic { get; set; } // Set before serializing

        public FLIC_ByteRunLine[] Lines { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Lines = s.SerializeObjectArray<FLIC_ByteRunLine>(Lines, Flic.Height, x => x.Flic = Flic, name: nameof(Lines));
        }

        public class FLIC_ByteRunLine : BinarySerializable
        {
            public FLIC Flic { get; set; } // Set before serializing

            public byte PacketsCount { get; set; } // This value is a leftover
            public FLIC_ByteRunPacket[] Packets { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                PacketsCount = s.Serialize<byte>(PacketsCount, name: nameof(PacketsCount));

                var serializedPixels = 0;
                Packets = s.SerializeObjectArrayUntil(Packets, x =>
                {
                    serializedPixels += Math.Abs(x.Count);
                    return serializedPixels >= Flic.Width;
                }, includeLastObj: true, name: nameof(Packets));
            }

            public class FLIC_ByteRunPacket : BinarySerializable
            {
                public sbyte Count { get; set; }
                public byte[] ImageData { get; set; }
                public byte RepeatByte { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    Count = s.Serialize<sbyte>(Count, name: nameof(Count));

                    if (Count < 0)
                    {
                        ImageData = s.SerializeArray<byte>(ImageData, Math.Abs(Count), name: nameof(Count));
                    }
                    else
                    {
                        RepeatByte = s.Serialize<byte>(RepeatByte, name: nameof(RepeatByte));
                        ImageData = Enumerable.Repeat(RepeatByte, Count).ToArray();
                    }
                }
            }
        }
    }
}