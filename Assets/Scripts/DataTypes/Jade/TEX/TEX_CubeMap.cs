using System;
using BinarySerializer;

namespace R1Engine.Jade 
{
	public class TEX_CubeMap : Jade_File 
    {
        public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, FileSize, name: nameof(Data));
        }
    }
}