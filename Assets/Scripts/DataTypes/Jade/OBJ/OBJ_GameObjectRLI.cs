using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObjectRLI : Jade_File {
		public Jade_Code RLICode { get; set; }
		public uint[] UInts { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RLICode = s.Serialize<Jade_Code>(RLICode, name: nameof(RLICode));
			if(RLICode != Jade_Code.RLI)
				throw new NotImplementedException($"Parsing failed: wrong code {RLICode} in header of {GetType()}");
			UInts = s.SerializeArray<uint>(UInts, (FileSize - 4) / 4, name: nameof(UInts));
			// TODO: Game doesn't parse this properly. First uint is a count, rest are colors
		}
	}
}
