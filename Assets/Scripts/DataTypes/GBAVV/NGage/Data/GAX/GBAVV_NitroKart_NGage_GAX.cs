using BinarySerializer;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_GAX : BinarySerializable
    {
        public long SongsCount { get; set; } // Set before serializing
        public int? SamplesCount { get; set; } // Set before serializing

        public string Magic { get; set; }
        public Pointer[] SongPointers { get; set; }

        // Serialized from pointers
        public GAX2_Song[] Songs { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Magic = s.SerializeString(Magic, 4, name: nameof(Magic));
            SongPointers = s.SerializePointerArray(SongPointers, SongsCount, name: nameof(SongPointers));

            if (Songs == null)
                Songs = new GAX2_Song[SongPointers.Length];

            for (int i = 0; i < Songs.Length; i++)
                Songs[i] = s.DoAt(SongPointers[i], () => s.SerializeObject(Songs[i], onPreSerialize: sng => sng.PredefinedSampleCount = SamplesCount, name: $"{nameof(Songs)}[{i}]"));

            s.Goto(Offset + s.CurrentLength);
        }
    }
}