using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.GEN {
	public class RaymanGardenHelpers {
        public class Loc : BinarySerializable {
            public string Name { get; set; }
            public ushort HuffTableOffset { get; set; }
            public ushort HuffTableLength { get; set; }
            public ushort NumTables { get; set; }
            public HuffTableEntry[] HuffTable { get; set; }
            public Table[] Tables { get; set; }
            public Entry[][] Entries { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                s.Goto(Offset + 0x1C);
                Name = s.SerializeString(Name, name: nameof(Name));
                s.Goto(Offset + 0x2C);
                HuffTableOffset = s.Serialize<ushort>(HuffTableOffset, name: nameof(HuffTableOffset));
                HuffTableLength = s.Serialize<ushort>(HuffTableLength, name: nameof(HuffTableLength));
                s.DoAt(Offset + HuffTableOffset, () => {
                    HuffTable = s.SerializeObjectArray<HuffTableEntry>(HuffTable, HuffTableLength / 6, name: nameof(HuffTable));
                });
                NumTables = s.Serialize<ushort>(NumTables, name: nameof(NumTables));
                Tables = s.SerializeObjectArray<Table>(Tables, NumTables, onPreSerialize: t => t.HuffTable = HuffTable, name: nameof(Tables));
            }
            public class HuffTableEntry : BinarySerializable {
                public ushort Value { get; set; }
                public ushort Left { get; set; }
                public ushort Right { get; set; }
                public bool IsLeaf => Value != 0xFE && Left == 0 && Right == 0;

                public override void SerializeImpl(SerializerObject s) {
                    Value = s.Serialize<ushort>(Value, name: nameof(Value));
                    Left = s.Serialize<ushort>(Left, name: nameof(Left));
                    Right = s.Serialize<ushort>(Right, name: nameof(Right));
                }
            }
            public class Entry : BinarySerializable {
                public byte ID { get; set; }
                public byte Length { get; set; }
                public ushort Pos { get; set; }
                public override void SerializeImpl(SerializerObject s) {
                    ID = s.Serialize<byte>(ID, name: nameof(ID));
                    Length = s.Serialize<byte>(Length, name: nameof(Length));
                    Pos = s.Serialize<ushort>(Pos, name: nameof(Pos));
                }
            }
            public class Table : BinarySerializable {
                public HuffTableEntry[] HuffTable { get; set; }
                public byte ID { get; set; }
                public ushort NumEntries { get; set; }
                public ushort Pos { get; set; }
                public ushort Padding { get; set; }
                public Entry[] Entries { get; set; }
                public string[] Strings { get; set; }

                public class Eclipse_StringEncoder : IStreamEncoder 
                {
                    public Eclipse_StringEncoder(HuffTableEntry[] helpers, Entry entry)
                    {
                        Helpers = helpers;
                        Entry = entry;
                    }

                    public string Name => "Eclipse_StringEncoding";

                    public HuffTableEntry[] Helpers { get; set; }
                    public Entry Entry { get; set; }

                    public void DecompressString(byte[] compressed, Writer writer) {
                        byte bits = 0;
                        int curBit = -1;
                        int curHelper = 0;
                        int inPos = 0;
                        while (true) {
                            if (curBit < 0) {
                                if (inPos >= compressed.Length) break;
                                bits = compressed[inPos++];
                                curBit += 8;
                            }
                            if (BitHelpers.ExtractBits(bits, 1, curBit) == 0) {
                                curHelper = Helpers[curHelper].Left;
                                if (Helpers[curHelper].IsLeaf) {
                                    writer.Write(Helpers[curHelper].Value);
                                    curHelper = 0;
                                }
                            } else {
                                curHelper = Helpers[curHelper].Right;
                                if (Helpers[curHelper].IsLeaf) {
                                    writer.Write(Helpers[curHelper].Value);
                                    curHelper = 0;
                                }
                            }
                            curBit--;
                        }
                    }
                    public void DecodeStream(Stream input, Stream output) 
                    {
                        using Reader reader = new Reader(input, isLittleEndian: true, leaveOpen: true);
                        byte length = Entry.Length;
                        byte[] compressed = reader.ReadBytes(length);
                        byte[] decompressed = new byte[0];
                        
                        if (length > 0)
                        {
                            using Writer writer = new Writer(new MemoryStream());
                            DecompressString(compressed, writer);
                            writer.BaseStream.Position = 0;
                            decompressed = (writer.BaseStream.InnerStream as MemoryStream).ToArray();
                        }

                        output.Write(decompressed, 0, decompressed.Length);
                    }

                    public void EncodeStream(Stream input, Stream output) {
                        throw new NotImplementedException();
                    }
                }

                public override void SerializeImpl(SerializerObject s) {
                    ID = s.Serialize<byte>(ID, name: nameof(ID));
                    NumEntries = s.Serialize<ushort>(NumEntries, name: nameof(NumEntries));
                    Pos = s.Serialize<ushort>(Pos, name: nameof(Pos));
                    Padding = s.Serialize<ushort>(Padding, name: nameof(Padding));
                    s.DoAt(new Pointer(Pos, Offset.File), () => {
                        Entries = s.SerializeObjectArray<Entry>(Entries, NumEntries, name: nameof(Entries));
                    });
                    Strings = new string[NumEntries];
                    for (int i = 0; i < Entries.Length; i++) {
                        s.DoAt(new Pointer(Entries[i].Pos, Offset.File), () => {
                            s.DoEncoded(new Eclipse_StringEncoder(HuffTable, Entries[i]), () => {
                                var bytes = s.SerializeArray<byte>(default, s.CurrentLength, "bytes[" + i + "]");
                                string str = Encoding.Unicode.GetString(bytes);
                                Strings[i] = str;
                                s.Log(str);
                            });
                        });
                    }
                }
            }
        }
        public static async UniTask ExportGardenAsync(GameSettings settings, string input, string output) {
            //var pal = PaletteHelpers.CreateDummyPalette(256, false);
            var pal = File.ReadAllBytes(input + "/palette.dat");
            Color[] cols = new Color[pal.Length / 3];
            for (int i = 0; i < cols.Length; i++) {
                cols[i] = new Color(pal[i * 3] / 255f, pal[i * 3 + 1] / 255f, pal[i * 3 + 2] / 255f, 1f);
            }
            using (var context = new Ray1MapContext(input, settings)) {
                List<Loc> locs = new List<Loc>();
                foreach (var file in Directory.EnumerateFiles(input + "/loc")) {
                    FileInfo fi = new FileInfo(file);
                    string name = "loc/" + fi.Name;
                    await context.AddLinearFileAsync(name);
                    locs.Add(FileFactory.Read<Loc>(context, name));
                }
                var dict = locs.ToDictionary(l => l.Name, l => l.Tables[6].Strings);
                TextEditor te = new TextEditor {
                    text = Newtonsoft.Json.JsonConvert.SerializeObject(dict, Newtonsoft.Json.Formatting.Indented)
                };
                te.SelectAll();
                te.Copy();
            }
            byte[] curTileSet = null;
            foreach (var file in Directory.EnumerateFiles(input)) {
                FileInfo fi = new FileInfo(file);
                string name = fi.Name;
                if (!name.EndsWith(".bin")) continue;
                name = name.Replace(".bin", "");
                byte[] bs = File.ReadAllBytes(file);
                byte type = bs[0];
                byte w, h;
                Texture2D tex;
                switch (type) {
                    case 0:
                        curTileSet = bs;
                        //Util.ByteArrayToFile(output + "/" + name + ".rgb", bs.Skip(6).SelectMany(b => new byte[] { pal[b * 3], pal[b * 3 + 1], pal[b * 3 + 2] }).ToArray());
                        break;
                    case 1:
                        w = bs[4];
                        h = bs[5];
                        Debug.Log(name + " - " + w + "x" + h);
                        tex = TextureHelpers.CreateTexture2D(w * 8, h * 8);
                        for (int y = 0; y < h; y++) {
                            for (int x = 0; x < w; x++) {
                                int val = bs[6 + (y * w + x) * 2] | (bs[6 + (y * w + x) * 2 + 1] << 8);
                                int tileInd = BitHelpers.ExtractBits(val, 14, 0);
                                bool flipX = BitHelpers.ExtractBits(val, 1, 15) == 1;
                                tex.FillInTile(curTileSet, tileInd * 8 * 8 + 6, cols, Util.TileEncoding.Linear_8bpp, 8, true, x * 8, y * 8, flipX, false);
                                //tex.SetPixel(x, h - 1 - y, colsGrey[bs[6 + (y * w + x) * 2]]);
                            }
                        }
                        tex.Apply();
                        Util.ByteArrayToFile(output + "/" + name + ".png", tex.EncodeToPNG());
                        break;
                    case 2:
                    default:
                        w = bs[2];
                        h = bs[3];
                        Debug.Log(name + " - " + w + "x" + h);
                        tex = new Texture2D(w, h);
                        for (int y = 0; y < h; y++) {
                            for (int x = 0; x < w; x++) {
                                int b = bs[4 + y * w + x];
                                if (b == 0xE3) {
                                    tex.SetPixel(x, h - 1 - y, Color.clear);
                                } else {
                                    tex.SetPixel(x, h - 1 - y, cols[bs[4 + y * w + x]]);
                                }
                            }
                        }
                        tex.Apply();

                        Util.ByteArrayToFile(output + "/" + name + ".png", tex.EncodeToPNG());
                        break;
                }
            }
        }
    }
}
