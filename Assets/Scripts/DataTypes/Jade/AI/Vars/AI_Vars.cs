using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class AI_Vars : Jade_File {
		public uint VarInfosBufferSize { get; set; }
		public AI_VarInfo[] VarInfos { get; set; }
		public uint NameBufferSize { get; set; }
		public string[] Names { get; set; }
		public uint VarEditorInfoBufferSize { get; set; }
		public int VarEditorInfoStringBufferSize { get; set; }
		public AI_VarEditorInfo[] VarEditorInfos { get; set; }
		public uint VarValueBufferSize { get; set; }
		public AI_VarValue[] Values { get; set; }

		public Jade_Reference<AI_Function>[] Functions { get; set; }

		// Custom
		public AI_Var[] Vars { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			// Normal var data
			VarInfosBufferSize = s.Serialize<uint>(VarInfosBufferSize, name: nameof(VarInfosBufferSize));
			VarInfos = s.SerializeObjectArray<AI_VarInfo>(VarInfos, VarInfosBufferSize / 12, name: nameof(VarInfos));
			NameBufferSize = s.Serialize<uint>(NameBufferSize, name: nameof(NameBufferSize));
			if (NameBufferSize > 0) {
				Names = s.SerializeStringArray(Names, VarInfos.Length, 30, name: nameof(Names));
			}

			// Generate main Vars array
			Vars = new AI_Var[VarInfos.Length];
			for (int i = 0; i < Vars.Length; i++) {
				Vars[i] = new AI_Var() {
					Index = i,
					Info = VarInfos[i],
					Name = Names[i],
				};
				Vars[i].Init();
			}

			// Editor var data
			VarEditorInfoBufferSize = s.Serialize<uint>(VarEditorInfoBufferSize, name: nameof(VarEditorInfoBufferSize));
			VarEditorInfoStringBufferSize = s.Serialize<int>(VarEditorInfoStringBufferSize, name: nameof(VarEditorInfoStringBufferSize));
			if (VarEditorInfoBufferSize > 0 && !Loader.IsBinaryData) {
				VarEditorInfos = s.SerializeObjectArray<AI_VarEditorInfo>(VarEditorInfos, VarEditorInfoBufferSize / 0x14, name: nameof(VarEditorInfos));
				for(int i = 0; i < VarEditorInfos.Length; i++) {
					var var = VarEditorInfos[i];
					s.Log($"Strings for {nameof(VarEditorInfos)}[{i}]");
					var.SerializeStrings(s);

					var match = Vars.FirstOrDefault(v => v.Info.Key == var.Key);
					if(match != null) match.EditorInfo = var;
				}
			}

			// Var values
			VarValueBufferSize = s.Serialize<uint>(VarValueBufferSize, name: nameof(VarValueBufferSize));
			var sortedVars = Vars.OrderBy(v => v.Info.Key).ToArray();
			if(Values == null) Values = new AI_VarValue[sortedVars.Length];
			for(int i = 0; i < Values.Length; i++) {
				var variable = sortedVars[i];
				Values[i] = s.SerializeObject<AI_VarValue>(Values[i], onPreSerialize: v => v.Var = variable, name: $"{nameof(Values)}[{i}]");
				
				variable.Value = Values[i];
			}

			if(Functions == null) Functions = new Jade_Reference<AI_Function>[5];
			for (int i = 0; i < Functions.Length; i++) {
				Functions[i] = s.SerializeObject<Jade_Reference<AI_Function>>(Functions[i], name: $"{nameof(Functions)}[{i}]");
				Functions[i].Resolve();
			}

			//PrintVarsOverview(s);
		}

		public void PrintVarsOverview(SerializerObject s) {
			s.Log($"VARS COUNT: {Vars.Length}");
			for (int i = 0; i < Vars.Length; i++) {
				s.Log($"Vars[{i}]: {Vars[i].Name}" +
					$"\n\t\tDescription: {Vars[i].EditorInfo?.Description?.Trim() ?? "null"}" +
					$"\n\t\tToggle text: {Vars[i].EditorInfo?.SelectionString?.Trim() ?? "null"}" +
					$"\n\t\tValue offset: {Vars[i].Value?.Offset}" +
					$"\n\t\tValue type: {Vars[i].Type} ({Vars[i].Link.Key})" +
					$"\n\t\tValue element size: {Vars[i].Link.Size}" +
					$"\n\t\tValue count: {Vars[i].Info.ArrayLength}" +
					$"\n\t\tValue dimensions count: {Vars[i].Info.ArrayDimensionsCount}");
			}

		}
	}
}
