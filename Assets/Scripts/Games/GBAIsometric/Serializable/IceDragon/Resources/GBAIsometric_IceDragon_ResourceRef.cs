using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_ResourceRef : GBAIsometric_IceDragon_DataRef
    {
        public static HashSet<ushort> UsedIndices { get; } = new HashSet<ushort>();

        public bool Pre_HasPadding { get; set; }

        public ushort Index { get; set; }

        public override void DoAt(Action<long> action)
        {
            Context.GetRequiredStoredObject<GBAIsometric_IceDragon_Resources>(nameof(GBAIsometric_Dragon_ROM.Resources)).
                DoAtResource(Context, Index, action);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));

            if (Pre_HasPadding)
                s.SerializePadding(2, logIfNotNull: true);

            UsedIndices.Add(Index);
        }

        public static GBAIsometric_IceDragon_ResourceRef FromIndex(SerializerObject s, ushort index)
        {
            var i = new GBAIsometric_IceDragon_ResourceRef()
            {
                Index = index
            };

            i.Init(s.CurrentPointer);

            UsedIndices.Add(index);

            return i;
        }
    }
}