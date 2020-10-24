using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine {
	public class GAX2_XMWriter {
        public XM ConvertToXM(GAX2_Song song) {
            XM xm = new XM();
            xm.ModuleName = song.ParsedName;
            xm.Instruments = song.InstrumentIndices.Select((s,i) => GetInstrument(song,i)).ToArray();
            xm.NumInstruments = (ushort)xm.Instruments.Length;
            xm.NumChannels = song.NumChannels;
            xm.NumPatterns = song.NumPatternsPerChannel;
            xm.PatternOrderTable = Enumerable.Range(0,song.NumPatternsPerChannel).Select(i => (byte)i).ToArray();
            xm.SongLength = (ushort)xm.PatternOrderTable.Length;
            xm.DefaultTempo = 6;
            xm.DefaultBPM = 150;
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
            smp.SampleData16 = ConvertSample(song.Samples[index].Sample); //song.Samples[index].Sample.Select(b => (sbyte)(b - 128)).ToArray();
            smp.Type = 1 << 4; // 16 bit sample data
            smp.SampleLength = (uint)smp.SampleData16.Length * 2;
            smp.FineTune = gax_instr.Pitch1;
            smp.RelativeNoteNumber = (sbyte)(gax_instr.Pitch2 * 12);

            XM_Instrument instr = new XM_Instrument();
            instr.InstrumentName = "Instrument " + ind;
            instr.Samples = new XM_Sample[] { smp };
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
            foreach (var row in gax.Rows) {
                switch (row.Command) {
                    case GAX2_PatternRow.Cmd.StartTrack:
                        break;
                    case GAX2_PatternRow.Cmd.EmptyTrack:
                        while (curRow < rows.Length) {
                            rows[curRow++] = new XM_PatternRow();
                        }
                        break;
                    // TODO: correct these
                    case GAX2_PatternRow.Cmd.Unknown:
                        break;
                    case GAX2_PatternRow.Cmd.EffectOnly:
                        eff = (row.Effect < 16 && row.Effect != 14 && row.Effect != 12) ? (byte?)row.Effect : null;
                        effParam = eff.HasValue ? (byte?)(row.Velocity) : null;
                        vol = (row.Effect == 12) ? (byte?)(0x10 + (row.Velocity >> 2)) : null;
                        rows[curRow++] = new XM_PatternRow(volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        break;
                    case GAX2_PatternRow.Cmd.Unknown81:
                        rows[curRow++] = new XM_PatternRow();
                        break;
                    case GAX2_PatternRow.Cmd.RestSingle:
                        rows[curRow++] = new XM_PatternRow();
                        break;
                    case GAX2_PatternRow.Cmd.RestMultiple:
                        for (int i = 0; i < row.RestDuration; i++) {
                            rows[curRow++] = new XM_PatternRow();
                        }
                        break;
                    case GAX2_PatternRow.Cmd.Note:
                        eff = (row.Effect < 16 && row.Effect != 14 && row.Effect != 12) ? (byte?)row.Effect : null;
                        effParam = eff.HasValue ? (byte?)(row.Velocity) : null;
                        vol = (row.Effect == 12) ? (byte?)(0x10 + (row.Velocity >> 2)) : null;
                        if (row.Instrument == 250) {
                            rows[curRow++] = new XM_PatternRow(note: 97, volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        } else {
                            int instr = 1 + Array.IndexOf(song.InstrumentIndices, row.Instrument);
                            rows[curRow++] = new XM_PatternRow(note: row.Note, instrument: (byte)instr, volumeColumnByte: vol, effectType: eff, effectParameter: effParam);
                        }
                        break;
                }
            }
            return rows;
        }
    }
}
