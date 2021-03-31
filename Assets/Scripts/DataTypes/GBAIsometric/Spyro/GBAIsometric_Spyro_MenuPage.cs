using BinarySerializer;

namespace R1Engine
{
    public class GBAIsometric_Spyro_MenuPage : BinarySerializable
    {
        public GBAIsometric_LocIndex Header { get; set; }
        public GBAIsometric_LocIndex SubHeader { get; set; }
        public uint Uint_08 { get; set; }
        public Pointer OptionsPointer { get; set; }
        public ushort OptionsCount { get; set; }

        // Parsed
        public GBAIsometric_Spyro_MenuOption[] Options { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.SerializeObject<GBAIsometric_LocIndex>(Header, x => x.ParseIndexFunc = i => i == 0 ? -1 : i, name: nameof(Header));
            s.Serialize<ushort>(default, name: "Padding");
            SubHeader = s.SerializeObject<GBAIsometric_LocIndex>(SubHeader, x => x.ParseIndexFunc = i => i == 0 ? -1 : i, name: nameof(SubHeader));
            s.Serialize<ushort>(default, name: "Padding");
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
            OptionsPointer = s.SerializePointer(OptionsPointer, name: nameof(OptionsPointer));
            OptionsCount = s.Serialize<ushort>(OptionsCount, name: nameof(OptionsCount));
            s.Serialize<ushort>(default, name: "Padding");

            Options = s.DoAt(OptionsPointer, () => s.SerializeObjectArray<GBAIsometric_Spyro_MenuOption>(Options, OptionsCount, name: nameof(Options)));
        }
    }
}