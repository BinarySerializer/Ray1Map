using System.Linq;

namespace R1Engine
{
    public class GBAVV_Graphics : R1Serializable
    {
        public Pointer AnimSetsPointer { get; set; }
        public Pointer TileSetPointer { get; set; }
        public Pointer PalettesPointer { get; set; }
        public ushort AnimSetsCount { get; set; }
        public ushort PalettesCount { get; set; }

        // Serialized from pointers
        public GBAVV_AnimSet[] AnimSets { get; set; }
        public byte[] TileSet { get; set; }
        public GBAVV_ObjPalette[] Palettes { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion >= EngineVersion.GBAVV_CrashFusion && s.GameSettings.EngineVersion != EngineVersion.GBAVV_KidsNextDoorOperationSODA)
            {
                // Since animation sets are referenced directly in later games there is no array
                var pointers = s.GameSettings.GetGameManagerOfType<GBAVV_BaseManager>().GraphicsDataPointers;

                if (AnimSets == null)
                    AnimSets = new GBAVV_AnimSet[pointers.Length];

                for (int i = 0; i < pointers.Length; i++)
                    AnimSets[i] = s.DoAt(new Pointer(pointers[i], Offset.file),
                        () => s.SerializeObject<GBAVV_AnimSet>(AnimSets[i], name: $"{nameof(AnimSets)}[{i}]"));
            }
            else
            {
                AnimSetsPointer = s.SerializePointer(AnimSetsPointer, name: nameof(AnimSetsPointer));
                TileSetPointer = s.SerializePointer(TileSetPointer, name: nameof(TileSetPointer));
                PalettesPointer = s.SerializePointer(PalettesPointer, name: nameof(PalettesPointer));
                AnimSetsCount = s.Serialize<ushort>(AnimSetsCount, name: nameof(AnimSetsCount));
                PalettesCount = s.Serialize<ushort>(PalettesCount, name: nameof(PalettesCount));

                AnimSets = s.DoAt(AnimSetsPointer,
                    () => s.SerializeObjectArray<GBAVV_AnimSet>(AnimSets, AnimSetsCount, name: nameof(AnimSets)));

                var tileSetLength = AnimSets.SelectMany(x => x.AnimationFrames).Select(x =>
                    x.TileOffset + (x.TileShapes.Select(t =>
                        (GBAVV_BaseManager.TileShapes[t.ShapeIndex].x * GBAVV_BaseManager.TileShapes[t.ShapeIndex].y) /
                        2).Sum())).Max();
                TileSet = s.DoAt(TileSetPointer,
                    () => s.SerializeArray<byte>(TileSet, tileSetLength, name: nameof(TileSet)));
                Palettes = s.DoAt(PalettesPointer,
                    () => s.SerializeObjectArray<GBAVV_ObjPalette>(Palettes, PalettesCount, name: nameof(Palettes)));
            }
        }
    }
}