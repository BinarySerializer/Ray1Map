namespace R1Engine
{
    public class GBAIsometric_Spyro_ROM : GBA_ROMBase
    {
        public GBAIsometric_Spyro_DataTable DataTable { get; set; }
        
        public Pointer<GBAIsometric_Spyro_LevelInfo>[] LevelInfos { get; set; }

        public GBAIsometric_Spyro_UnkStruct1[] UnkStructs1 { get; set; }
        public GBAIsometric_Spyro_UnkStruct2[] UnkStructs2 { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // TODO: Don't hard-code pointers

            // Serialize primary data table and store it so we can get the data blocks
            DataTable = s.DoAt(Offset + 0x1c0b60, () => s.SerializeObject<GBAIsometric_Spyro_DataTable>(DataTable, name: nameof(DataTable)));
            s.Context.StoreObject(nameof(DataTable), DataTable);

            // Serialize level infos
            LevelInfos = s.DoAt(Offset + 0x1cfe38, () => s.SerializePointerArray<GBAIsometric_Spyro_LevelInfo>(LevelInfos, 80, resolve: true, name: nameof(LevelInfos)));

            // Serialize unknown structs
            UnkStructs1 = s.DoAt(Offset + 0x1c8024, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct1>(UnkStructs1, 196, name: nameof(UnkStructs1)));
            UnkStructs2 = s.DoAt(Offset + 0x1bf644, () => s.SerializeObjectArray<GBAIsometric_Spyro_UnkStruct2>(UnkStructs2, 38, name: nameof(UnkStructs2)));
        }
    }
}