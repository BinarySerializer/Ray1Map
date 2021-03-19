using System;

namespace R1Engine.Jade {
	public class AI_VarValue : R1Serializable {
		// Set in onPreSerialize
		public AI_Var Var { get; set; }

		// Struct fields
		public uint[] Dimensions { get; set; }
		public bool IsArrayElement { get; set; }

		// Values
		public int ValueInt { get; set; }
		public Jade_Vector ValueVector { get; set; }
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
				if (Var.Link.Size == 4) {
					ValueInt = s.Serialize<int>(ValueInt, name: nameof(ValueInt));
				} else {
					switch (Var.Type) {
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
					}
				}
			}
		}

		public bool ValueBool {
			get => ValueInt != 0;
			set => ValueInt = value ? 1 : 0;
		}

		public uint ValueUInt {
			get => (uint)ValueInt;
			set => ValueInt = (int)value;
		}

		// TODO: Add other getters/setters
	}
}
