using System;

namespace R1Engine.Jade {
	public class OBJ_GameObject : Jade_File {
		public Jade_FileType Type { get; set; }
		public uint UInt_04_Editor { get; set; }
		public uint UInt_04 { get; set; }
		public uint FlagsIdentity { get; set; }
		public ushort FlagsStatus { get; set; }
		public ushort FlagsControl { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
			if(!Loader.IsBinaryData) UInt_04_Editor = s.Serialize<uint>(UInt_04_Editor, name: nameof(UInt_04_Editor));
			UInt_04 = s.Serialize<uint>(UInt_04, name: nameof(UInt_04));
			FlagsIdentity = s.Serialize<uint>(FlagsIdentity, name: nameof(FlagsIdentity));
			s.SerializeBitValues<uint>(bitFunc => {
				FlagsStatus = (ushort)bitFunc(FlagsStatus, 16, name: nameof(FlagsStatus));
				FlagsControl = (ushort)bitFunc(FlagsControl, 16, name: nameof(FlagsControl));
			});
			throw new NotImplementedException("TODO: Implement OBJ_GameObject");
		}
	}
}
