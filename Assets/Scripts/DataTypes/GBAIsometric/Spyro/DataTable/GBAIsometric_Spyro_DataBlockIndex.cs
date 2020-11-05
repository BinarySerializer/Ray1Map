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
            // 0 is the A button prompt for dialog boxes and is never referenced to from any struct, thus 0 is the default value when no index is set
            if (Index < 1)
                return null;

            return Context.GetStoredObject<GBAIsometric_Spyro_DataTable>(nameof(GBAIsometric_Spyro_ROM.DataTable)).DoAtBlock(Context, Index, action);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Index = s.Serialize<ushort>(Index, name: nameof(Index));

            if (HasPadding)
                s.Serialize<ushort>(default, name: "Padding");
        }
    }
}