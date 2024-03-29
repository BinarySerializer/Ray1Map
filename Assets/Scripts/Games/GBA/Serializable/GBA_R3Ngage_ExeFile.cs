﻿using BinarySerializer;
using Ray1Map.GBA;

namespace Ray1Map
{
    public class GBA_R3Ngage_ExeFile : BinarySerializable
    {
        public GBA_Localization Localization { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.File);

            s.DoAt(pointerTable[DefinedPointer.Localization], () => Localization = s.SerializeObject<GBA_Localization>(Localization, name: nameof(Localization)));
        }
    }
}