using System.Collections.Generic;
using System.Text;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_Function : Jade_File {
		public override string Export_Extension => "ofc";
		public override string Export_FileBasename => FunctionDef?.Name;

		public int SizeLocalStack { get; set; }
		public uint FunctionBufferLength { get; set; }
		public AI_Node[] Nodes { get; set; }
		public uint DebugBufferLength { get; set; }
		public AI_Node_DebugLink[] DebugLinks { get; set; }
		public uint StringBufferLength { get; set; }
		public byte[] StringBuffer { get; set; }
		public string[] Strings { get; set; }
		public Dictionary<long, int> StringOffsetToIndex { get; set; }
		public uint LocalStackFrameBufferLength { get; set; }
		public AI_LocalStackFrame[] LocalStackFrame { get; set; }
		public uint LocalsBufferLength { get; set; }
		public AI_Local[] Locals { get; set; }
		public int TMNT_LocalsUnknown0 { get; set; }
		public bool TMNT_LocalsUnknownFlag { get; set; }
		public uint TMNT_LocalsUnknownBufferSize { get; set; }
		public AI_Local[] LocalsUnknown { get; set; }

		// Custom
		public AI_FunctionDef FunctionDef { get; set; }
		public uint BinaryFunctionBufferLength { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.CompiledFunctions.ContainsKey(Key)) {
				FunctionDef = links.CompiledFunctions[Key];
				s.Log("Compiled function found! Function name: {0}", FunctionDef.Name);
			}

			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT)) {
				s.DoBits<uint>(b => {
					TMNT_LocalsUnknown0 = b.SerializeBits<int>(TMNT_LocalsUnknown0, 31, name: nameof(TMNT_LocalsUnknown0));
					TMNT_LocalsUnknownFlag = b.SerializeBits<bool>(TMNT_LocalsUnknownFlag, 1, name: nameof(TMNT_LocalsUnknownFlag));
				});
				if(TMNT_LocalsUnknownFlag)
					SizeLocalStack = s.Serialize<int>(SizeLocalStack, name: nameof(SizeLocalStack));
				else
					SizeLocalStack = TMNT_LocalsUnknown0;
			} else {
				SizeLocalStack = s.Serialize<int>(SizeLocalStack, name: nameof(SizeLocalStack));
			}
			FunctionBufferLength = s.Serialize<uint>(FunctionBufferLength, name: nameof(FunctionBufferLength));
			if(Loader.IsBinaryData) BinaryFunctionBufferLength = FunctionBufferLength;

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
			if (!(s is BinarySerializer.BinarySerializer)) {
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
			}
			StringBuffer = s.SerializeArray<byte>(StringBuffer, StringBufferLength, name: nameof(StringBuffer));
			if (!Loader.IsBinaryData) { // Actually, check is for SpeedMode, but BinaryData check is better in this case

				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT) &&
					(s.CurrentAbsoluteOffset - Offset.AbsoluteOffset < FileSize)) {
					TMNT_LocalsUnknownBufferSize = s.Serialize<uint>(TMNT_LocalsUnknownBufferSize, name: nameof(TMNT_LocalsUnknownBufferSize));

					var localSize = s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)
									? ((TMNT_LocalsUnknownFlag && TMNT_LocalsUnknown0 != 0) ? 0x40 : 0x3c)
									: 40;
					LocalsUnknown = s.SerializeObjectArray<AI_Local>(LocalsUnknown, TMNT_LocalsUnknownBufferSize / localSize, name: nameof(LocalsUnknown));
				}
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_TMNT) &&
					(s.CurrentAbsoluteOffset - Offset.AbsoluteOffset < FileSize)) {
					LocalStackFrameBufferLength = s.Serialize<uint>(LocalStackFrameBufferLength, name: nameof(LocalStackFrameBufferLength));
					LocalStackFrame = s.SerializeObjectArray<AI_LocalStackFrame>(LocalStackFrame, LocalStackFrameBufferLength / 0x10, name: nameof(LocalStackFrame));
				}
				if (s.CurrentAbsoluteOffset - Offset.AbsoluteOffset < FileSize) {
					var localSize = s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)
									? ((TMNT_LocalsUnknownFlag && TMNT_LocalsUnknown0 != 0) ? 0x40 : 0x3c)
									: 40;
					LocalsBufferLength = s.Serialize<uint>(LocalsBufferLength, name: nameof(LocalsBufferLength));
					Locals = s.SerializeObjectArray<AI_Local>(Locals, LocalsBufferLength / localSize, name: nameof(Locals));
				}
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

		protected override void OnChangedIsBinaryData() {
			base.OnChangedIsBinaryData();
			if (CurrentIsBinaryData == false && Nodes == null) {
				FunctionBufferLength = 0;
			} else if (CurrentIsBinaryData == true && BinaryFunctionBufferLength > 0) {
				FunctionBufferLength = BinaryFunctionBufferLength;
			}
		}
	}
}
