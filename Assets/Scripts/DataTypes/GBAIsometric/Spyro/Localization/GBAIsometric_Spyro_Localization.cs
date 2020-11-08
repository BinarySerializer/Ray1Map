using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro_Localization : R1Serializable
    {
        public GBAIsometric_Spyro_DataBlockIndex[] LocalizationBlockIndices { get; set; }
        public GBAIsometric_Spyro_DataBlockIndex[] LocalizationDecompressionBlockIndices { get; set; }

        public GBAIsometric_Spyro_LocTable[] LocTables { get; set; }

        // Parsed
        public GBAIsometric_Spyro_LocBlock[] LocBlocks { get; set; }
        public GBAIsometric_Spyro_LocDecompress[][] LocDecompressHelpers { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Get the language count for the current game
            var langCount = ((GBAIsometric_Spyro_Manager)s.GameSettings.GetGameManager).GetLanguages.Count();
            var pointerTable = PointerTables.GBAIsometric_Spyro_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Parse loc tables
            LocalizationBlockIndices = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LocalizationBlockIndices), () => s.SerializeObjectArray<GBAIsometric_Spyro_DataBlockIndex>(LocalizationBlockIndices, langCount, x => x.HasPadding = true, name: nameof(LocalizationBlockIndices)));
            LocalizationDecompressionBlockIndices = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LocalizationDecompressionBlockIndices), () => s.SerializeObjectArray<GBAIsometric_Spyro_DataBlockIndex>(LocalizationDecompressionBlockIndices, langCount, x => x.HasPadding = true, name: nameof(LocalizationDecompressionBlockIndices)));

            LocTables = s.DoAt(pointerTable.TryGetItem(GBAIsometric_Spyro_Pointer.LocTables), () => s.SerializeObjectArray<GBAIsometric_Spyro_LocTable>(LocTables, 38, name: nameof(LocTables)));

            if (LocTables == null)
                return;

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

            // Store the localization tables so we can access them to get the strings
            s.Context.StoreObject("Loc", LocBlocks.Select(x => x.Strings).ToArray());
        }
    }
}