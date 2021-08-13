using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class AI_Model: Jade_File {
		public override string Export_Extension => "omd";
		public override string Export_FileBasename => FunctionDef?.Name;

		public Jade_GenericReference[] References { get; set; }

		// Custom
		public AI_FunctionDef FunctionDef { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			var links = Context.GetStoredObject<AI_Links>(Jade_BaseManager.AIKey);
			if (links.CompiledFunctions.ContainsKey(Key)) {
				FunctionDef = links.CompiledFunctions[Key];
				s.Log($"Compiled function found! Function name: {FunctionDef.Name}");
			}

			References = s.SerializeObjectArray<Jade_GenericReference>(References, FileSize / 8, name: nameof(References));
			foreach (var reference in References) {
				switch (reference.Type) {
					case Jade_FileType.FileType.AI_TT:
						reference.Resolve(flags: LOA_Loader.ReferenceFlags.DontCache);
						break;
					default:
						reference.Resolve();
						break;
				}
			}
		}

		public AI_Vars Vars => References?.FirstOrDefault(r => !r.IsNull && r.Type == Jade_FileType.FileType.AI_Vars)?.Value as AI_Vars;
	}
}
