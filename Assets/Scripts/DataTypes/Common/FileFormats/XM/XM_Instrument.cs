using System.Collections.Generic;
using System.Text;

namespace R1Engine
{
    /// <summary>
    /// XM audio file data
    /// </summary>
    public class XM_Instrument : R1Serializable {
        public uint InstrumentSize { get; set; } = 243; // This is for instrument with sample. 29 without
        public string InstrumentName { get; set; }
        public byte InstrumentType { get; set; } = 0;
        public ushort NumSamples { get; set; } = 1;

        public uint SampleHeaderSize { get; set; } = 40;
        public byte[] SampleKeymapAssignments { get; set; } = new byte[96];
        public ushort[] PointsForVolumeEnvelope { get; set; } = new ushort[24] {
            0,
            64,
            40,
            64,
            80,
            64,
            120,
            64,
            160,
            64,
            160,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };
        public ushort[] PointsForPanningEnvelope { get; set; } = new ushort[24] {
            0,
            32,
            40,
            32,
            80,
            32,
            120,
            32,
            160,
            32,
            160,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0
        };
        public byte NumVolumePoints { get; set; } = 5;
        public byte NumPanningPoints { get; set; } = 5;
        public byte VolumeSustainPoint { get; set; } = 2;
        public byte VolumeLoopStartPoint { get; set; } = 3;
        public byte VolumeLoopEndPoint { get; set; } = 4;
        public byte PanningSustainPoint { get; set; } = 2;
        public byte PanningLoopStartPoint { get; set; } = 3;
        public byte PanningLoopEndPoint { get; set; } = 4;
        public byte VolumeType { get; set; }
        public byte PanningType { get; set; }
        public byte VibratoType { get; set; }
        public byte VibratoSweep { get; set; }
        public byte VibratoDepth { get; set; }
        public byte VibratoRate { get; set; }
        public ushort VolumeFadeout { get; set; } = 128;
        public byte[] Reserved { get; set; } = new byte[2];

        public XM_Sample[] Samples { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            InstrumentSize = s.Serialize<uint>(InstrumentSize, name: nameof(InstrumentSize));
            InstrumentName = s.SerializeString(InstrumentName, 22, Encoding.ASCII, name: nameof(InstrumentName));
            InstrumentType = s.Serialize<byte>(InstrumentType, name: nameof(InstrumentType));
            NumSamples = s.Serialize<ushort>(NumSamples, name: nameof(NumSamples));
            if (NumSamples > 0) {
                SampleHeaderSize = s.Serialize<uint>(SampleHeaderSize, name: nameof(SampleHeaderSize));
                SampleKeymapAssignments = s.SerializeArray<byte>(SampleKeymapAssignments, 96, name: nameof(SampleKeymapAssignments));
                PointsForVolumeEnvelope = s.SerializeArray<ushort>(PointsForVolumeEnvelope, 24, name: nameof(PointsForVolumeEnvelope));
                PointsForPanningEnvelope = s.SerializeArray<ushort>(PointsForPanningEnvelope, 24, name: nameof(PointsForPanningEnvelope));

                NumVolumePoints = s.Serialize<byte>(NumVolumePoints, name: nameof(NumVolumePoints));
                NumPanningPoints = s.Serialize<byte>(NumPanningPoints, name: nameof(NumPanningPoints));
                VolumeSustainPoint = s.Serialize<byte>(VolumeSustainPoint, name: nameof(VolumeSustainPoint));
                VolumeLoopStartPoint = s.Serialize<byte>(VolumeLoopStartPoint, name: nameof(VolumeLoopStartPoint));
                VolumeLoopEndPoint = s.Serialize<byte>(VolumeLoopEndPoint, name: nameof(VolumeLoopEndPoint));
                PanningSustainPoint = s.Serialize<byte>(PanningSustainPoint, name: nameof(PanningSustainPoint));
                PanningLoopStartPoint = s.Serialize<byte>(PanningLoopStartPoint, name: nameof(PanningLoopStartPoint));
                PanningLoopEndPoint = s.Serialize<byte>(PanningLoopEndPoint, name: nameof(PanningLoopEndPoint));
                VolumeType = s.Serialize<byte>(VolumeType, name: nameof(VolumeType));
                PanningType = s.Serialize<byte>(PanningType, name: nameof(PanningType));
                VibratoType = s.Serialize<byte>(VibratoType, name: nameof(VibratoType));
                VibratoSweep = s.Serialize<byte>(VibratoSweep, name: nameof(VibratoSweep));
                VibratoDepth = s.Serialize<byte>(VibratoDepth, name: nameof(VibratoDepth));
                VibratoRate = s.Serialize<byte>(VibratoRate, name: nameof(VibratoRate));
                VolumeFadeout = s.Serialize<ushort>(VolumeFadeout, name: nameof(VolumeFadeout));
                Reserved = s.SerializeArray<byte>(Reserved, 2, name: nameof(Reserved));

                Samples = s.SerializeObjectArray<XM_Sample>(Samples, NumSamples, name: nameof(Samples));
            }
        }
    }
}