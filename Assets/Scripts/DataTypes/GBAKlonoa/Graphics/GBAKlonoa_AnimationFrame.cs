using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBAKlonoa_AnimationFrame : BinarySerializable
    {
        public ushort Pre_ImgDataLength { get; set; }
        public bool Pre_IsReferencedInLevel { get; set; }
        public bool Pre_IsMapAnimation { get; set; }

        public int ImgDataPointerValue { get; set; }
        public bool IsRLECompressed { get; set; }
        public Pointer ImgDataPointer { get; set; }
        public byte? LinkedAnimIndex { get; set; }
        public byte Speed { get; set; }
        public byte Byte_05 { get; set; }
        public byte Byte_06 { get; set; }
        public byte Byte_07 { get; set; }

        // Serialized from pointers
        public byte[] ImgData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            ImgDataPointerValue = s.Serialize<int>(ImgDataPointerValue, name: nameof(ImgDataPointerValue));

            // Last frame in the animation, nothing more to read
            if (ImgDataPointerValue == -1)
                return;

            // Some special case. Both set the animation timer to -1, thus stopping further frames from playing, and -2 sets some object properties.
            if (ImgDataPointerValue == -2 || ImgDataPointerValue == -3)
                return;

            // Some animations end with an index to a linked animation
            if (ImgDataPointerValue < 10000)
            {
                LinkedAnimIndex = (byte)ImgDataPointerValue;
                s.Log($"{nameof(LinkedAnimIndex)}: {LinkedAnimIndex}");

                if (Pre_IsMapAnimation)
                    Debug.LogWarning($"{s.CurrentPointer}: Map animation frame links to another animation");

                return;
            }

            // DCT has a lot of flags we need to check for, unlike EOD where the pointer is always a direct pointer
            if (s.GetR1Settings().EngineVersion == EngineVersion.KlonoaGBA_DCT)
            {
                // Map animations can have relative offsets to compressed world data
                if (Pre_IsMapAnimation)
                {
                    if ((ImgDataPointerValue & 0x10000000) != 0)
                        ImgDataPointer = new Pointer((ushort)ImgDataPointerValue, Offset.Context.GetFile(GBAKlonoa_BaseManager.CompressedWorldObjTileBlockName));
                    else
                        ImgDataPointer = new Pointer(ImgDataPointerValue, Offset.File);
                }
                else
                {
                    // Frames can be directly compressed with RLE
                    if ((ImgDataPointerValue & 0x20000000) != 0)
                    {
                        ImgDataPointer = new Pointer(ImgDataPointerValue & 0xdfffffff, Offset.File);
                        IsRLECompressed = true;
                    }
                    // Like EOD maps and bosses have compressed data, but unlike that game the pointer is now a relative offset
                    else if ((ImgDataPointerValue & 0x40000000) != 0)
                    {
                        // We only want to parse this if the animation is referenced in the currently loaded level or else the compressed
                        // data might not match this animation, thus showing the wrong frames

                        if (Pre_IsReferencedInLevel)
                            ImgDataPointer = new Pointer(ImgDataPointerValue - 0x40000000, Offset.Context.GetFile(GBAKlonoa_BaseManager.CompressedObjTileBlockName));
                    }
                    else
                    {
                        ImgDataPointer = new Pointer(ImgDataPointerValue, Offset.File);
                    }
                }
            }
            else
            {
                ImgDataPointer = new Pointer(ImgDataPointerValue, Offset.File);
            }

            if (ImgDataPointer == null)
                Debug.LogWarning($"Animation frame data is null at {Offset}");

            s.Log($"{nameof(ImgDataPointer)}: {ImgDataPointer}");

            Speed = s.Serialize<byte>(Speed, name: nameof(Speed));
            Byte_05 = s.Serialize<byte>(Byte_05, name: nameof(Byte_05));
            Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
            Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));

            s.DoAt(ImgDataPointer, () => s.DoEncodedIf(new GBA_RLEEncoder(), IsRLECompressed, () => ImgData = s.SerializeArray<byte>(ImgData, Pre_ImgDataLength, name: nameof(ImgData))));
        }
    }
}