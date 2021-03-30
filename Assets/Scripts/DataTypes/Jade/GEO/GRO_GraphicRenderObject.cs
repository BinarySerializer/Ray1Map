using System;

namespace R1Engine.Jade {
    public class GRO_GraphicRenderObject : R1Serializable {
        public GRO_Type Type { get; set; }
        public uint Count_Editor { get; set; }
        public byte[] Bytes_Editor { get; set; }

        public GRO_Struct GRO { get; set; }

		public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            Type = s.Serialize<GRO_Type>(Type, name: nameof(Type));
            if (!Loader.IsBinaryData) {
                Count_Editor = s.Serialize<uint>(Count_Editor, name: nameof(Count_Editor));
                Bytes_Editor = s.SerializeArray<byte>(Bytes_Editor, Count_Editor, name: nameof(Bytes_Editor));
            }

            GRO = Type switch
            {
                GRO_Type.None => null,
                _ => throw new NotImplementedException($"TODO: Implement GRO Struct Type {Type}")
            };
        }
	}
}
