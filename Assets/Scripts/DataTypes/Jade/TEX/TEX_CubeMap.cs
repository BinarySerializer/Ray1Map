using System;
using BinarySerializer;

namespace R1Engine.Jade 
{
	public class TEX_CubeMap : Jade_File 
    {
        public byte[] Data { get; set; }
        public TEX_File Header { get; set; }

		public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, FileSize - 0x20, name: nameof(Data));
            Header = s.SerializeObject<TEX_File>(Header, onPreSerialize: h => {
                h.FileSize = 0x20;
                h.Loader = Loader;
                h.Key = Key;
            }, name: nameof(Header));
        }
    }
}