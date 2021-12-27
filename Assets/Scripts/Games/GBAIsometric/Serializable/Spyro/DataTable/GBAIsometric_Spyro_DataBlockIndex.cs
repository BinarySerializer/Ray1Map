using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_DataBlockIndex : GBAIsometric_Spyro_DataRef
    {
        public static HashSet<ushort> UsedIndices { get; } = new HashSet<ushort>();

        public bool Pre_HasPadding { get; set; }

        public ushort Index { get; set; }

        public override void DoAt(Action<long> action)
        {
            Context.GetStoredObject<GBAIsometric_Spyro_DataTable>(nameof(GBAIsometric_Spyro_ROM.DataTable)).
                DoAtBlock(Context, Index, action);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));

            if (Pre_HasPadding)
                s.SerializePadding(2, logIfNotNull: true);

            UsedIndices.Add(Index);
        }

        public static GBAIsometric_Spyro_DataBlockIndex FromIndex(SerializerObject s, ushort index)
        {
            var i = new GBAIsometric_Spyro_DataBlockIndex()
            {
                Index = index
            };

            i.Init(s.CurrentPointer);

            UsedIndices.Add(index);

            return i;
        }
    }
}