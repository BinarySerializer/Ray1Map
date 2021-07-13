using BinarySerializer;
using BinarySerializer.PS1;

namespace R1Engine
{
    // Backgrounds
    public class PS1Klonoa_ArchiveFile_BackgroundPack : PS1Klonoa_ArchiveFile
    {
        public PS1Klonoa_ArchiveFile<PS1_TIM> TIMFiles { get; set; } // Tilesets
        public PS1Klonoa_ArchiveFile<PS1_CEL> CELFiles { get; set; } // Tiles
        public PS1Klonoa_ArchiveFile<PS1_BGD> BGDFiles { get; set; } // Maps
        public PS1Klonoa_ArchiveFile<TileAnimations> TileAnimationFiles { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            base.SerializeImpl(s);

            TIMFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1_TIM>>(s, TIMFiles, 0, name: nameof(TIMFiles));
            CELFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1_CEL>>(s, CELFiles, 1, name: nameof(CELFiles));
            BGDFiles = SerializeFile<PS1Klonoa_ArchiveFile<PS1_BGD>>(s, BGDFiles, 2, name: nameof(BGDFiles));
            TileAnimationFiles = SerializeFile<PS1Klonoa_ArchiveFile<TileAnimations>>(s, TileAnimationFiles, 3, name: nameof(TileAnimationFiles));
        }

        // TODO: Not totally sure about this, but some backgrounds should animate so this might be it?
        public class TileAnimations : PS1Klonoa_BaseFile
        {
            public int Count { get; set; }
            public Entry[] Entries { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Count = s.Serialize<int>(Count, name: nameof(Count));
                Entries = s.SerializeObjectArray<Entry>(Entries, Count, name: nameof(Entries));
            }

            public class Entry : BinarySerializable
            {
                public byte[] Data { get; set; }

                public override void SerializeImpl(SerializerObject s)
                {
                    Data = s.SerializeArray<byte>(Data, 64, name: nameof(Data));
                }
            }
        }
    }
}