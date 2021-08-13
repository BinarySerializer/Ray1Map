using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_ProcList : Jade_File {
		public override string Export_Extension => "fce";
		public override string Export_FileBasename => FunctionDef?.Name;

		public ushort ProcsCount { get; set; }
		public Proc[] Procs { get; set; }
		public uint Code { get; set; }
		public uint ProcListStringLength { get; set; }
		public string ProcListString { get; set; }
		public uint Unknown_04 { get; set; }

		// Custom
		public AI_FunctionDef FunctionDef { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.CompiledFunctions.ContainsKey(Key)) {
				FunctionDef = links.CompiledFunctions[Key];
				s.Log($"Compiled function found! Function name: {FunctionDef.Name}");
			}

			ProcsCount = s.Serialize<ushort>(ProcsCount, name: nameof(ProcsCount));
			Procs = s.SerializeObjectArray<Proc>(Procs, ProcsCount, name: nameof(Procs));
			Code = s.Serialize<uint>(Code, name: nameof(Code));
			if (Code == (uint)Jade_Code.Code6660) {
				ProcListStringLength = s.Serialize<uint>(ProcListStringLength, name: nameof(ProcListStringLength));
				ProcListString = s.SerializeString(ProcListString, length: ProcListStringLength, encoding: Jade_BaseManager.Encoding, name: nameof(ProcListString));
				Unknown_04 = s.Serialize<uint>(Unknown_04, name: nameof(Unknown_04));
			} else {
				Unknown_04 = Code;
			}
		}

		public class Proc : BinarySerializable {
			public uint NameLength { get; set; }
			public string Name { get; set; }
			public uint Code { get; set; }
			public ushort UShort_Code { get; set; }
			public uint NodesCount { get; set; }
			public AI_Node[] Nodes { get; set; }
			public AI_Node_DebugLink[] DebugLinks { get; set; }
			public uint Code2 { get; set; }
			public uint LocalsCount { get; set; }
			public byte[] LocalsStringBuffer { get; set; }
			public AI_Local[] Locals { get; set; }

			// Custom
			public uint BinaryCode { get; set; }
			public uint BinaryNodesCount { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				NameLength = s.Serialize<uint>(NameLength, name: nameof(NameLength));
				Name = s.SerializeString(Name, length: NameLength, encoding: Jade_BaseManager.Encoding, name: nameof(Name));
				Code = s.Serialize<uint>(Code, name: nameof(Code));
				if (Code == (uint)Jade_Code.ACBD) {
					UShort_Code = s.Serialize<ushort>(UShort_Code, name: nameof(UShort_Code));
					NodesCount = s.Serialize<uint>(NodesCount, name: nameof(NodesCount));
				} else {
					NodesCount = Code;
				}
				if (!Loader.IsBinaryData || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_RRR2)) {
					Nodes = s.SerializeObjectArray<AI_Node>(Nodes, NodesCount, name: nameof(Nodes));
					if (!Loader.IsBinaryData) {
						DebugLinks = s.SerializeObjectArray<AI_Node_DebugLink>(DebugLinks, NodesCount, name: nameof(DebugLinks));
						Code2 = s.Serialize<uint>(Code2, name: nameof(Code2));
						if (Code2 == (uint)Jade_Code.All6) {
							LocalsCount = s.Serialize<uint>(LocalsCount, name: nameof(LocalsCount));
							LocalsStringBuffer = s.SerializeArray<byte>(LocalsStringBuffer, 0x400, name: nameof(LocalsStringBuffer));
						} else {
							LocalsCount = Code2;
						}
						Locals = s.SerializeObjectArray<AI_Local>(Locals, LocalsCount, name: nameof(Locals));
					}
				}
			}
		}


		protected override void OnChangedIsBinaryData() {
			base.OnChangedIsBinaryData();
			if (CurrentIsBinaryData == false) {
				foreach (var p in Procs) {
					if (p.Code != 0) p.BinaryCode = p.Code;
					if (p.NodesCount != 0) p.BinaryNodesCount = p.BinaryNodesCount;
					p.NodesCount = 0;
					if(p.Code != (uint)Jade_Code.ACBD) Code = 0;
				}
			} else if (CurrentIsBinaryData == true) {
				foreach (var p in Procs) {
					if (p.BinaryNodesCount > 0) {
						p.NodesCount = p.BinaryNodesCount;
						p.Code = p.BinaryCode;
					}
				}
			}
		}
	}
}
