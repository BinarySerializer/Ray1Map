using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WAY_Network : Jade_File {
		public override string Export_Extension => "way";
		public override bool HasHeaderBFFile => true;

		public Jade_Reference<OBJ_GameObject> Root { get; set; }
		public uint Flags { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Root = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Root, name: nameof(Root))?.Resolve();
			Flags = s.Serialize<uint>(Flags, name: nameof(Flags));
		}
	}
}
