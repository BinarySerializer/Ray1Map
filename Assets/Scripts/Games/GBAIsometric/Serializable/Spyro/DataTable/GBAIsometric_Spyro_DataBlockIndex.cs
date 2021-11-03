
using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_DataBlockIndex : BinarySerializable
    {
        public static HashSet<ushort> UsedIndices { get; } = new HashSet<ushort>();

        public bool HasPadding { get; set; }

        public ushort Index { get; set; }

        public T DoAtBlock<T>(Func<long, T> action)
            where T : class
        {
            return Context.GetStoredObject<GBAIsometric_Spyro_DataTable>(nameof(GBAIsometric_Spyro_ROM.DataTable)).DoAtBlock(Context, Index, action);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));

            if (HasPadding)
                s.Serialize<ushort>(default, name: "Padding");

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