using System.Text;

namespace R1Engine {
    public class MusyX_Sample : R1Serializable {
        public uint Length { get; set; }
        public int Int_04 { get; set; }
        public ushort SampleRate { get; set; }
        public ushort UShort_0A { get; set; }
        public uint UInt_0C { get; set; }
        public sbyte[] SampleData { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            Length = s.Serialize<uint>(Length, name: nameof(Length));
            Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
            SampleRate = s.Serialize<ushort>(SampleRate, name: nameof(SampleRate));
            UShort_0A = s.Serialize<ushort>(UShort_0A, name: nameof(UShort_0A));
            UInt_0C = s.Serialize<uint>(UInt_0C, name: nameof(UInt_0C));
            SampleData = s.SerializeArray<sbyte>(SampleData, Length, name: nameof(SampleData));
        }
    }
}