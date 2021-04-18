using System;
using BinarySerializer;
using UnityEngine;

namespace R1Engine.Jade
{
    public class TEX_Content_Animated : BinarySerializable {
        public TEX_File Texture { get; set; }

        public Frame[] References { get; set; }

        public uint UInt_00 { get; set; }
        public short Short_04 { get; set; }
        public byte FramesCount { get; set; }
        public byte Byte_07_Editor { get; set; }

        public Frame[] Frames { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            uint FileSize = (uint)(Texture.FileSize - (s.CurrentPointer - Texture.Offset));
            if (FileSize == 0) return;
            
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
            UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
            Short_04 = s.Serialize<short>(Short_04, name: nameof(Short_04));
            FramesCount = s.Serialize<byte>(FramesCount, name: nameof(FramesCount));
            if (!Loader.IsBinaryData) Byte_07_Editor = s.Serialize<byte>(Byte_07_Editor, name: nameof(Byte_07_Editor));

            Frames = s.SerializeObjectArray<Frame>(Frames, FramesCount, name: nameof(Frames));
        }

        public class Frame : BinarySerializable {
            public Jade_TextureReference Texture { get; set; }
            public short Duration { get; set; }
            public short Short_Editor { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                Texture = s.SerializeObject<Jade_TextureReference>(Texture, name: nameof(Texture))?.Resolve();
                Duration = s.Serialize<short>(Duration, name: nameof(Duration));
                if (!Loader.IsBinaryData) Short_Editor = s.Serialize<short>(Short_Editor, name: nameof(Short_Editor));
            }
		}
	}
}