using BinarySerializer;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Audio;
using BinarySerializer.GBA.Audio.GAX;

namespace Ray1Map
{
    public class GAX_XMWriter {

        public class ConsoleLog {
            public void Log(object log) => UnityEngine.Debug.Log(log);
            public void LogWarning(object log) => UnityEngine.Debug.LogWarning(log);
            public void LogError(object log) => UnityEngine.Debug.LogError(log);
        }

        private readonly ConsoleLog Debug = new ConsoleLog();

        public XM ConvertToXM(IGAX_Song song) {
            XM xm = new XM();
            Debug?.Log($"Exporting song: {song.Info.ParsedName}");
            xm.ModuleName = song.Info.ParsedName;
            xm.Instruments = song.Info.UsedInstrumentIndices.Select((s, i) => GetInstrument(song, i)).ToArray();
            xm.NumInstruments = (ushort)xm.Instruments.Length;
            xm.NumChannels = song.Info.NumChannels;
            xm.NumPatterns = song.Info.NumPatternsPerChannel;
            xm.PatternOrderTable = Enumerable.Range(0, song.Info.NumPatternsPerChannel).Select(i => (byte)i).ToArray();
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
            xm.Patterns = Enumerable.Range(0, song.Info.NumPatternsPerChannel).Select(i => GetPattern(song, i)).ToArray();
            return xm;
        }
        public short[] ConvertSample(byte[] sample) {
            if (sample == null) return new short[0];
            short[] delta = new short[sample.Length];
            int cur = 0;
            for (int i = 0; i < sample.Length; i++) {
                short b = (short)(sample[i] - 128);
                delta[i] = (short)((b - cur) * 256);
                cur = b;
            }
            return delta;
        }
        public short[] ConvertSampleSigned(sbyte[] sample) {
            if (sample == null) return new short[0];
            short[] delta = new short[sample.Length];
            int cur = 0;
            for (int i = 0; i < sample.Length; i++) {
                short b = (short)(sample[i]);
                delta[i] = (short)((b - cur) * 256);
                cur = b;
            }
            return delta;
        }
        public XM_Instrument GetInstrument(IGAX_Song song, int index) {
            int ind = song.Info.UsedInstrumentIndices[index];
            var gax_instr = song.Info.InstrumentSet[ind].Value;

            // Create instrument
            XM_Instrument instr = new XM_Instrument {
                InstrumentName = "Instrument " + ind
            };

            if (gax_instr.VibratoDepth != 0) {
                instr.VibratoDepth = (byte)(gax_instr.VibratoDepth);
                instr.VibratoRate = (byte)(gax_instr.VibratoSpeed % 0x3F);
                instr.VibratoSweep = gax_instr.VibratoDelay; // Not the same, but similar
                instr.VibratoType = 0; // GAX vibrato is sine-only
            }
            GetInstrument_ConfigureEnvelope(gax_instr, instr);

            // Create samples
            var gax_instrRow = gax_instr.Rows[0];
            var gax_sampleIndex = gax_instrRow.SampleIndex > 0 ? gax_instrRow.SampleIndex - 1 : 0;
            var gax_smp = gax_instr.Samples[gax_sampleIndex];

            bool is16Bit = true;
            /*if (song.Info.Context.GetGAXSettings().MajorVersion < 3) {
                is16Bit = false;
            }*/
            bool isSigned = false;
            if (song.Info.Context.GetGAXSettings().MajorVersion < 3) {
                isSigned = true;
            }


            XM_Sample smp = new XM_Sample {
                SampleName = "Sample " + gax_instr.SampleIndices[gax_sampleIndex],
                SampleData16 = (isSigned
                ? ConvertSampleSigned(song.Info.Samples[gax_instr.SampleIndices[gax_sampleIndex]].SampleSigned)
                : ConvertSample(song.Info.Samples[gax_instr.SampleIndices[gax_sampleIndex]].SampleUnsigned)),
                Type = (byte)(is16Bit ? (1 << 4) : 0) // 16 bit sample data
            };
            smp.SampleLength = (uint)(smp.SampleData16.Length);

            // If loop
            bool loop = gax_smp.LoopStart != 0 || gax_smp.LoopEnd != 0;
            if (loop) {
                smp.Type = (byte)BitHelpers.SetBits(smp.Type, 1, 2, 0);
                if (gax_smp.IsBidirectional) smp.Type = (byte)BitHelpers.SetBits(smp.Type, 2, 2, 0); // Bidirectional
                smp.SampleLoopStart = gax_smp.LoopStart;
                smp.SampleLoopLength = (gax_smp.LoopEnd - gax_smp.LoopStart) - 1;
            }
            if (is16Bit) {
                smp.SampleLength *= 2;
                smp.SampleLoopStart *= 2;
                smp.SampleLoopLength *= 2;
            }

            int instrPitch = gax_smp.Pitch / 32;
            int relNote = gax_instrRow.RelativeNoteNumber;
            int relativeNoteNumber =
                instrPitch - 1 // GAX notes start at 1
                + (gax_instr.Rows[0].DontUseNotePitch ? 0 : (relNote - 2));

            Debug?.Log($"(Instrument {ind}) Sample:{gax_instr.SampleIndices[gax_sampleIndex]}" +
                $" - Pitch:{gax_smp.Pitch}" +
                $" - Relative Note Number:{relativeNoteNumber}" +
                $" - Cfg1:{gax_instrRow.DontUseNotePitch}" +
                $" - CfgRelNote:{relNote}" +
                $" - NumRows:{gax_instr.NumRows}" +
                $"");
            smp.RelativeNoteNumber = (sbyte)relativeNoteNumber;
            smp.FineTune = (sbyte)((gax_smp.Pitch - (instrPitch * 32)) * 4);
            if (ind == 0) {
                smp.RelativeNoteNumber = 0;
                smp.FineTune = 0;
            }

            instr.Samples = new XM_Sample[] { smp };

            return instr;
        }

