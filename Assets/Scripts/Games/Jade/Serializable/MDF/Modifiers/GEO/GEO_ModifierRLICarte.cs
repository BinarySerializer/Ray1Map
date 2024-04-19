using BinarySerializer;

namespace Ray1Map.Jade {
	// Found in GEO_ul_ModifierRLICarte_Load
	public class GEO_ModifierRLICarte : MDF_Modifier {
		public uint UInt_Editor_00 { get; set; }
		public SerializableColor[] Colors { get; set; }
		public byte Flags { get; set; }
		public byte Op { get; set; }
		public byte InternalFlags { get; set; }
		public byte Dummy2 { get; set; }
		public uint PointsCount { get; set; }
		public byte[] PtGroup { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if(!Loader.IsBinaryData) UInt_Editor_00 = s.Serialize<uint>(UInt_Editor_00, name: nameof(UInt_Editor_00));
			Colors = s.SerializeIntoArray<SerializableColor>(Colors, 64, name: nameof(Colors));
			Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
			Op = s.Serialize<byte>(Op, name: nameof(Op));
			InternalFlags = s.Serialize<byte>(InternalFlags, name: nameof(InternalFlags));
			Dummy2 = s.Serialize<byte>(Dummy2, name: nameof(Dummy2));
			PointsCount = s.Serialize<uint>(PointsCount, name: nameof(PointsCount));
			PtGroup = s.SerializeArray<byte>(PtGroup, PointsCount, name: nameof(PtGroup));
		}
	}
}
