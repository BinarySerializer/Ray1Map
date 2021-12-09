using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map.GBARRR;
using System;
using System.Linq;

namespace Ray1Map
{
    public class GAX2_XMWriter {
        public XM ConvertToXM(GAX2_Song song) {
            XM xm = new XM();
            UnityEngine.Debug.Log($"Exporting song: {song.ParsedName}");
            xm.ModuleName = song.ParsedName;
            xm.Instruments = song.InstrumentIndices.Select((s,i) => GetInstrument(song,i)).ToArray();
            xm.NumInstruments = (ushort)xm.Instruments.Length;
            xm.NumChannels = song.NumChannels;
            xm.NumPatterns = song.NumPatternsPerChannel;
            xm.PatternOrderTable = Enumerable.Range(0,song.NumPatternsPerChannel).Select(i => (byte)i).ToArray();
            xm.SongLength = (ushort)xm.PatternOrderTable.Length;
            xm.DefaultTempo = 6;
            xm.DefaultBPM = 149;
            xm.Flags = 1;
            if (xm.PatternOrderTable.Length < 256) {
                byte[] patOrderTable = xm.PatternOrderTable;
                Array.Resize(ref patOrderTable, 256);
                xm.PatternOrderTable = patOrderTable;
            }
            xm.HeaderSize = (uint)(20 + xm.PatternOrderTable.Length);
            xm.Patterns = Enumerable.Range(0, song.NumPatternsPerChannel).Select(i => GetPattern(song, i)).ToArray();
            return xm;
        }
        public short[] ConvertSample(byte[] sample) {
            short[] delta = new short[sample.Length];
            int cur = 0;
            for (int i = 0; i < sample.Length; i++) {
                short b = (short)(sample[i] - 128);
                delta[i] = (short)((b - cur) * 256);
                cur = b;
            }
            return delta;
        }
        public XM_Instrument GetInstrument(GAX2_Song song, int index) {
            int ind = song.InstrumentIndices[index];
            var gax_instr = song.InstrumentSet[ind].Value;

            XM_Sample smp = new XM_Sample();
            smp.SampleName = "Sample " + gax_instr.Sample;
            smp.SampleData16 = ConvertSample(song.Samples[index].Sample);
            smp.Type = 1 << 4; // 16 bit sample data
            smp.SampleLength = (uint)smp.SampleData16.Length * 2;

            // If loop
            bool loop = gax_instr.LoopStart != 0 || gax_instr.LoopEnd != 0;
            if (loop) {
                smp.Type = (byte)BitHelpers.SetBits(smp.Type, 1, 2, 0);
                smp.SampleLoopStart = gax_instr.LoopStart * 2;
                smp.SampleLoopLength = (gax_instr.LoopEnd - gax_instr.LoopStart) * 2 - 2;
            }
            int instrPitch = (gax_instr.Pitch / 32);
            int relNote = (gax_instr.InstrumentConfig.Value.RelativeNoteNumber & 63);
            int relativeNoteNumber =
                instrPitch - 1 // GAX notes start at 1
                + (gax_instr.InstrumentConfig.Value.Byte_01 == 1 ? -1 : 1) * relNote - 2;
            UnityEngine.Debug.Log($"(Instrument {ind}) Sample:{gax_instr.Sample}" +
                $" - Pitch:{gax_instr.Pitch}" +
                $" - Relative Note Number:{relativeNoteNumber}" +
                $" - Cfg1:{gax_instr.InstrumentConfig.Value.Byte_01}" +
                $" - CfgRelNote:{relNote}" +
                $"");
            smp.RelativeNoteNumber = (sbyte)relativeNoteNumber;
            smp.FineTune = (sbyte)((gax_instr.Pitch - (instrPitch * 32)) * 4);
            if (ind == 0) {
                smp.RelativeNoteNumber = 0;
                smp.FineTune = 0;
            }

            XM_Instrument instr = new XM_Instrument();
            instr.InstrumentName = "Instrument " + ind;
            instr.Samples = new XM_Sample[] { smp };

            // Volume envelope
            instr.NumVolumePoints = gax_instr.Envelope.Value.NumPointsVolume;
            for (int i = 0; i < instr.NumVolumePoints; i++) {
                instr.PointsForVolumeEnvelope[i * 2 + 0] = gax_instr.Envelope.Value.Points[i].X;
                instr.PointsForVolumeEnvelope[i * 2 + 1] = (ushort)(gax_instr.Envelope.Value.Points[i].Y / 4);
                // The values in gax's envelope are 0-255 - in XM's they are 0-64.
                // However for GAX they all seem to be conversions from 0-64, i.e. no values that aren't 0 when doing % 4.
                // Excpet for 0xFF which should be treated as 64
            }
            instr.VolumeLoopEndPoint = (byte)Math.Max(instr.NumVolumePoints - 1,0);
            instr.VolumeLoopStartPoint = 0;
            instr.VolumeSustainPoint = instr.VolumeLoopEndPoint;
            instr.VolumeType = 1;
            instr.VolumeFadeout = 0x400;
            //if(loop) BitHelpers.SetBits(instr.VolumeType, 1, 1, 2); // Todo: find sustain point, loop start & end point, etc. Set sustain flag if necessary
            if (gax_instr.Envelope.Value.VolumeSustainPoint.HasValue) {
                instr.VolumeSustainPoint = gax_instr.Envelope.Value.VolumeSustainPoint.Value;
                BitHelpers.SetBits(instr.VolumeType, 1, 1, 1);
            }


            // Panning envelope
            /*instr.NumPanningPoints = gax_instr.Envelope.Value.NumPointsPanning;
            if (gax_instr.Envelope.Value.NumPointsPanning == 0xFF) {
                instr.NumPanningPoints = gax_instr.Envelope.Value.NumPointsVolume;
            } else {
                instr.NumPanningPoints = gax_instr.Envelope.Value.NumPointsPanning;
            }
            for (int i = 0; i < instr.NumPanningPoints; i++) {
                instr.PointsForPanningEnvelope[i * 2 + 0] = gax_instr.Envelope.Value.Points[i].X;
                instr.PointsForPanningEnvelope[i * 2 + 1] = 32;// gax_instr.Envelope.Value.Points[i].Short_02;
                //UnityEngine.Debug.Log($"PP: {gax_instr.Envelope.Value.Points[i].Short_02}");
            }
            instr.PanningLoopEndPoint = 0;
            instr.PanningLoopStartPoint = 0;
            instr.PanningSustainPoint = 0;*/
            return instr;
        }