        private void GetInstrument_ConfigureEnvelope(GAX_Instrument gax_instr, XM_Instrument instr) {
            var env = gax_instr.Envelope.Value;


            float volumeFactor = 1f;

            // Try to find a "set volume" effect in this instrument's rows
            var volEffect = gax_instr.Rows[0].Effects.FirstOrDefault(e => e.Type == GAX_InstrumentRow.Effect.EffectType.SetVolume);
            if (volEffect != null) {
                volumeFactor = volEffect.Parameter / 255f;
            }

            // Volume envelope
            instr.NumVolumePoints = env.NumPoints;
            for (int i = 0; i < instr.NumVolumePoints; i++) {
                var vol = (env.Points[i].Y / 255f) * volumeFactor;

                instr.PointsForVolumeEnvelope[i * 2 + 0] = env.Points[i].X;
                instr.PointsForVolumeEnvelope[i * 2 + 1] = (ushort)(vol * 64);
                // The values in gax's envelope are 0-255 - in XM's they are 0-64.
                // However for GAX they all seem to be conversions from 0-64, i.e. no values that aren't 0 when doing % 4.
                // Excpet for 0xFF which should be treated as 64
            }
            instr.VolumeType = 1;
            if (env.Sustain.HasValue) {
                instr.VolumeFadeout = 0;
            } else {
                instr.VolumeFadeout = 0xFFFF;
            }
            //instr.VolumeFadeout = 0x400;


            bool addedPoint = false;
            // In GAX
            /*if (instr.NumVolumePoints > 0 && env.Points[instr.NumVolumePoints - 1].Y != 0) {
                var i = instr.NumVolumePoints;
                instr.PointsForVolumeEnvelope[i * 2 + 0] = (ushort)(env.Points[i-1].X+1);
                instr.PointsForVolumeEnvelope[i * 2 + 1] = 0;
                addedPoint = true;
                instr.NumVolumePoints++;
            }*/

            if (env.Sustain.HasValue || addedPoint) {
                if (env.Sustain.HasValue) {
                    instr.VolumeSustainPoint = env.Sustain.Value;
                } else {
                    instr.VolumeSustainPoint = (byte)(instr.NumVolumePoints - 2);
                }
                instr.VolumeType = (byte)BitHelpers.SetBits(instr.VolumeType, 1, 1, 1);
            }
            if (env.LoopEnd.HasValue && env.LoopStart.HasValue) {
                instr.VolumeLoopStartPoint = env.LoopStart.Value;
                instr.VolumeLoopEndPoint = env.LoopEnd.Value;
                instr.VolumeType = (byte)BitHelpers.SetBits(instr.VolumeType, 1, 1, 2);
            }
        }


