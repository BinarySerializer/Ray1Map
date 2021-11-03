using System;
using BinarySerializer;
using BinarySerializer.GBA;
using UnityEngine;

namespace Ray1Map.GBAVV
{
    public class GBAVV_Crash1_CutsceneFrameGraphics : BinarySerializable
    {
        public RGBA5551Color[] Palette { get; set; }
        public byte[] ImageData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 256, name: nameof(Palette));

            // The JP version has data here for one frame which is not compressed, but appears to just be leftover data (?)
            try
            {
                s.DoEncoded(new GBA_LZSSEncoder(), () => ImageData = s.SerializeArray<byte>(ImageData, s.CurrentLength, name: nameof(ImageData)));
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
    }
}