using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_IceDragon_MenuPage : BinarySerializable
    {
        public GBAIsometric_LocIndex Header { get; set; }
        public GBAIsometric_LocIndex SubHeader { get; set; }
        public uint Uint_08 { get; set; } // Position related, x offset?
        public Pointer OptionsPointer { get; set; }
        public ushort OptionsCount { get; set; }

        // Parsed
        public GBAIsometric_IceDragon_MenuOption[] Options { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Header = s.SerializeObject<GBAIsometric_LocIndex>(Header, x =>
            {
                x.Pre_Is32Bit = true;
                x.Pre_ParseIndexFunc = i => i == 0 ? -1 : i;
            }, name: nameof(Header));
            SubHeader = s.SerializeObject<GBAIsometric_LocIndex>(SubHeader, x =>
            {
                x.Pre_Is32Bit = true;
                x.Pre_ParseIndexFunc = i => i == 0 ? -1 : i;
            }, name: nameof(SubHeader));
            Uint_08 = s.Serialize<uint>(Uint_08, name: nameof(Uint_08));
            OptionsPointer = s.SerializePointer(OptionsPointer, name: nameof(OptionsPointer));
            OptionsCount = s.Serialize<ushort>(OptionsCount, name: nameof(OptionsCount));
            s.Serialize<ushort>(default, name: "Padding");

            s.DoAt(OptionsPointer, () => 
                Options = s.SerializeObjectArray<GBAIsometric_IceDragon_MenuOption>(Options, OptionsCount, name: nameof(Options)));
        }
    }
}