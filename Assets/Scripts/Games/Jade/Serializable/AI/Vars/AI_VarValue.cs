using System;
using System.Globalization;
using System.Linq;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_VarValue : BinarySerializable {
		// Set in onPreSerialize
		public AI_Var Var { get; set; }

		// Struct fields
		public uint[] Dimensions { get; set; }
		public bool IsArrayElement { get; set; }

		// Values
		public bool ValueBool { get; set; }
		public int ValueInt { get; set; }
		public float ValueFloat { get; set; }
		public Jade_Vector ValueVector { get; set; }
		public Jade_Key ValueKey { get; set; }
		public AI_VarValue[] ValueArray { get; set; }
		public Jade_Color ValueColor { get; set; }
		public long ValueLong { get; set; }

		public Jade_Key TextFile { get; set; }
		public Jade_Key TextEntryID { get; set; }

		public int MsgID_MsgID { get; set; }
		public int MsgID_ID { get; set; }

		public AI_Trigger ValueTrigger { get; set; }
		public AI_Message ValueMessage { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!IsArrayElement) {
				Dimensions = s.SerializeArray<uint>(Dimensions, Var.Info.ArrayDimensionsCount, name: nameof(Dimensions));
				if (Var.Info.ArrayLength != 1) {
					ValueArray = s.SerializeObjectArray<AI_VarValue>(ValueArray, Var.Info.ArrayLength, onPreSerialize: v => {
						v.IsArrayElement = true;
						v.Var = Var;
					}, name: nameof(ValueArray));
				}
			}
			if (IsArrayElement || Var.Info.ArrayLength == 1) {
				switch (Var.Type) {
					case AI_VarType.Bool:
						//ValueInt = ValueBool ? 1 : 0;
						ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
						ValueBool = ValueInt != 0;
						break;
					case AI_VarType.Int:
						ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
						break;
					case AI_VarType.Float:
						ValueFloat = s.Serialize<float>(ValueFloat, name: nameof(ValueFloat));
						break;
					case AI_VarType.Key:
					case AI_VarType.GAO:

					case AI_VarType.PointerRef:
					case AI_VarType.Network:
					case AI_VarType.Function:
					case AI_VarType.Model:
					case AI_VarType.Sound:
					case AI_VarType.FlashMovie:
						ValueKey = s.SerializeObject<Jade_Key>(ValueKey, name: nameof(ValueKey));
						break;
					case AI_VarType.Vector:
						ValueVector = s.SerializeObject<Jade_Vector>(ValueVector, name: nameof(ValueVector));
						break;
					case AI_VarType.Text:
						TextFile = s.SerializeObject<Jade_Key>(TextFile, name: nameof(TextFile));
						TextEntryID = s.SerializeObject<Jade_Key>(TextEntryID, onPreSerialize: k => k.Pre_IsTextKey = true, name: nameof(TextEntryID));
						break;
					case AI_VarType.MessageId:
						MsgID_MsgID = s.Serialize<int>(MsgID_MsgID, name: nameof(MsgID_MsgID));
						MsgID_ID = s.Serialize<int>(MsgID_ID, name: nameof(MsgID_ID));
						break;
					case AI_VarType.Message:
						ValueMessage = s.SerializeObject<AI_Message>(ValueMessage, name: nameof(ValueMessage));
						break;
					case AI_VarType.Trigger:
						ValueTrigger = s.SerializeObject<AI_Trigger>(ValueTrigger, name: nameof(ValueTrigger));
						break;
					case AI_VarType.String:
						ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
						break;
					case AI_VarType.Color:
						ValueColor = s.SerializeObject<Jade_Color>(ValueColor, name: nameof(ValueColor));
						break;
					case AI_VarType.Long64:
						ValueLong = s.Serialize<long>(ValueLong, name: nameof(ValueLong));
						break;
					default:
						//throw new Exception($"What should I do for {Var.Type}?");
						s.SystemLogger?.LogInfo($"Value with type {Var.Type} at {s.CurrentPointer}");
						if (Var.Link.Size == 4) {
							ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
						}
						break;
				}
			}
		}

		public override string ToString() {
			if (IsArrayElement || Var.Info.ArrayLength == 1) {
				switch (Var.Type) {
					case AI_VarType.Bool: return ValueBool.ToString();
					case AI_VarType.Int: return ValueInt.ToString();
					case AI_VarType.Float:
						NumberFormatInfo nfi = new NumberFormatInfo() {
							NumberDecimalSeparator = "."
						};
						return ValueFloat.ToString(nfi);
					case AI_VarType.Key: return ValueKey.ToString();
					case AI_VarType.Vector:
						if(ValueVector != null) return ValueVector.ToString();
						break;
					case AI_VarType.Trigger: return ValueTrigger.ToString();
					case AI_VarType.Message: return ValueMessage.ToString();
					default:
						if (Var.Link.Size == 4) {
							return $"{Var.Type} ({ValueInt} | 0x{ValueInt:X8})";
						}
						break;
				}
				return Var.Type.ToString();
			} else {
				string dimensions = string.Join(",",Dimensions);
				string values = string.Join(", ", ValueArray.Select(v => v.ToString()));
				return $"{Var.Type}[{dimensions}] {{ {values} }}";
			}
		}

		public uint ValueUInt {
			get => unchecked((uint)ValueInt);
			set => ValueInt = unchecked((int)value);
		}

		// TODO: Add other getters/setters
	}
}
