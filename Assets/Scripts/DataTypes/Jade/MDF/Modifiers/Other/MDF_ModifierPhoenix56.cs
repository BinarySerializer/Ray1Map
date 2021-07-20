using BinarySerializer;

namespace R1Engine.Jade {
	public class MDF_ModifierPhoenix56 : MDF_Modifier {
		public uint Version { get; set; }
		public uint Type { get; set; }
		public uint UInt0 { get; set; }
		public ushort UShort1 { get; set; }
		public float Float2 { get; set; }
		public uint UInt2 { get; set; }
		public float Float3 { get; set; }
		public byte Byte4 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) Version = s.Serialize<uint>(Version, name: nameof(Version));

			Type = s.Serialize<uint>(Type, name: nameof(Type));
			if (Type >= 2) {
				if(Type >= 3) UInt0 = s.Serialize<uint>(UInt0, name: nameof(UInt0));
				UShort1 = s.Serialize<ushort>(UShort1, name: nameof(UShort1));
				if (Type == 4)
					Float2 = s.Serialize<float>(Float2, name: nameof(Float2));
				else
					UInt2 = s.Serialize<uint>(UInt2, name: nameof(UInt2));
				Float3 = s.Serialize<float>(Float3, name: nameof(Float3));
				Byte4 = s.Serialize<byte>(Byte4, name: nameof(Byte4));
			}
		}
	}
}
