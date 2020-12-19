namespace R1Engine {
    /// <summary>
    /// Palette block for GBA
    /// </summary>
    public class GBA_Palette : GBA_BaseBlock {
        public uint MadTrax_Uint_00 { get; set; }
        public uint MadTrax_Uint_04 { get; set; }
        public uint Length { get; set; }
        public BaseColor[] Palette { get; set; }

        public override void SerializeBlock(SerializerObject s) 
        {
            if (s.GameSettings.GBA_IsShanghai)
                s.Goto(ShanghaiOffsetTable.GetPointer(0));

            Length = s.Serialize<uint>(Length, name: nameof(Length));

            if (s.GameSettings.GBA_IsShanghai)
                s.Goto(ShanghaiOffsetTable.GetPointer(1));

            if (s.GameSettings.EngineVersion == EngineVersion.GBA_SplinterCell_NGage) {
                Palette = s.SerializeObjectArray<BGRA4441Color>((BGRA4441Color[])Palette, Length, name: nameof(Palette));
            } else {
                Palette = s.SerializeObjectArray<RGBA5551Color>((RGBA5551Color[])Palette, Length, name: nameof(Palette));
            }
        }

        public override long GetShanghaiOffsetTableLength => 2;
    }
}