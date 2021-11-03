namespace Ray1Map.GBC
{
    public enum GBC_BlockType : ushort
    {
        TileKit = 0x04,
        PlayField = 0x05,
        MapBlock0 = 0x06,
        MapBlock1 = 0x07,
        MapBlock2 = 0x08,
        RomChannel = 0x09,
        Puppet = 0x0A,
        ActionTable = 0x0B,
        ActorModel = 0x0C,
        SoundBank = 0x0D,
        SoundProgram = 0xF,
        Scene = 0x10,
        Type_12 = 0x12,
        Level = 0x16,
        LevelList = 0x17,
        Vignette = 0x1A,
        DD_Menu = 0x1F, // Contains sub-blocks with vignette etc.
        VideoFrame = 0x25,
        Video = 0x26,
    }
}