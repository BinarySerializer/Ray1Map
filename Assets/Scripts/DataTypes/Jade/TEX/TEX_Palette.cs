using System;
using BinarySerializer;

namespace R1Engine.Jade 
{
	public class TEX_Palette : Jade_File {
        public Jade_Key Montreal_Key { get; set; }
        public BaseColor[] Colors { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && Loader.IsBinaryData)
                Montreal_Key = s.SerializeObject<Jade_Key>(Montreal_Key, name: nameof(Montreal_Key));
            uint size = Montreal_Key != null ? FileSize - 4 : FileSize;
            switch (size)
            {
                case 0x40:
                case 0x400:
                case 0x880:
                case 0x100:
                    if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier) && (size == 0x880 || size == 0x100)) {
                        throw new NotImplementedException($"Invalid palette size");
                    }
                    Colors = s.SerializeObjectArray<BGRA8888Color>((BGRA8888Color[]) Colors, size / 4, name: nameof(Colors));
                    break;

                case 0x30:
                case 0x300:
                    Colors = s.SerializeObjectArray<BGR888Color>((BGR888Color[]) Colors, size / 3, name: nameof(Colors));
                    break;

                default:
                    throw new NotImplementedException($"Invalid palette size");
            }
        }
    }
}