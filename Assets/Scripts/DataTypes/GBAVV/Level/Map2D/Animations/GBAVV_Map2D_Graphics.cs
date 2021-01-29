using System.Linq;

namespace R1Engine
{
    public class GBAVV_Map2D_Graphics : R1Serializable
    {
        public Pointer AnimSetsPointer { get; set; }
        public Pointer TileSetPointer { get; set; }
        public Pointer PalettesPointer { get; set; }
        public ushort AnimSetsCount { get; set; }
        public ushort PalettesCount { get; set; }

        // Serialized from pointers
        public GBAVV_Map2D_AnimSet[] AnimSets { get; set; }
        public byte[] TileSet { get; set; }
        public GBAVV_Map2D_ObjPal[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            AnimSetsPointer = s.SerializePointer(AnimSetsPointer, name: nameof(AnimSetsPointer));
            TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
            PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));
            AnimSetsCount = s.Serialize<ushort>(AnimSetsCount, name: nameof(AnimSetsCount));
            PalettesCount = s.Serialize<ushort>(PalettesCount, name: nameof(PalettesCount));

            AnimSets = s.DoAt(AnimSetsPointer, () => s.SerializeObjectArray<GBAVV_Map2D_AnimSet>(AnimSets, AnimSetsCount, name: nameof(AnimSets)));

            var tileSetLength = AnimSets.SelectMany(x => x.AnimationFrames).Select(x =>
                x.TileOffset + (x.TileShapes.Select(t => (GBAVV_Map2D_AnimSet.TileShapes[t.ShapeIndex].x * GBAVV_Map2D_AnimSet.TileShapes[t.ShapeIndex].y) / 2).Sum())).Max();
            TileSet = s.DoAt(TileSetPointer, () => s.SerializeArray<byte>(TileSet, tileSetLength, name: nameof(TileSet)));
            Palettes = s.DoAt(PalettesPointer, () => s.SerializeObjectArray<GBAVV_Map2D_ObjPal>(Palettes, PalettesCount, name: nameof(Palettes)));
        }
    }
}