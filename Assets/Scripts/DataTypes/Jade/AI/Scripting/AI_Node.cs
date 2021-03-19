namespace R1Engine.Jade {
	public class AI_Node : R1Serializable {
		public uint Parameter { get; set; }
		public ushort NodeType { get; set; }
		public byte Byte_06 { get; set; }
		public byte Byte_07 { get; set; }

		public AI_Link Link { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Parameter = s.Serialize<uint>(Parameter, name: nameof(Parameter));
			NodeType = s.Serialize<ushort>(NodeType, name: nameof(NodeType));

			Byte_06 = s.Serialize<byte>(Byte_06, name: nameof(Byte_06));
			Byte_07 = s.Serialize<byte>(Byte_07, name: nameof(Byte_07));
			
			var links = Context.GetStoredObject<AI_Links>("ai");
			if (links.Links.ContainsKey(NodeType)) {
				Link = links.Links[NodeType];
				s.Log($"Node function name: {Link.Name}");
			}
		}
	}
}
