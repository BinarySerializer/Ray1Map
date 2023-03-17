using BinarySerializer;
using System.Collections.Generic;

namespace Ray1Map.Jade {
	public class AI_Node : BinarySerializable, ISerializerShortLog {
		public int Parameter { get; set; }
		public ushort NodeType { get; set; }
		public byte Flags { get; set; }
		public byte CategoryType { get; set; }

		public AI_Link Link_ParameterType { get; set; }
		public AI_Link Link_CategoryType { get; set; }
		public AI_VarType LinkType { get; set; }

		public bool ValueBool { get; set; }
		public float ValueFloat { get; set; }
		public Jade_Key ValueKey { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Parameter = s.Serialize<int>(Parameter, name: nameof(Parameter));
			NodeType = s.Serialize<ushort>(NodeType, name: nameof(NodeType));

			Flags = s.Serialize<byte>(Flags, name: nameof(Flags));
			CategoryType = s.Serialize<byte>(CategoryType, name: nameof(CategoryType));
			
			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.Links.ContainsKey(NodeType)) {
				Link_ParameterType = links.Links[NodeType];
				//s.Log($"Node param function name: {Link_ParameterType.Name}");
				LinkType = Link_ParameterType.VarType;
			}
			if (links.Links.ContainsKey(CategoryType)) {
				Link_CategoryType = links.Links[CategoryType];
				//s.Log($"Node category function name: {Link_CategoryType.Name}");
			}

			if (LinkType != AI_VarType.None) {
				ValueBool = Parameter != 0;
				s.DoAt(Offset, () => {
					switch (LinkType) {
						case AI_VarType.Float:
							ValueFloat = s.Serialize<float>(ValueFloat, name: nameof(ValueFloat));
							break;
						case AI_VarType.Key:
						case AI_VarType.PointerRef:
						case AI_VarType.GAO:

						case AI_VarType.Function: // All of these are PointerRef
						case AI_VarType.Model:
						case AI_VarType.Network:
							ValueKey = s.SerializeObject<Jade_Key>(ValueKey, name: nameof(ValueKey));
							break;
					}
				});
			}
			
		}

		public string ValueToString() {
			switch (LinkType) {
				case AI_VarType.Bool: return ValueBool.ToString();
				case AI_VarType.Float: return $"{Parameter}/{ValueFloat}";
				case AI_VarType.Key:
				case AI_VarType.GAO:
					return ValueKey.ToString();
				case AI_VarType.PointerRef:
				case AI_VarType.Function: // All of these are PointerRef
				case AI_VarType.Model:
				case AI_VarType.Network:
					var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
					if (links.CompiledFunctions.ContainsKey(ValueKey)) return links.CompiledFunctions[ValueKey].Name;
					if (links.Links.ContainsKey(ValueKey)) return links.Links[ValueKey].Name;
					return ValueKey.ToString();
				default: return Parameter.ToString();
			}
		}

		public string ShortLog => ToString(null, null);

		public string ToString(Dictionary<long, int> stringOffsetDictionary, string[] stringList) {
			var cat = Link_CategoryType?.Name ?? ("Category_" + CategoryType);
			var nodeType = Link_ParameterType?.Name ?? ("NodeType_" + NodeType);
			if (Link_ParameterType?.OverrideType != null) nodeType += $"/{Link_ParameterType.OverrideType}";
			var val = ValueToString();

			if (stringOffsetDictionary != null && stringList != null) {
				if (Link_CategoryType?.Name == "AI_EvalCateg_Type" && Link_ParameterType?.VarType == AI_VarType.String) {
					if(stringOffsetDictionary.ContainsKey(Parameter)) val = $"\"{stringList[stringOffsetDictionary[Parameter]]}\"";
				}
			}

			return $"{cat.Replace("AI_Eval", ""),-32}{nodeType.Replace("AI_Eval", ""),-32}{val}";
		}
	}
}
