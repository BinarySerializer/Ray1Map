using System;
using BinarySerializer;

namespace R1Engine.Jade {
    public class GRO_Struct : BinarySerializable {
        public GRO_Type Type { get; set; }
        public uint Count_Editor { get; set; }
        public byte[] Bytes_Editor { get; set; }

        public GRO_GraphicRenderObject Value { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Type = s.Serialize<GRO_Type>(Type, name: nameof(Type));
            if (!Loader.IsBinaryData) {
                Count_Editor = s.Serialize<uint>(Count_Editor, name: nameof(Count_Editor));
                Bytes_Editor = s.SerializeArray<byte>(Bytes_Editor, Count_Editor, name: nameof(Bytes_Editor));
            }

            Value = Type switch
            {
                GRO_Type.None => null,
                GRO_Type.GEO => Value = s.SerializeObject<GEO_RenderObject>((GEO_RenderObject)Value, name: nameof(Value)),
                _ => throw new NotImplementedException($"TODO: Implement GRO Struct Type {Type}")
            };
        }
	}
}
