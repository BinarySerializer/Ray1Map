using System.Collections.Generic;
using System.Text;
using BinarySerializer;

namespace Ray1Map
{
    /// <summary>
    /// XM audio file data
    /// </summary>
    public class XM : BinarySerializable {
        public string IDText { get; set; } = "Extended Module: ";
        public string ModuleName { get; set; }
        public byte EscapeName { get; set;} = 0x1A;
        public string TrackerName { get; set; } = "FastTracker v2.00   ";
        public ushort VersionNumber { get; set; } = 0x0104;
        public uint HeaderSize { get; set; } = 20 + 256;
        public ushort SongLength { get; set; }
        public ushort RestartPosition { get; set; }
        public ushort NumChannels { get; set; }
        public ushort NumPatterns { get; set; }
        public ushort NumInstruments { get; set; }
        public ushort Flags { get; set; } = 1; // Linear Freq Table
        public ushort DefaultTempo { get; set; }
        public ushort DefaultBPM { get; set; }

        public byte[] PatternOrderTable { get; set; }
        public XM_Pattern[] Patterns { get; set; }
        public XM_Instrument[] Instruments { get; set; }


        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            IDText = s.SerializeString(IDText, 17, Encoding.ASCII, name: nameof(IDText));
            ModuleName = s.SerializeString(ModuleName, 20, Encoding.ASCII, name: nameof(ModuleName));
            EscapeName = s.Serialize<byte>(EscapeName, name: nameof(EscapeName));
            TrackerName = s.SerializeString(TrackerName, 20, name: nameof(TrackerName));
            VersionNumber = s.Serialize<ushort>(VersionNumber, name: nameof(VersionNumber));
            HeaderSize = s.Serialize<uint>(HeaderSize, name: nameof(HeaderSize));
            SongLength = s.Serialize<ushort>(SongLength, name: nameof(SongLength));
            RestartPosition = s.Serialize<ushort>(RestartPosition, name: nameof(RestartPosition));
            NumChannels = s.Serialize<ushort>(NumChannels, name: nameof(NumChannels));
            NumPatterns = s.Serialize<ushort>(NumPatterns, name: nameof(NumPatterns));
            NumInstruments = s.Serialize<ushort>(NumInstruments, name: nameof(NumInstruments));
            Flags = s.Serialize<ushort>(Flags, name: nameof(Flags));
            DefaultTempo = s.Serialize<ushort>(DefaultTempo, name: nameof(DefaultTempo));
            DefaultBPM = s.Serialize<ushort>(DefaultBPM, name: nameof(DefaultBPM));
            PatternOrderTable = s.SerializeArray<byte>(PatternOrderTable, HeaderSize - 20, name: nameof(PatternOrderTable));
            Patterns = s.SerializeObjectArray<XM_Pattern>(Patterns, NumPatterns, onPreSerialize: p => p.NumChannels = NumChannels, name: nameof(Patterns));
            Instruments = s.SerializeObjectArray<XM_Instrument>(Instruments, NumInstruments, name: nameof(Instruments));
        }
    }
}