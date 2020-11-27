using System.Linq;

namespace R1Engine
{
    public class GBC_PlayField : GBC_BaseBlock
    {
        public ushort ActorsCount { get; set; }
        public ushort ActorsOffset { get; set; }
        public ushort Ushort_04 { get; set; }
        public ushort SectorsOffset { get; set; }
        public ushort Ushort_08 { get; set; }
        public byte[] Bytes_0A { get; set; }
        public byte Index_Music { get; set; } // Index to the level music

        public ARGB1555Color[] GBC_ObjPalette { get; set; }
        public GBC_UnkActorStruct[] UnkActorStructs { get; set; }

        // Parsed from offsets
        public GBC_Actor[] Actors { get; set; }
        public GBC_Sector[] Sectors { get; set; }

        // Parsed from offset table
        public GBC_Map Map { get; set; }

        public override void SerializeBlock(SerializerObject s)
        {
            var blockOffset = s.CurrentPointer;

            // Serialize data (always little endian)
            s.DoEndian(R1Engine.Serialize.BinaryFile.Endian.Little, () => 
            {
                ActorsCount = s.Serialize<ushort>(ActorsCount, name: nameof(ActorsCount));
                ActorsOffset = s.Serialize<ushort>(ActorsOffset, name: nameof(ActorsOffset));
                Ushort_04 = s.Serialize<ushort>(Ushort_04, name: nameof(Ushort_04));
                SectorsOffset = s.Serialize<ushort>(SectorsOffset, name: nameof(SectorsOffset));
                Ushort_08 = s.Serialize<ushort>(Ushort_08, name: nameof(Ushort_08));
                Bytes_0A = s.SerializeArray<byte>(Bytes_0A, 5, name: nameof(Bytes_0A));
                Index_Music = s.Serialize<byte>(Index_Music, name: nameof(Index_Music));

                if (s.GameSettings.EngineVersion == EngineVersion.GBC_R1)
                {
                    GBC_ObjPalette = s.SerializeObjectArray<ARGB1555Color>(GBC_ObjPalette, 8 * 4, name: nameof(GBC_ObjPalette));
                    // TODO: Parse unknown bytes
                    // TODO: Parse array of UnkActorStructs (exists on PalmOS too!)
                }
                else
                {
                    // TODO: Parse data
                    // The PalmOS version has different data here (same length though), although it appears to begin with a palette as well which would be irrelevant since it uses the system palette. The data ends with the UnkActorStructs just like on GBC.
                }

                // Parse actors
                Actors = s.DoAt(blockOffset + ActorsOffset, () => s.SerializeObjectArray<GBC_Actor>(Actors, ActorsCount, name: nameof(Actors)));

                // Parse sectors
                s.DoAt(blockOffset + SectorsOffset, () =>
                {
                    // TODO: Get sector count (0x0D in first level)
                });
            });

            // Parse data from pointers
            Map = s.DoAt(OffsetTable.GetPointer(0), () => s.SerializeObject<GBC_Map>(Map, name: nameof(Map)));

            // Parse actor graphics
            foreach (var actor in Actors.Where(x => x.Index_GraphicsData > 1))
                actor.GraphicsData = s.DoAt(OffsetTable.GetPointer(actor.Index_GraphicsData - 1), () => s.SerializeObject<GBC_ActorGraphicsData>(actor.GraphicsData, name: $"{nameof(actor.GraphicsData)}[{actor.Index_GraphicsData}]"));
        }
    }
}