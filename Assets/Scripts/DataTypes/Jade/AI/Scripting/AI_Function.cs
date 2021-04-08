using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Function : Jade_File {
		public int Int_00 { get; set; }
		public uint FunctionBufferLength { get; set; }
		public AI_Node[] Nodes { get; set; }
		public uint UnknownBufferLength { get; set; }
		public AI_Node_Unknown[] Unknown { get; set; }
		public uint StringBufferLength { get; set; }
		public byte[] StringBuffer { get; set; }
		public string[] Strings { get; set; }

		// Custom
		public AI_FunctionDef FunctionDef { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.CompiledFunctions.ContainsKey(Key)) {
				FunctionDef = links.CompiledFunctions[Key];
				s.Log($"Compiled function found! Function name: {FunctionDef.Name}");
			}

			Int_00 = s.Serialize<int>(Int_00, name: nameof(Int_00));
			FunctionBufferLength = s.Serialize<uint>(FunctionBufferLength, name: nameof(FunctionBufferLength));

			if (FunctionBufferLength > 0 && (FunctionDef == null || !Loader.IsBinaryData)) {
				Nodes = s.SerializeObjectArray<AI_Node>(Nodes, FunctionBufferLength / 8, name: nameof(Nodes));
			}
			if (!Loader.IsBinaryData) {
				UnknownBufferLength = s.Serialize<uint>(UnknownBufferLength, name: nameof(UnknownBufferLength));
				Unknown = s.SerializeObjectArray<AI_Node_Unknown>(Unknown, UnknownBufferLength / 8, name: nameof(Unknown));
			}
			if (s.GetR1Settings().EngineVersion != EngineVersion.Jade_RRR_Xbox360) {
				StringBufferLength = s.Serialize<uint>(StringBufferLength, name: nameof(StringBufferLength));
			} else {
				StringBufferLength = FileSize - (uint)(s.CurrentPointer - Offset);
			}
			Pointer stringBufferStart = s.CurrentPointer;
			s.DoAt(stringBufferStart, () => {
				if (Strings == null) {
					List<string> strings = new List<string>();
					Pointer targetPointer = s.CurrentPointer + StringBufferLength;
					int ind = 0;
					while (s.CurrentPointer.AbsoluteOffset < targetPointer.AbsoluteOffset) {
						strings.Add(s.SerializeString(default, encoding: Jade_BaseManager.Encoding, name: $"{nameof(Strings)}[{ind}]"));
						ind++;
					}
					Strings = strings.ToArray();
				} else {
					for (int i = 0; i < Strings.Length; i++) {
						Strings[i] = s.SerializeString(Strings[i], encoding: Jade_BaseManager.Encoding, name: $"{nameof(Strings)}[{i}]");
					}
				}
			});
			StringBuffer = s.SerializeArray<byte>(StringBuffer, StringBufferLength, name: nameof(StringBuffer));
			if (!Loader.SpeedMode) {
				throw new NotImplementedException("AI_Function: Implement Locals");
			}
		}
	}
}
