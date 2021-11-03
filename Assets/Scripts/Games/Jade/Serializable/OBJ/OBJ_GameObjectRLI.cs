using System;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class OBJ_GameObjectRLI : Jade_File {
		public override string Export_Extension => "rli";
		public Jade_Code RLICode { get; set; }
		public uint VertexColorsCount { get; set; }
		public Jade_Color[] VertexRLI { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			RLICode = s.Serialize<Jade_Code>(RLICode, name: nameof(RLICode));
			if(RLICode != Jade_Code.RLI)
				throw new NotImplementedException($"Parsing failed: wrong code {RLICode} in header of {GetType()}");
			VertexColorsCount = s.Serialize<uint>(VertexColorsCount, name: nameof(VertexColorsCount));
			VertexRLI = s.SerializeObjectArray<Jade_Color>(VertexRLI, VertexColorsCount, name: nameof(VertexRLI));
		}
	}
}
