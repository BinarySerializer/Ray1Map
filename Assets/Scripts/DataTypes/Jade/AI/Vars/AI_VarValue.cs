﻿using System;
using System.Globalization;
using System.Linq;

namespace R1Engine.Jade {
	public class AI_VarValue : R1Serializable {
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
						ValueInt = ValueBool ? 1 : 0;
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
						ValueKey = s.SerializeObject<Jade_Key>(ValueKey, name: nameof(ValueKey));
						break;
					case AI_VarType.Vector:
						if (Var.Link.Key == 37) { // TODO: Why only this vector type?
							ValueVector = s.SerializeObject<Jade_Vector>(ValueVector, name: nameof(ValueVector));
						}
						break;
					case AI_VarType.Message:
					case AI_VarType.Text:
					case AI_VarType.MessageId:
					case AI_VarType.Trigger:
						// TODO: Add these. They're read in AI_ul_CallbackLoadVars
						throw new NotImplementedException();
					default:
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
					default:
						if (Var.Link.Size == 4) {
							return $"{Var.Type} ({ValueInt})";
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
			get => (uint)ValueInt;
			set => ValueInt = (int)value;
		}

		// TODO: Add other getters/setters
	}
}