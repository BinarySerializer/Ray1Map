using BinarySerializer;

namespace Ray1Map.Jade {
	public class GRP_Grp : Jade_File {
		public override string Export_Extension => "grp";
		public override bool HasHeaderBFFile => true;

		public Jade_Reference<OBJ_World_GroupObjectList> GroupObjectList { get; set; }
		public uint UInt_04 { get; set; }
		public uint Editor_UInt_08 { get; set; }
		public uint Editor_UInt_0C { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			GroupObjectList = s.SerializeObject<Jade_Reference<OBJ_World_GroupObjectList>>(GroupObjectList, name: nameof(GroupObjectList))?.Resolve();
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			if (!Loader.IsBinaryData) {
				Editor_UInt_08 = s.Serialize<uint>(Editor_UInt_08, name: nameof(Editor_UInt_08));
				Editor_UInt_0C = s.Serialize<uint>(Editor_UInt_0C, name: nameof(Editor_UInt_0C));
			}
		}
	}
}
