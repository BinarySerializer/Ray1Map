using System.Text;
using BinarySerializer;

namespace Ray1Map {
    public class MusyX_Sample : BinarySerializable {
        public uint Length { get; set; }
        public int LoopStart { get; set; }
        public ushort SampleRate { get; set; }
        public ushort BaseNote { get; set; } // usually 0x3C (MIDI C4) 
        public uint UInt_0C { get; set; }
        public sbyte[] SampleData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            LoopStart = s.Serialize<int>(LoopStart, name: nameof(LoopStart));
            SampleRate = s.Serialize<ushort>(SampleRate, name: nameof(SampleRate));
            BaseNote = s.Serialize<ushort>(BaseNote, name: nameof(BaseNote));
            UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
            SampleData = s.SerializeArray<sbyte>(SampleData, Length, name: nameof(SampleData));
        }
    }
}