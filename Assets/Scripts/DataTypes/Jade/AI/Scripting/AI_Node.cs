namespace R1Engine.Jade {
	public class AI_Node : R1Serializable {
		public uint Parameter { get; set; }
		public ushort NodeType { get; set; }
		public byte Flags { get; set; }
		public byte CategoryType { get; set; }

		public AI_Link Link_ParameterType { get; set; }
		public AI_Link Link_CategoryType { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Parameter = s.Serialize<uint>(Parameter, name: nameof(Parameter));
			NodeType = s.Serialize<ushort>(NodeType, name: nameof(NodeType));

			Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
			CategoryType = s.Serialize<byte>(CategoryType, name: nameof(CategoryType));
			
			var links = Context.GetStoredObject<AI_Links>("ai");
			if (links.Links.ContainsKey(NodeType)) {
				Link_ParameterType = links.Links[NodeType];
				s.Log($"Node param function name: {Link_ParameterType.Name}");
			}
			if (links.Links.ContainsKey(CategoryType)) {
				Link_CategoryType = links.Links[CategoryType];
				s.Log($"Node category function name: {Link_CategoryType.Name}");
			}
		}
	}
}
