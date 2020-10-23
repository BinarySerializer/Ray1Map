using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// XM audio file data
    /// </summary>
    public class XM_Sample : R1Serializable {
        public uint SampleLength { get; set; }
        public uint SampleLoopStart { get; set; }
        public uint SampleLoopLength { get; set; }

        public byte Volume { get; set; } = 64;
        public sbyte FineTune { get; set; }
        public byte Type { get; set; }
        public byte Panning { get; set; } = 128;
        public sbyte RelativeNoteNumber { get; set; } = 0;
        public byte DataType { get; set; } // 0 = PCM, 1 = 4-bit ADPCM
        public string SampleName { get; set; }

        public sbyte[] SampleData8 { get; set; } // Signed!
        public short[] SampleData16 { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            SampleLength = s.Serialize<uint>(SampleLength, name: nameof(SampleLength));
            SampleLoopStart = s.Serialize<uint>(SampleLoopStart, name: nameof(SampleLoopStart));
            SampleLoopLength = s.Serialize<uint>(SampleLoopLength, name: nameof(SampleLoopLength));
            Volume = s.Serialize<byte>(Volume, name: nameof(Volume));
            FineTune = s.Serialize<sbyte>(FineTune, name: nameof(FineTune));
            Type = s.Serialize<byte>(Type, name: nameof(Type));
            Panning = s.Serialize<byte>(Panning, name: nameof(Panning));
            RelativeNoteNumber = s.Serialize<sbyte>(RelativeNoteNumber, name: nameof(RelativeNoteNumber));
            DataType = s.Serialize<byte>(DataType, name: nameof(DataType));
            SampleName = s.SerializeString(SampleName, 22, Encoding.ASCII, name: nameof(SampleName));

            if (BitHelpers.ExtractBits(Type, 1, 4) == 1) {
                SampleData16 = s.SerializeArray<short>(SampleData16, SampleLength / 2, name: nameof(SampleData16));
            } else {
                SampleData8 = s.SerializeArray<sbyte>(SampleData8, SampleLength, name: nameof(SampleData8));
            }
        }
    }
}