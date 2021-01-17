namespace R1Engine
{
    public class GBACrash_Isometric_CharacterIcon : R1Serializable
    {
        public Pointer TileSetPointer { get; set; }
        public uint Uint_04 { get; set; }

        // Serialized from pointers

        public GBACrash_TileSet TileSet { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));

            TileSet = s.DoAt(TileSetPointer, () => s.SerializeObject<GBACrash_TileSet>(TileSet, name: nameof(TileSet)));
        }
    }
}