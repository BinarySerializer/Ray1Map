using System;

namespace R1Engine
{
    public class GBC_Pointer : R1Serializable
    {
        public ushort Palm_Ushort_00 { get; set; }
        public ushort Palm_FileIndex { get; set; }
        public ushort Palm_BlockIndex { get; set; }
        public ushort Palm_Ushort_06 { get; set; } // Padding?

        public ushort GBC_Offset { get; set; }
        public ushort GBC_Bank { get; set; }

        public T DoAtBlock<T>(Func<T> action)
            where T : class
        {
            var s = Context.Deserializer;

            if (Context.Settings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                if (Palm_Ushort_00 == 0)
                    return null;

                var filePath = @"jungle1.pdb"; // TODO: Get the file path from the file index!
                
                // TODO: Make sure the file is added to the context
                var dataBase = FileFactory.Read<Palm_Database>(filePath, Context, (serializerObject, database) => database.Type = Palm_Database.DatabaseType.PDB);
                var record = dataBase.Records[Palm_BlockIndex];

                return s.DoAt(record.DataPointer, action);
            }
            else
            {
                var offset = Offset.file.StartPointer + (0x4000 * GBC_Bank) + (GBC_Offset - 0x4000); // The ROM is split into memory banks, with the size 0x4000 which get loaded at 0x4000 in RAM.
                return s.DoAt(offset, action);
            }
        }

        public override void SerializeImpl(SerializerObject s)
        {
            if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1_Palm)
            {
                Palm_Ushort_00 = s.Serialize<ushort>(Palm_Ushort_00, name: nameof(Palm_Ushort_00));
                Palm_FileIndex = s.Serialize<ushort>(Palm_FileIndex, name: nameof(Palm_FileIndex));
                Palm_BlockIndex = s.Serialize<ushort>(Palm_BlockIndex, name: nameof(Palm_BlockIndex));
                Palm_Ushort_06 = s.Serialize<ushort>(Palm_Ushort_06, name: nameof(Palm_Ushort_06));
            }
            else
            {
                GBC_Offset = s.Serialize<ushort>(GBC_Offset, name: nameof(GBC_Offset));
                GBC_Bank = s.Serialize<ushort>(GBC_Bank, name: nameof(GBC_Bank));
            }
        }
    }
}