using System;

namespace R1Engine.Jade {
	public class OBJ_GameObject : Jade_File {
		public Jade_FileType Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
			throw new NotImplementedException("TODO: Implement OBJ_GameObject");

		}
	}
}
