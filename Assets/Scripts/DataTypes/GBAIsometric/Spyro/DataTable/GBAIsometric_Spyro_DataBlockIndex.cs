using R1Engine.Serialize;
using System;

namespace R1Engine
{
    public class GBAIsometric_Spyro_DataBlockIndex : R1Serializable
    {
        public bool HasPadding { get; set; }

        public ushort Index { get; set; }

        public T DoAtBlock<T>(Func<uint, T> action)
            where T : class
        {
            return Context.GetStoredObject<GBAIsometric_Spyro_DataTable>(nameof(GBAIsometric_Spyro_ROM.DataTable)).DoAtBlock(Context, Index, action);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));

            if (HasPadding)
                s.Serialize<ushort>(default, name: "Padding");
        }

        public static GBAIsometric_Spyro_DataBlockIndex FromIndex(SerializerObject s, ushort index)
        {
            var i = new GBAIsometric_Spyro_DataBlockIndex()
            {
                Index = index
            };

            i.Init(s.CurrentPointer);

            return i;
        }
    }
}