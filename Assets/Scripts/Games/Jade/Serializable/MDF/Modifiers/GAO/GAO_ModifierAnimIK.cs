using BinarySerializer;

namespace Ray1Map.Jade {
	public class GAO_ModifierAnimIK : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public uint UInt_04 { get; set; }
		public uint Flags { get; set; }
		public float MaxExtension { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
			MaxExtension = s.Serialize<float>(MaxExtension, name: nameof(MaxExtension));
		}
	}
}
