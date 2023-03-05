using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObject_GeometricData_Xenon2 : BinarySerializable {
		public uint Version { get; set; } // Set in on PreSerialize

		public uint Type4_UInt { get; set; }
		public uint Type5_UInt { get; set; }
		public uint Type6_UInt { get; set; }
		public Jade_Reference<GEO_XenonPack> XenonPack { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (Version >= 4) Type4_UInt = s.Serialize<uint>(Type4_UInt, name: nameof(Type4_UInt));
			if (Version >= 5) Type5_UInt = s.Serialize<uint>(Type5_UInt, name: nameof(Type5_UInt));
			if (Version >= 6) Type6_UInt = s.Serialize<uint>(Type6_UInt, name: nameof(Type6_UInt));
			if (Version >= 7) {
				XenonPack = s.SerializeObject<Jade_Reference<GEO_XenonPack>>(XenonPack, name: nameof(XenonPack));

				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if (Loader.IsBinaryData) {
					XenonPack?.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontCache);
				}
			}
		}
	}
}
