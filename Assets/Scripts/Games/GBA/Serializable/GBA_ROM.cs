using System.Linq;
using BinarySerializer;
using BinarySerializer.GBA;
using Ray1Map.GBA;

namespace Ray1Map
{
    public class GBA_ROM : GBA_ROMBase
    {
        // Game data
        public GBA_Data Data { get; set; }

        // Contains general info about levels, but not anything map related
        public GBA_R3_SceneInfo[] LevelInfo { get; set; }

        // Localization
        public GBA_LocLanguageTable Localization { get; set; }
        public GBA_Milan_LocTable Milan_Localization { get; set; }

        // Actor type data
        public GBA_ActorTypeTableEntry[] ActorTypeTable { get; set; }

        // Rayman 3 Single Pak
        public GBA_OffsetTable R3SinglePak_OffsetTable { get; set; }
        public GBA_Puppet[] R3SinglePak_Puppets { get; set; }
        public RGBA5551Color[] R3SinglePak_Palette { get; set; }
        public byte[] R3SinglePak_TileSet { get; set; }
        public ushort[] R3SinglePak_TileMap { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            var manager = ((GBA_Manager)s.Context.GetR1Settings().GetGameManager);

            // Get the pointer table
            var pointerTable = PointerTables.GBA_PointerTable(s.Context, Offset.File);
            var lvlType = manager.GetLevelType(s.Context);

            // Serialize the offset table
            if (lvlType != GBA_Manager.LevelType.R3SinglePak)
                s.DoAt(pointerTable[DefinedPointer.UiOffsetTable], () => Data = s.SerializeObject<GBA_Data>(Data, name: nameof(Data)));

            // Serialize level info
            if (pointerTable.ContainsKey(DefinedPointer.LevelInfo))
                LevelInfo = s.DoAt(pointerTable[DefinedPointer.LevelInfo], () => s.SerializeObjectArray<GBA_R3_SceneInfo>(LevelInfo, manager.LevelCount, name: nameof(LevelInfo)));

            // Serialize localization
            if (pointerTable.ContainsKey(DefinedPointer.Localization))
            {
                if (s.GetR1Settings().GBA_IsMilan)
                    s.DoAt(pointerTable[DefinedPointer.Localization], () => Milan_Localization = s.SerializeObject<GBA_Milan_LocTable>(Milan_Localization, name: nameof(Milan_Localization)));
                else
                    s.DoAt(pointerTable[DefinedPointer.Localization], () => Localization = s.SerializeObject<GBA_LocLanguageTable>(Localization, name: nameof(Localization)));
            }

            // Serialize actor type data
            if (pointerTable.ContainsKey(DefinedPointer.ActorTypeTable))
                ActorTypeTable = s.DoAt(pointerTable[DefinedPointer.ActorTypeTable], () => s.SerializeObjectArray<GBA_ActorTypeTableEntry>(ActorTypeTable, manager.ActorTypeTableLength, name: nameof(ActorTypeTable)));

            if (lvlType == GBA_Manager.LevelType.R3SinglePak)
            {
                R3SinglePak_OffsetTable = s.DoAt(pointerTable[DefinedPointer.R3SinglePak_OffsetTable], () => s.SerializeObject<GBA_OffsetTable>(R3SinglePak_OffsetTable, name: nameof(R3SinglePak_OffsetTable)));
                R3SinglePak_Palette = s.DoAt(pointerTable[DefinedPointer.R3SinglePak_Palette], () => s.SerializeObjectArray<RGBA5551Color>(R3SinglePak_Palette, 256, name: nameof(R3SinglePak_Palette)));
                R3SinglePak_TileMap = s.DoAt(pointerTable[DefinedPointer.R3SinglePak_TileMap], () => s.SerializeArray<ushort>(R3SinglePak_TileMap, 0x400, name: nameof(R3SinglePak_TileMap)));
                R3SinglePak_TileSet = s.DoAt(pointerTable[DefinedPointer.R3SinglePak_TileSet], () => s.SerializeArray<byte>(R3SinglePak_TileSet, (R3SinglePak_TileMap.Max() + 1) * 0x40, name: nameof(R3SinglePak_TileSet)));

                if (R3SinglePak_Puppets == null)
                    R3SinglePak_Puppets = new GBA_Puppet[R3SinglePak_OffsetTable.OffsetsCount];

                for (int i = 0; i < R3SinglePak_Puppets.Length; i++)
                    R3SinglePak_Puppets[i] = s.DoAt(R3SinglePak_OffsetTable.GetPointer(i), () => s.SerializeObject<GBA_Puppet>(R3SinglePak_Puppets[i], name: $"{nameof(R3SinglePak_Puppets)}[{i}]"));
            }
        }
    }
}