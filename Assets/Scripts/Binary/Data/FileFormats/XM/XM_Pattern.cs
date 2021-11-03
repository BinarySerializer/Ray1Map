using System.Collections.Generic;
using System.Text;
using BinarySerializer;

namespace Ray1Map
{
    /// <summary>
    /// XM audio file data
    /// </summary>
    public class XM_Pattern : BinarySerializable {
        // Set in onPreSerialize
        public int NumChannels { get; set; }

        public uint PatternHeaderLength { get; set; } = 9;
        public byte PackingType { get; set; } = 0;
        public ushort NumRows { get; set; }
        public ushort PackedPatternDataSize { get; set; }

        public XM_PatternRow[] PatternRows { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            PatternHeaderLength = s.Serialize<uint>(PatternHeaderLength, name: nameof(PatternHeaderLength));
            PackingType = s.Serialize<byte>(PackingType, name: nameof(PackingType));
            NumRows = s.Serialize<ushort>(NumRows, name: nameof(NumRows));
            PackedPatternDataSize = s.Serialize<ushort>(PackedPatternDataSize, name: nameof(PackedPatternDataSize));

            if (PatternRows == null) {
                PatternRows = s.SerializeObjectArray<XM_PatternRow>(PatternRows, NumChannels * NumRows, name: nameof(PatternRows));
            } else {
                PatternRows = s.SerializeObjectArray<XM_PatternRow>(PatternRows, PatternRows.Length, name: nameof(PatternRows));
            }
        }
    }
}