        private XM_Pattern GetPattern(GAX2_Song song, int index) {
            XM_Pattern pat = new XM_Pattern();
            pat.NumChannels = song.NumChannels;
            pat.NumRows = song.NumRowsPerPattern;
            XM_PatternRow[] rows = Enumerable.Range(0, song.NumChannels).SelectMany(c => GetPatternRows(song, index, c, song.NumRowsPerPattern)).ToArray();
            pat.PatternRows = new XM_PatternRow[rows.Length];
            for (int row = 0; row < song.NumRowsPerPattern; row++) {
                for (int ch = 0; ch < song.NumChannels; ch++) {
                    pat.PatternRows[row * song.NumChannels + ch] = rows[ch * song.NumRowsPerPattern + row];
                }
            }
            // pat.PackedPatternDataSize = (ushort)(song.NumChannels * song.NumRowsPerPattern);
            pat.PackedPatternDataSize = (ushort)pat.PatternRows.Sum(p => p.Size);

            return pat;
        }

        private XM_PatternRow[] GetPatternRows(GAX2_Song song, int patternIndex, int channelIndex, int numRows) {
            GAX2_Pattern gax = song.Patterns[channelIndex][patternIndex];
            XM_PatternRow[] rows = new XM_PatternRow[numRows];
            int curRow = 0;
            byte? vol = null;
            byte? eff = null;
            byte? effParam = null;
            byte? note = null;
            int lastInstr = 0;
            if (gax.IsEmptyTrack) {
                while (curRow < rows.Length) {
                    rows[curRow++] = new XM_PatternRow();
                }
                return rows;
            }
            foreach (var row in gax.Rows) {
                eff = null;
                effParam = null;
                vol = null;
                note = null;

                void ConfigureEffect() {
                    /*eff = (row.Effect < 16 && row.Effect != 12) ? (byte?)row.Effect : null;
                    effParam = eff.HasValue ? (byte?)(row.EffectParameter) : null;
                    vol = (row.Effect == 12) ? (byte?)(0x10 + (row.EffectParameter >> 2)) : null;*/
                    eff = row.Effect;
                    if (row.Effect > 16) eff++;
                    effParam = row.EffectParameter;
                    switch (row.Effect) {
                        case 24:
                        case 32:
                        case 8:
                        case 9:
                        case 3:
                            // TODO
                            eff = null;
                            effParam = null;
                            break;
                        case 12: // Set volume
                            effParam >>= 2; // Volume ranges from 0-255 in GAX, and 0-64 in XM
                            break;
                    }
                }
                void ConfigureNote() {
                    if(row.Note != 0) note = row.Note;
                }

                switch (row.Command) {
                    // TODO: correct these
                    case GAX2_PatternRow.Cmd.Unknown:
                        break;
                    case GAX2_PatternRow.Cmd.EffectOnly:
                        ConfigureEffect();
                        rows[curRow++] = new XM_PatternRow(volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        break;
                    case GAX2_PatternRow.Cmd.Rest:
                        rows[curRow++] = new XM_PatternRow();
                        break;
                    case GAX2_PatternRow.Cmd.RestMulti:
                        for (int i = 0; i < row.RestDuration; i++) {
                            rows[curRow++] = new XM_PatternRow();
                        }
                        break;
                    case GAX2_PatternRow.Cmd.Note:
                        ConfigureEffect();
                        ConfigureNote();
                        if (row.Instrument == 250) { // Note off
                            rows[curRow++] = new XM_PatternRow(note: 97, volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        } else {
                            int instr = 1 + Array.IndexOf(song.InstrumentIndices, row.Instrument);
                            lastInstr = instr;
                            rows[curRow++] = new XM_PatternRow(note: note, instrument: (byte)instr, volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        }
                        break;
                    case GAX2_PatternRow.Cmd.NoteOnly:
                        ConfigureNote();
                        if (row.Instrument == 250) { // Note off
                            rows[curRow++] = new XM_PatternRow(note: 97);
                        } else {
                            int instr = 1 + Array.IndexOf(song.InstrumentIndices, row.Instrument);
                            lastInstr = instr;
                            rows[curRow++] = new XM_PatternRow(note: note, instrument: (byte)instr);
                        }
                        break;
                    case GAX2_PatternRow.Cmd.NoteOff:
                        rows[curRow++] = new XM_PatternRow(note: 97);
                        //rows[curRow++] = new XM_PatternRow(effectType: 0xe, effectParameter: 0xc0);
                        break;
                }
            }
            return rows;
        }
    }
}