        private void GetInstrument_ConfigureEnvelope2(GAX_Instrument gax_instr, XM_Instrument instr) {
            var env = gax_instr.Envelope.Value;

            Dictionary<int, byte> pointMapping = new Dictionary<int, byte>();
            List<KeyValuePair<ushort, ushort>> points = new List<KeyValuePair<ushort, ushort>>();

            // Volume envelope
            for (int i = 0; i < env.NumPoints; i++) {
                var p0 = env.Points[i];
                pointMapping[i] = (byte)points.Count;
                points.Add(new KeyValuePair<ushort, ushort>(p0.X, (ushort)((p0.Y + 1) / 4)));

                if (i < env.NumPoints - 1 && p0.X < env.Points[i + 1].X - 1 && env.Points[i + 1].Interpolation == 0) {
                    var p1 = env.Points[i + 1];
                    var currentX = p1.X - 1;
                    var newX = p1.X;
                    ushort newY = p1.Y;
                    newY = (ushort)(((p1.Interpolation * (newX - currentX)) >> 8) + newY);
                    points.Add(new KeyValuePair<ushort, ushort>(newX, (ushort)((newY + 1) / 4)));
                }
            }
            instr.NumVolumePoints = (byte)points.Count;
            //Debug.Log(instr.NumVolumePoints);
            for (int i = 0; i < instr.NumVolumePoints; i++) {
                instr.PointsForVolumeEnvelope[i * 2 + 0] = points[i].Key;
                instr.PointsForVolumeEnvelope[i * 2 + 1] = points[i].Value;
            }

            instr.VolumeType = 1;
            //instr.VolumeFadeout = 0x400;

            if (env.Sustain.HasValue) {
                instr.VolumeSustainPoint = pointMapping[env.Sustain.Value];
                instr.VolumeType = (byte)BitHelpers.SetBits(instr.VolumeType, 1, 1, 1);
            }
            if (env.LoopEnd.HasValue && env.LoopStart.HasValue) {
                instr.VolumeLoopStartPoint = pointMapping[env.LoopStart.Value];
                instr.VolumeLoopEndPoint = pointMapping[env.LoopEnd.Value];
                instr.VolumeType = (byte)BitHelpers.SetBits(instr.VolumeType, 1, 1, 2);
            }
        }

        private XM_Pattern GetPattern(IGAX_Song song, int index) {
            XM_Pattern pat = new XM_Pattern {
                NumChannels = song.Info.NumChannels,
                NumRows = song.Info.NumRowsPerPattern
            };
            XM_PatternRow[] rows = Enumerable.Range(0, song.Info.NumChannels).SelectMany(c => GetPatternRows(song, index, c, song.Info.NumRowsPerPattern)).ToArray();
            pat.PatternRows = new XM_PatternRow[rows.Length];
            for (int row = 0; row < song.Info.NumRowsPerPattern; row++) {
                for (int ch = 0; ch < song.Info.NumChannels; ch++) {
                    pat.PatternRows[row * song.Info.NumChannels + ch] = rows[ch * song.Info.NumRowsPerPattern + row];
                }
            }
            // pat.PackedPatternDataSize = (ushort)(song.NumChannels * song.NumRowsPerPattern);
            pat.PackedPatternDataSize = (ushort)pat.PatternRows.Sum(p => p.Size);

            return pat;
        }

