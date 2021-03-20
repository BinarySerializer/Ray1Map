using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R1Engine.Jade {
	public class AI_Link {
		public uint Key { get; set; }
		public uint Size { get; set; }
		public string Name { get; set; }

		public AI_Link(uint key, uint size, string name, AI_VarType? overrideType = null) {
			Key = key;
			Size = size;
			Name = name;
			OverrideType = overrideType;
		}

		public AI_VarType? OverrideType { get; set; }

		public AI_VarType VarType {
			get {
				if(OverrideType.HasValue) return OverrideType.Value;
				if(Size == 0) return AI_VarType.None;
				if (Name.StartsWith("AI_EvalType_Get")) {
					var typeName = Name.Substring("AI_EvalType_Get".Length);
					switch (typeName) {
						case "Bool": return AI_VarType.Bool;
						case "Int": return AI_VarType.Int;
						case "Vector": return AI_VarType.Vector;
						case "String": return AI_VarType.String;
						case "PointerRef": return AI_VarType.PointerRef;
						case "GAO": return AI_VarType.GAO;
						case "Message": return AI_VarType.Message;
						case "Text": return AI_VarType.Text;
						case "Key": return AI_VarType.Key;
						case "Color": return AI_VarType.Color;
						case "MessageId": return AI_VarType.MessageId;
						case "Trigger": return AI_VarType.Trigger;
						default: return AI_VarType.Unknown;
					}
				}
				return AI_VarType.Unknown;
			}
		}
	}
}
