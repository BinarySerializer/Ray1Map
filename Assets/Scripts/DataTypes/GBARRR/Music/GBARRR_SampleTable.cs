using System;
using UnityEngine;

namespace R1Engine
{
    public class GBARRR_SampleTable : R1Serializable
    {
        public uint Length { get; set; }
        public Pointer Silence { get; set; }
        public uint Unknown { get; set; }
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Silence = s.SerializePointer(Silence, name: nameof(Silence));
            Unknown = s.Serialize<uint>(Unknown, name: nameof(Unknown));
            Entries = s.SerializeObjectArray<Entry>(Entries, Length, name: nameof(Entries));
        }

        public class Entry : R1Serializable {
            public Pointer SampleOffset { get; set; }
            public uint Length { get; set; }

            public byte[] Sample { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				SampleOffset = s.SerializePointer(SampleOffset, name: nameof(SampleOffset));
                Length = s.Serialize<uint>(Length, name: nameof(Length));

                s.DoAt(SampleOffset, () => {
                    Sample = s.SerializeArray<byte>(Sample, Length, name: nameof(Sample));
                });
			}
		}
    }
}