        private XM_PatternRow[] GetPatternRows(IGAX_Song song, int patternIndex, int channelIndex, int numRows) {
            GAX_Channel channel = song.GetChannel(channelIndex);
            GAX_Pattern gax = channel?.Patterns[patternIndex];
            GAX_PatternHeader header = channel?.PatternHeaders[patternIndex];
            XM_PatternRow[] rows = new XM_PatternRow[numRows];
            int curRow = 0;
            if (gax.IsEmptyTrack) {
                while (curRow < rows.Length) {
                    rows[curRow++] = new XM_PatternRow();
                }
                return rows;
            }
            foreach (var row in gax.Rows) {
                byte? eff = null;
                byte? effParam = null;
                byte? vol = null;
                byte? note = null;
                byte? instrument = null;
                bool noteOff = false;

                void ConfigureEffect() {
                    /*eff = (row.Effect < 16 && row.Effect != 12) ? (byte?)row.Effect : null;
                    effParam = eff.HasValue ? (byte?)(row.EffectParameter) : null;
                    vol = (row.Effect == 12) ? (byte?)(0x10 + (row.EffectParameter >> 2)) : null;*/
                    switch (row.Effect) {
                        case GAX_PatternRow.EffectType.None:
                            break;
                        case GAX_PatternRow.EffectType.PortamentoUp: // TODO: Also the first tick
                            eff = 1;
                            effParam = row.EffectParameter;
                            break;
                        case GAX_PatternRow.EffectType.PortamentoDown: // TODO: Also the first tick
                            eff = 2;
                            effParam = row.EffectParameter;
                            break;
                        case GAX_PatternRow.EffectType.TonePortamento: // TODO: Also the first tick
                            eff = 3;
                            effParam = row.EffectParameter;
                            break;
                        case GAX_PatternRow.EffectType.SetSpeedModulate:
                            var param1 = BitHelpers.ExtractBits(row.EffectParameter, 4, 4);
                            var param2 = BitHelpers.ExtractBits(row.EffectParameter, 4, 0);
                            var param = (param1 + param2) / 2;
                            eff = 15;
                            effParam = (byte)param;
                            Debug?.Log($"{row.Offset}: Effect {row.Effect} was used, incorrect speed!");
                            break;
                        case GAX_PatternRow.EffectType.VolumeSlideUp:
                        case GAX_PatternRow.EffectType.VolumeSlideDown:
                            if (row.EffectParameter >= 0x10) {
                                Debug?.LogWarning($"{row.Offset}: Effect {row.Effect} was used with param {row.EffectParameter} >= 0x10!");
                            } else {
                                eff = 10;
                                if (row.Effect == GAX_PatternRow.EffectType.VolumeSlideUp) {
                                    effParam = (byte)BitHelpers.SetBits(0, row.EffectParameter, 4, 4);
                                    vol = (byte)BitHelpers.SetBits(0x90, row.EffectParameter, 4, 0); // Fine volume up, in volume column
                                } else {
                                    effParam = (byte)BitHelpers.SetBits(0, row.EffectParameter, 4, 0);
                                    vol = (byte)BitHelpers.SetBits(0x80, row.EffectParameter, 4, 0); // Fine volume down, in volume column
                                }
                            }
                            break;
                        case GAX_PatternRow.EffectType.SetVolume: // Set volume
                            eff = 12;
                            effParam = (byte)(row.EffectParameter >> 2); // Volume ranges from 0-255 in GAX, and 0-64 in XM
                            break;
                        case GAX_PatternRow.EffectType.PatternBreak:
                            eff = 13;
                            effParam = row.EffectParameter;
                            break;
                        case GAX_PatternRow.EffectType.NoteDelay:
                            var topBits = BitHelpers.ExtractBits(row.EffectParameter, 4, 4);
                            if (topBits == 0xD) {
                                eff = 14;
                                effParam = row.EffectParameter;
                            } else {
                                Debug?.LogWarning($"{row.Offset}: Effect 0xE{topBits:X1}x was used!");
                            }
                            break;
                        case GAX_PatternRow.EffectType.SetSpeed:
                            if (row.EffectParameter < 0x20) {
                                eff = 15;
                                effParam = row.EffectParameter;
                            } else {
                                eff = 15;
                                effParam = 0x1F;
                                Debug?.LogWarning($"{row.Offset}: Effect {row.Effect} was used with param {row.EffectParameter} >= 0x20!");
                            }
                            break;
                        default:
                            Debug?.LogWarning($"{row.Offset}: Unknown effect {row.Effect} was used!");
                            break;

                    }
                }
                void ConfigureNote() {

                    if (row.Note != 1) {
                        instrument = (byte)(1 + Array.IndexOf(song.Info.UsedInstrumentIndices, row.Instrument));
                        var gax_instr = song.Info.InstrumentSet[row.Instrument].Value;
                        if (gax_instr.Rows[0].DontUseNotePitch) {
                            if (row.Note != 0) note = gax_instr.Rows[0].RelativeNoteNumber;
                        } else {
                            if (row.Note != 0) {
                                note = (byte)(row.Note + header.Transpose);
                            }
                        }
                    } else {
                        // Note off
                        instrument = 0;
                        note = 97;
                        noteOff = true;
                    }
                }

                switch (row.Command) {
                    // TODO: correct these
                    case GAX_PatternRow.Cmd.Unknown:
                        break;
                    case GAX_PatternRow.Cmd.EffectOnly:
                        ConfigureEffect();
                        rows[curRow++] = new XM_PatternRow(volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        break;
                    case GAX_PatternRow.Cmd.Rest:
                        rows[curRow++] = new XM_PatternRow();
                        break;
                    case GAX_PatternRow.Cmd.RestMulti:
                        for (int i = 0; i < row.RestDuration; i++) {
                            rows[curRow++] = new XM_PatternRow();
                        }
                        break;
                    case GAX_PatternRow.Cmd.Note:
                        ConfigureEffect();
                        ConfigureNote();
                        if (noteOff) { // Note off
                            rows[curRow++] = new XM_PatternRow(note: 97, volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        } else {
                            rows[curRow++] = new XM_PatternRow(note: note, instrument: instrument, volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        }
                        break;
                    case GAX_PatternRow.Cmd.NoteOnly:
                        ConfigureNote();
                        if (noteOff) { // Note off
                            rows[curRow++] = new XM_PatternRow(note: 97);
                            //rows[curRow++] = new XM_PatternRow(effectType: 0xe, effectParameter: 0xc0); // Note Cut
                        } else {
                            rows[curRow++] = new XM_PatternRow(note: note, instrument: instrument);
                        }
                        break;
                }
            }
            return rows;
        }
    }
}
