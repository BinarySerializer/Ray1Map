using System.Linq;
using BinarySerializer;

namespace Ray1Map.GBAIsometric
{
    public class GBAIsometric_Spyro_Localization : BinarySerializable
    {
        public Pointer[] LocalizationPointers { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex[] LocalizationBlockIndices { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex[] LocalizationDecompressionBlockIndices { get; set; }

        public GBAIsometric_Spyro_LocTable[] LocTables { get; set; }

        public GBAIsometric_Spyro_SpriteMap FontTileMap { get; set; }
        public byte[] FontTileSet { get; set; }

        // Parsed
        public GBAIsometric_Spyro_LocBlock[] LocBlocks { get; set; }
        public GBAIsometric_Spyro_LocDecompress[][] LocDecompressHelpers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Get the language count for the current game
            var langCount = ((GBAIsometric_Spyro_Manager)s.GetR1Settings().GetGameManager).GetLanguages.Count();
            var pointerTable = PointerTables.GBAIsometric_Spyro_PointerTable(s.GetR1Settings().GameModeSelection, Offset.File);

            if (s.GetR1Settings().EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                // Parse loc tables
                LocalizationBlockIndices = s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.LocalizationBlockIndices), () => s.SerializeObjectArray<GBAIsometric_Spyro_DataBlockIndex>(LocalizationBlockIndices, langCount, x => x.HasPadding = true, name: nameof(LocalizationBlockIndices)));
                LocalizationDecompressionBlockIndices = s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.LocalizationDecompressionBlockIndices), () => s.SerializeObjectArray<GBAIsometric_Spyro_DataBlockIndex>(LocalizationDecompressionBlockIndices, langCount, x => x.HasPadding = true, name: nameof(LocalizationDecompressionBlockIndices)));

                LocTables = s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.LocTables), () => s.SerializeObjectArray<GBAIsometric_Spyro_LocTable>(LocTables, 38, name: nameof(LocTables)));

                // Parse block data

                if (LocDecompressHelpers == null)
                    LocDecompressHelpers = new GBAIsometric_Spyro_LocDecompress[langCount][];

                for (int i = 0; i < LocDecompressHelpers.Length; i++)
                    LocDecompressHelpers[i] = LocalizationDecompressionBlockIndices[i].DoAtBlock(size => s.SerializeObjectArray<GBAIsometric_Spyro_LocDecompress>(LocDecompressHelpers[i], size / 3, name: $"{nameof(LocDecompressHelpers)}[{i}]"));

                if (LocBlocks == null)
                    LocBlocks = new GBAIsometric_Spyro_LocBlock[langCount];

                for (int i = 0; i < LocBlocks.Length; i++)
                    LocBlocks[i] = LocalizationBlockIndices[i].DoAtBlock(size =>
                        s.SerializeObject<GBAIsometric_Spyro_LocBlock>(LocBlocks[i], onPreSerialize: lb =>
                        {
                            lb.Length = LocTables.Max(lt => lt.StartIndex + lt.NumEntries);
                            lb.DecompressHelpers = LocDecompressHelpers[i];
                        }, name: $"{nameof(LocBlocks)}[{i}]"));
            }
            else
            {
                LocalizationPointers = s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.LocalizationPointers), () => s.SerializePointerArray(LocalizationPointers, langCount, name: nameof(LocalizationPointers)));

                if (LocalizationPointers == null)
                    return;

                LocBlocks ??= new GBAIsometric_Spyro_LocBlock[langCount];

                for (int i = 0; i < LocBlocks.Length; i++)
                    LocBlocks[i] = s.DoAt(LocalizationPointers[i], () => s.SerializeObject<GBAIsometric_Spyro_LocBlock>(LocBlocks[i], name: $"{nameof(LocBlocks)}[{i}]"));
            }

            // Store the localization tables so we can access them to get the strings
            s.Context.StoreObject("Loc", LocBlocks.Select(x => x.Strings).ToArray());

            s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.FontTileMap), () =>
                FontTileMap = s.SerializeObject<GBAIsometric_Spyro_SpriteMap>(FontTileMap, name: nameof(FontTileMap)));

            s.DoAt(pointerTable.TryGetItem(Spyro_DefinedPointer.FontTileSet), () =>
                FontTileSet = s.SerializeArray<byte>(FontTileSet, (FontTileMap.MapData.Max(x => x.TileMapY) + 1) * 0x20, name: nameof(FontTileSet)));
        }
    }
}