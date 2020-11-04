using System.Linq;

namespace R1Engine
{
    public class GBAIsometric_Spyro_ROM : GBA_ROMBase
    {
        public GBAIsometric_Spyro_DataTable DataTable { get; set; }
        
        public GBAIsometric_Spyro_LevelData[][] LevelData { get; set; }

        public GBAIsometric_Spyro_PortraitSprite[] PortraitSprites { get; set; }
        public GBAIsometric_Spyro_Dialog[] DialogEntries { get; set; }

        public GBAIsometric_Spyro_UnkStruct1[] UnkStructs1 { get; set; }
        public GBAIsometric_Spyro_UnkStruct2[] UnkStructs2 { get; set; } // One for every level
        public byte[] UnkIndices { get; set; } // Indexes with the level ID

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var pointerTabe = PointerTables.GBAIsometric_Spyro_PointerTable(s.GameSettings.GameModeSelection, Offset.file);
            var manager = (GBAIsometric_Spyro_Manager)s.GameSettings.GetGameManager;

            // Serialize primary data table and store it so we can get the data blocks
            DataTable = s.DoAt(pointerTabe[GBAIsometric_Spyro_Pointer.DataTable], () => s.SerializeObject<GBAIsometric_Spyro_DataTable>(DataTable, name: nameof(DataTable)));
            s.Context.StoreObject(nameof(DataTable), DataTable);

            var levelInfo = manager.LevelInfos;

            if (LevelData == null)
                LevelData = new GBAIsometric_Spyro_LevelData[levelInfo.Length][];

            for (int world = 0; world < LevelData.Length; world++)
            {
                if (levelInfo[world].UsesPointerArray)
                {
                    var pointers = s.DoAt(new Pointer(levelInfo[world].Offsets[s.GameSettings.GameModeSelection], Offset.file), () => s.SerializePointerArray(default, levelInfo[world].Length, name: $"{nameof(LevelData)}Pointers[{world}]"));

                    if (LevelData[world] == null)
                        LevelData[world] = new GBAIsometric_Spyro_LevelData[levelInfo[world].Length];

                    for (int levelIndex = 0; levelIndex < LevelData[world].Length; levelIndex++)
                    {
                        LevelData[world][levelIndex] = s.DoAt(pointers[levelIndex], () => s.SerializeObject<GBAIsometric_Spyro_LevelData>(LevelData[world][levelIndex], x =>
                        {
                            x.Is2D = levelInfo[world].Is2D;
                            x.SerializeData = s.GameSettings.World == world;
                            x.ID = (uint)levelIndex; // Default to use the index of the entry - this will get replaced with an actual index if it exists
                        }, name: $"{nameof(LevelData)}[{world}][{levelIndex}]"));
                    }
                }
                else
                {
                    s.DoAt(new Pointer(levelInfo[world].Offsets[s.GameSettings.GameModeSelection], Offset.file), () =>
                    {
                        if (LevelData[world] == null)
                            LevelData[world] = new GBAIsometric_Spyro_LevelData[levelInfo[world].Length];

                        for (int levelIndex = 0; levelIndex < LevelData[world].Length; levelIndex++)
                        {
                            LevelData[world][levelIndex] = s.SerializeObject<GBAIsometric_Spyro_LevelData>(LevelData[world][levelIndex], x =>
                            {
                                x.Is2D = levelInfo[world].Is2D;
                                x.SerializeData = s.GameSettings.World == world;
                                x.ID = (uint)levelIndex; // Default to use the index of the entry - this will get replaced with an actual index if it exists
                            }, name: $"{nameof(LevelData)}[{world}][{levelIndex}]");
                        }
                    });
                }
            }

            if (s.GameSettings.EngineVersion == EngineVersion.GBAIsometric_Spyro3)
            {
                // TODO: Don't hard-code pointer - find for Spyro 2
                PortraitSprites = s.DoAt(Offset + 0x1bf644, () => s.SerializeObjectArray<GBAIsometric_Spyro_PortraitSprite>(PortraitSprites, 38, name: nameof(PortraitSprites)));
                DialogEntries = s.DoAt(Offset + 0x1bea54, () => s.SerializeObjectArray<GBAIsometric_Spyro_Dialog>(DialogEntries, 344, name: nameof(DialogEntries)));

                // Serialize unknown structs
                UnkStructs1 = s.DoAt(Offset + 0x1c8024, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct1>(UnkStructs1, 196, name: nameof(UnkStructs1)));
                UnkStructs2 = s.DoAt(Offset + 0x1d1f44, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct2>(UnkStructs2, 90, name: nameof(UnkStructs2)));
                UnkIndices = s.DoAt(Offset + 0x1c0030, () => s.SerializeArray<byte>(UnkIndices, 108, name: nameof(UnkIndices)));
            }
        }
    }
}