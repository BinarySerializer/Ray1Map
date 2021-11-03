﻿using BinarySerializer;

namespace Ray1Map.GBAVV
{
    public class GBAVV_SpongeBobRevengeOfTheFlyingDutchman_LevelInfo : BinarySerializable
    {
        public int SerializeDataForMap { get; set; } // Set before serializing

        public int MapsCount { get; set; }
        public uint MapsPointer { get; set; } // Memory pointer
        public int Int_08 { get; set; }

        // Serialized from pointers
        public Pointer[] MapsPointers { get; set; }
        public GBAVV_Generic_MapInfo[] Maps { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            MapsCount = s.Serialize<int>(MapsCount, name: nameof(MapsCount));
            MapsPointer = s.Serialize<uint>(MapsPointer, name: nameof(MapsPointer));
            Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));

            if (MapsPointer != 0)
            {
                var memOffset = s.GetR1Settings().GetGameManagerOfType<GBAVV_SpongeBobRevengeOfTheFlyingDutchman_Manager>().ROMMemPointersOffset;
                MapsPointers = s.DoAt(new Pointer(MapsPointer + memOffset, Offset.File), () => s.SerializePointerArray(MapsPointers, MapsCount, name: nameof(MapsPointers)));
            }

            if (Maps == null)
                Maps = new GBAVV_Generic_MapInfo[MapsCount];

            for (int i = 0; i < Maps.Length; i++)
            {
                if (SerializeDataForMap == i)
                    Maps[i] = s.DoAt(MapsPointers[i], () => s.SerializeObject<GBAVV_Generic_MapInfo>(Maps[i], name: $"{nameof(Maps)}[{i}]"));
            }
        }
    }
}