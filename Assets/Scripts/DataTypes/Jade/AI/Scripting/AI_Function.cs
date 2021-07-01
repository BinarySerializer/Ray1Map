using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Function : Jade_File {
		public int SizeLocalStack { get; set; }
		public uint FunctionBufferLength { get; set; }
		public AI_Node[] Nodes { get; set; }
		public uint DebugBufferLength { get; set; }
		public AI_Node_DebugLink[] DebugLinks { get; set; }
		public uint StringBufferLength { get; set; }
		public byte[] StringBuffer { get; set; }
		public string[] Strings { get; set; }
		public Dictionary<long, int> StringOffsetToIndex { get; set; }

		// Custom
		public AI_FunctionDef FunctionDef { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.CompiledFunctions.ContainsKey(Key)) {
				FunctionDef = links.CompiledFunctions[Key];
				s.Log($"Compiled function found! Function name: {FunctionDef.Name}");
			}

			SizeLocalStack = s.Serialize<int>(SizeLocalStack, name: nameof(SizeLocalStack));
			FunctionBufferLength = s.Serialize<uint>(FunctionBufferLength, name: nameof(FunctionBufferLength));

			if (FunctionBufferLength > 0 && (FunctionDef == null || !Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal))) {
				Nodes = s.SerializeObjectArray<AI_Node>(Nodes, FunctionBufferLength / 8, name: nameof(Nodes));
			}
			if (!Loader.IsBinaryData) {
				DebugBufferLength = s.Serialize<uint>(DebugBufferLength, name: nameof(DebugBufferLength));
				DebugLinks = s.SerializeObjectArray<AI_Node_DebugLink>(DebugLinks, DebugBufferLength / 8, name: nameof(DebugLinks));
			}
			if (!s.GetR1Settings().EngineFlags.HasFlag(EngineFlags.Jade_Xenon) || !Loader.IsBinaryData) {
				StringBufferLength = s.Serialize<uint>(StringBufferLength, name: nameof(StringBufferLength));
			} else {
				StringBufferLength = FileSize - (uint)(s.CurrentPointer - Offset);
			}
			Pointer stringBufferStart = s.CurrentPointer;
			s.DoAt(stringBufferStart, () => {
				if (Strings == null) {
					List<string> strings = new List<string>();
					StringOffsetToIndex = new Dictionary<long, int>();
					Pointer targetPointer = s.CurrentPointer + StringBufferLength;
					int ind = 0;
					while (s.CurrentPointer.AbsoluteOffset < targetPointer.AbsoluteOffset) {
						long stringoffset = s.CurrentPointer.AbsoluteOffset - stringBufferStart.AbsoluteOffset;
						StringOffsetToIndex[stringoffset] = ind;
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
			//if (!Loader.IsBinaryData) ExportScript(s);
		}

		public void ExportScript(SerializerObject s) {
			StringBuilder b = new StringBuilder();
			//b.AppendLine("[Translated]");

			for (int i = 0; i < Nodes.Length; i++) {
				b.AppendLine($"{DebugLinks[i].DebugLinkContent.PadRight(32)}{Nodes[i].ToString(StringOffsetToIndex, Strings)}");
			}
			string basePath = Context.BasePath;
			string path = basePath + "scripts/" + FunctionDef.Name + "_" + s.GetR1Settings().Platform + ".ofcdec";
			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
			System.IO.File.WriteAllText(path, b.ToString());
		}
	}
}
