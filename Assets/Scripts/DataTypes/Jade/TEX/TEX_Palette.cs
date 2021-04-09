using System;
using BinarySerializer;

namespace R1Engine.Jade 
{
	public class TEX_Palette : Jade_File 
    {
        public BaseColor[] Colors { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
            switch (FileSize)
            {
                case 64:
                case 1024:
                    Colors = s.SerializeObjectArray<BGRA8888Color>((BGRA8888Color[]) Colors, FileSize / 4, name: nameof(Colors));
                    break;

                case 48:
                case 768:
                    Colors = s.SerializeObjectArray<BGR888Color>((BGR888Color[]) Colors, FileSize / 3, name: nameof(Colors));
                    break;

                default:
                    throw new NotImplementedException($"Invalid palette size");
            }
        }
    }
}