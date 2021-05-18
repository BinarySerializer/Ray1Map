using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_RawPal : BinarySerializable {
        public TEX_File Texture { get; set; }

        public Slot[] Slots { get; set; }

        public uint UInt_00 { get; set; }
        public int Int_04 { get; set; }
        public int Int_08 { get; set; }
        public int Int_0C { get; set; }
        public int Int_10 { get; set; }
        public int Int_14 { get; set; }
        public int Int_18 { get; set; }
        public int Int_1C { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = Texture.FileSize;
            if(s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && FileSize > 0x50) FileSize = 0x50;
            if (!(FileSize > 0x50 || FileSize % 4 != 0)) {
                var byteCount = (FileSize - (s.CurrentPointer - Texture.Offset));
                var count = byteCount / 12 + ((byteCount % 12 >= 4) ? 1 : 0);
                var startPtr = s.CurrentPointer;
                Slots = s.SerializeObjectArray<Slot>(Slots, count, onPreSerialize:
                    slot => {
                        slot.ReferenceArrayStart = startPtr;
                        slot.ReferenceArrayByteCount = byteCount;
                    }, name: nameof(Slots));
                PreferredSlot?.Resolve();
            } else {
                throw new BinarySerializableException(this, $"Invalid {nameof(TEX_Content_RawPal)}");
            }
            if (s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) && FileSize < Texture.FileSize) {
                UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
                if (UInt_00 != 0) {
                    Int_04 = s.Serialize<int>(Int_04, name: nameof(Int_04));
                    Int_08 = s.Serialize<int>(Int_08, name: nameof(Int_08));
                    Int_0C = s.Serialize<int>(Int_0C, name: nameof(Int_0C));
                    Int_10 = s.Serialize<int>(Int_10, name: nameof(Int_10));
                    Int_14 = s.Serialize<int>(Int_14, name: nameof(Int_14));
                    Int_18 = s.Serialize<int>(Int_18, name: nameof(Int_18));
                    Int_1C = s.Serialize<int>(Int_1C, name: nameof(Int_1C));
                }
            }
        }

        public Slot PreferredSlot {
            get {
                var palOrder = GetSlotOrder(Context.GetR1Settings());
                for (int i = 0; i < palOrder.Length; i++) {
                    var pi = palOrder[i];
                    if (pi < 0) break;
                    if (pi >= Slots.Length) continue;
                    var reference = Slots[pi];
                    if (!reference.Raw4or8.IsNull || !reference.Palette.IsNull || !reference.Raw24or32.IsNull) {
                        return reference;
                    }
                }
                return null;
            }
        }

        public static sbyte[] GetSlotOrder(GameSettings settings) {
            switch (settings.GameModeSelection) {
                case GameModeSelection.RaymanRavingRabbidsWii:
                case GameModeSelection.RaymanRavingRabbidsWiiJP:
                case GameModeSelection.RaymanRavingRabbids2Wii:
                case GameModeSelection.RaymanRavingRabbids2PC:
                case GameModeSelection.KingKongGC:
                case GameModeSelection.BeyondGoodAndEvilGC:
                    return SlotOrderWii;
                case GameModeSelection.RaymanRavingRabbidsXbox360:
                    return SlotOrderXbox360;
                default:
                    return SlotOrderPC;
            }
        }
        public static sbyte[] SlotOrderWii = new sbyte[] { 1, 0, -1, -1 };
        public static sbyte[] SlotOrderPC = new sbyte[] { 0, -1, -1, -1 };
        public static sbyte[] SlotOrderXbox360 = new sbyte[] { 0, 2, -1, -1 };

        public class Slot : BinarySerializable {
            public Pointer ReferenceArrayStart { get; set; } // Set in onPreSerialize
            public long ReferenceArrayByteCount { get; set; }

            public Jade_TextureReference Raw4or8 { get; set; }
            public Jade_PaletteReference Palette { get; set; }
            public Jade_TextureReference Raw24or32 { get; set; }
            public override void SerializeImpl(SerializerObject s) {
				Raw4or8 = s.SerializeObject<Jade_TextureReference>(Raw4or8, name: nameof(Raw4or8));
                if (s.CurrentAbsoluteOffset >= ReferenceArrayStart.AbsoluteOffset + ReferenceArrayByteCount) {
                    Palette = new Jade_PaletteReference(Context, new Jade_Key(Context, 0xFFFFFFFF));
                } else {
                    Palette = s.SerializeObject<Jade_PaletteReference>(Palette, name: nameof(Palette));
                }
                if (s.CurrentAbsoluteOffset >= ReferenceArrayStart.AbsoluteOffset + ReferenceArrayByteCount) {
                    Raw24or32 = new Jade_TextureReference(Context, new Jade_Key(Context, 0xFFFFFFFF));
                } else {
                    Raw24or32 = s.SerializeObject<Jade_TextureReference>(Raw24or32, name: nameof(Raw24or32));
                }
            }

            public bool HasTexture => Raw4or8?.Info != null || Palette?.Value != null || Raw24or32?.Info != null;

            public Jade_TextureReference TextureRef {
                get {
                    if (Raw4or8.IsNull && Palette.IsNull) {
                        return Raw24or32;
                    } else {
                        return Raw4or8;
                    }
                }
            }

            public void Resolve() {
                if (Raw4or8.IsNull && Palette.IsNull) {
                    Raw24or32?.Resolve();
                } else {//if (Unknown.IsNull) {
                    Raw4or8?.Resolve();
                    Palette?.Resolve();
                } /*else {
                    //throw new NotImplementedException("TODO: Implement RawPal textures where 
                }*/
            }
            public Texture2D ToTexture2D(TEX_File contentFile)
            {
                var texture = TEX_File.IsRawPalUnsupported(Context) ? contentFile : Raw4or8.Content;
                var pal = Palette.Value;
                if (Raw4or8.IsNull && Palette.IsNull) {
                    throw new BinarySerializableException(this, $"Implement RawPal format Unknown for key {texture.Key}");
                }

                if (texture == null || pal == null)
                    return null;

                if (texture.Type != TEX_File.TexFileType.Raw) return null;

                if (texture.Format != TEX_File.TexColorFormat.BPP_4 && texture.Format != TEX_File.TexColorFormat.BPP_8)
                    throw new BinarySerializableException(this, $"Unsupported raw texture format {texture.Format}");

                var tex = TextureHelpers.CreateTexture2D(texture.Width, texture.Height);

                for (int y = 0; y < texture.Height; y++)
                {
                    for (int x = 0; x < texture.Width; x++)
                    {
                        if (texture.Format == TEX_File.TexColorFormat.BPP_8)
                            tex.SetPixel(x, y, pal.Colors[texture.Content[(y * texture.Width + x)] % pal.Colors.Length].GetColor());
                        else
                            tex.SetPixel(x, y, pal.Colors[BitHelpers.ExtractBits(texture.Content[(y * texture.Width + x) / 2], 4, x % 2 == 1 ? 0 : 4) % pal.Colors.Length].GetColor());
                    }
                }

                tex.Apply();
                return tex;
            }
		}
	}
}