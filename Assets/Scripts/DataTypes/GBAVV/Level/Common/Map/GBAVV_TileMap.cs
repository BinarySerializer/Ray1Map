using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_TileMap : BinarySerializable
    {
        public Encoding MapEncoding { get; set; } = Encoding.Columns; // Set before serializing

        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public uint[] TileMapSectionOffsets { get; set; }

        public GBAVV_TileMapSection[] TileMapSections { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Width = s.Serialize<ushort>(Width, name: nameof(Width));
            Height = s.Serialize<ushort>(Height, name: nameof(Height));

            TileMapSectionOffsets = s.SerializeArray<uint>(TileMapSectionOffsets, MapEncoding == Encoding.Columns ? Width : Height, name: nameof(TileMapSectionOffsets));

            if (TileMapSections == null)
                TileMapSections = new GBAVV_TileMapSection[TileMapSectionOffsets.Length];

            for (int i = 0; i < TileMapSections.Length; i++)
                TileMapSections[i] = s.DoAt(s.CurrentPointer + TileMapSectionOffsets[i] * 2, () => s.SerializeObject<GBAVV_TileMapSection>(TileMapSections[i], x => x.Length = (MapEncoding == Encoding.Columns ? Height : Width), name: $"{nameof(TileMapSections)}[{i}]"));
        }

        public enum Encoding
        {
            Rows,
            Columns
        }
    }
}