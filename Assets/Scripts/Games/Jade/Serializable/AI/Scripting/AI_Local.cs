using BinarySerializer;
using System.Collections.Generic;

namespace Ray1Map.Jade {
	public class AI_Local : BinarySerializable {
		public int LocalOffset { get; set; }
		public uint Type { get; set; }
		public string Name { get; set; }
		public byte Byte0 { get; set; }
		public byte Byte1 { get; set; }

		public AI_Link Link_Type { get; set; }
		public AI_VarType LinkType { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			LocalOffset = s.Serialize<int>(LocalOffset, name: nameof(LocalOffset));
			Type = s.Serialize<uint>(Type, name: nameof(Type));
			Name = s.SerializeString(Name, length: 30, name: nameof(Name));
			Byte0 = s.Serialize<byte>(Byte0, name: nameof(Byte0));
			Byte1 = s.Serialize<byte>(Byte1, name: nameof(Byte1));

			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.Links.ContainsKey(Type)) {
				Link_Type = links.Links[Type];
				//s.Log($"Node param function name: {Link_ParameterType.Name}");
				LinkType = Link_Type.VarType;
			}
		}

		public override bool UseShortLog => true;
		public override string ShortLog => LocalToString();

		public string LocalToString() {
			var cat = Link_Type?.Name ?? ("Type_" + Type);
			return $"{cat.Replace("AI_Eval", "")} {Name} ({LocalOffset})";
		}
	}
}
