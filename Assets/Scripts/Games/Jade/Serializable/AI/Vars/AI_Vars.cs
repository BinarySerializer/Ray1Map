using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Ray1Map.Jade {
    public class AI_Vars : Jade_File {
        public override bool HasHeaderBFFile => true;
        public override string Export_Extension => "ova";

        public uint RewindVarEndOffset { get; set; }
        public bool HasRewindZones { get; set; }
        public AI_Vars_RewindZone[] RewindZones { get; set; }

        public uint VarInfosBufferSize { get; set; }
        public AI_VarInfo[] VarInfos { get; set; }
        public uint NameBufferSize { get; set; }
        public AI_VarName[] Names { get; set; }
        public uint VarEditorInfoBufferSize { get; set; }
        public uint VarEditorInfosCount { get; set; }
        public int VarEditorInfoStringBufferSize { get; set; }
        public AI_VarEditorInfo[] VarEditorInfos { get; set; }
        public uint VarValueBufferSize { get; set; }
        public AI_VarValue[] Values { get; set; }

        public Jade_Reference<AI_Function>[] Functions { get; set; }
        public uint ExtraFunctionsCount { get; set; }
        public Jade_Reference<AI_Function>[] ExtraFunctions { get; set; }

        // Custom
        public AI_Var[] Vars { get; set; }

        protected override void SerializeFile(SerializerObject s) {
            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_T2T_20051002)) {
                    s.DoBits<uint>(b => {
                        RewindVarEndOffset = (uint)b.SerializeBits<int>((int)RewindVarEndOffset, 31, name: nameof(RewindVarEndOffset));
                        HasRewindZones = b.SerializeBits<int>(HasRewindZones ? 1 : 0, 1, name: nameof(HasRewindZones)) == 1;
                    });
                    if (HasRewindZones) RewindZones = s.SerializeObjectArray<AI_Vars_RewindZone>(RewindZones, RewindVarEndOffset, name: nameof(RewindZones));
                } else {
                    RewindVarEndOffset = s.Serialize<uint>(RewindVarEndOffset, name: nameof(RewindVarEndOffset));
                }
            }

            // Normal var data
            VarInfosBufferSize = s.Serialize<uint>(VarInfosBufferSize, name: nameof(VarInfosBufferSize));
            VarInfos = s.SerializeObjectArray<AI_VarInfo>(VarInfos, VarInfosBufferSize / 12, name: nameof(VarInfos));

            NameBufferSize = s.Serialize<uint>(NameBufferSize, name: nameof(NameBufferSize));
            if (NameBufferSize > 0) {
                Names = s.SerializeObjectArray<AI_VarName>(Names, VarInfos.Length, name: nameof(Names));
            }

            // Generate main Vars array
            Vars = new AI_Var[VarInfos.Length];
            for (int i = 0; i < Vars.Length; i++) {
                Vars[i] = new AI_Var() {
                    Index = i,
                    Info = VarInfos[i],
                    Name = Names?[i]?.Name,
                };
                Vars[i].Init();
            }

            // Editor var data
            if (!Loader.IsBinaryData) {
                VarEditorInfoBufferSize = s.Serialize<uint>(VarEditorInfoBufferSize, name: nameof(VarEditorInfoBufferSize));
                VarEditorInfoStringBufferSize = s.Serialize<int>(VarEditorInfoStringBufferSize, name: nameof(VarEditorInfoStringBufferSize));
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                    VarEditorInfosCount = VarEditorInfoBufferSize;
                } else {
                    VarEditorInfosCount = VarEditorInfoBufferSize / 0x14;
                }
                if (VarEditorInfoBufferSize > 0) {
                    VarEditorInfos = s.SerializeObjectArray<AI_VarEditorInfo>(VarEditorInfos, VarEditorInfosCount, name: nameof(VarEditorInfos));
                    for (int i = 0; i < VarEditorInfos.Length; i++) {
                        var var = VarEditorInfos[i];
                        s.Log($"Strings for {nameof(VarEditorInfos)}[{i}]");
                        var.SerializeStrings(s);

                        var match = Vars.FirstOrDefault(v => v.Info.BufferOffset == var.BufferOffset);
                        if (match != null) match.EditorInfo = var;
                    }
                }
            }

            // Var values
            VarValueBufferSize = s.Serialize<uint>(VarValueBufferSize, name: nameof(VarValueBufferSize));
            var sortedVars = Vars.OrderBy(v => v.Info.BufferOffset).ToArray();
            if(Values == null) Values = new AI_VarValue[sortedVars.Length];
            for(int i = 0; i < Values.Length; i++) {
                var variable = sortedVars[i];
                Values[i] = s.SerializeObject<AI_VarValue>(Values[i], onPreSerialize: v => v.Var = variable, name: $"{nameof(Values)}[{i}]");
                
                variable.Value = Values[i];
            }

            if(Functions == null) Functions = new Jade_Reference<AI_Function>[5];
            for (int i = 0; i < Functions.Length; i++) {
                Functions[i] = s.SerializeObject<Jade_Reference<AI_Function>>(Functions[i], name: $"{nameof(Functions)}[{i}]")?.Resolve();
            }

            if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
                if (s.CurrentPointer.AbsoluteOffset < (Offset + FileSize).AbsoluteOffset) {
                    ExtraFunctionsCount = s.Serialize<uint>(ExtraFunctionsCount, name: nameof(ExtraFunctionsCount));
                    ExtraFunctions = s.SerializeObjectArray<Jade_Reference<AI_Function>>(ExtraFunctions, ExtraFunctionsCount, name: nameof(ExtraFunctions));
                    foreach (var extraFunction in ExtraFunctions) {
                        extraFunction?.Resolve();
                    }
                }
            }
            /*ExportVarsOverview(null);
            ExportIDAStruct(null);*/
        }

        public string ExportVarsOverview() {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"VARS COUNT: {Vars.Length}");

            for (int i = 0; i < Vars.Length; i++) {
                b.AppendLine($"Vars[{i}]: {Vars[i].Name}" +
                    $"\n\t\tDescription: {Vars[i].EditorInfo?.Description?.Trim() ?? "null"}" +
                    $"\n\t\tToggle text: {Vars[i].EditorInfo?.SelectionString?.Trim() ?? "null"}" +
                    $"\n\t\tValue: {Vars[i].Value}" +
                    $"\n\t\tBuffer value offset: {Vars[i].Info.BufferOffset:X8}" +
                    $"\n\t\tBF Value offset: {Vars[i].Value?.Offset}" +
                    $"\n\t\tValue type: {Vars[i].Type} ({Vars[i].Link.Key})" +
                    $"\n\t\tValue element size: {Vars[i].Link.Size}" +
                    $"\n\t\tValue count: {Vars[i].Info.ArrayLength}" +
                    $"\n\t\tValue dimensions count: {Vars[i].Info.ArrayDimensionsCount}" +
                    $"\n\t\tVariable flags: {Vars[i].Info.Flags}" +
                    $"\n\t\tCopy to instance buffer: {(Vars[i].Info.Flags.HasFlag(AI_VarInfoFlags.Reinit))}");
            }
            return b.ToString();

        }

        public string ExportStruct(string name, bool save = false, ExportStructMode mode = ExportStructMode.IDA) 
        {
            StringBuilder b = new StringBuilder();
            StringBuilder serializeImplStr = mode == ExportStructMode.BinarySerializable ? new StringBuilder() : null;
            string indent_member = new string(' ', 4);
            string indent_method = new string(' ', 8);
            string structName = $"AI2C_Vars_{Key.Key:X8}";

            if (name != null) 
                structName += $"_{name}";

            if (mode == ExportStructMode.IDA)
                b.AppendLine($"struct {structName} {{");
            else if (mode == ExportStructMode.BinarySerializable) 
                b.AppendLine($"public class {structName} : {nameof(BinarySerializable)}{Environment.NewLine}{{");

            var varsOrderered = !save ? (IEnumerable<AI_Var>)Vars.OrderBy(v => v.Info.BufferOffset) : Vars;
            int curBufferOffset = 0;

            foreach (AI_Var v in varsOrderered) 
            {
                if (!save) 
                {
                    int newBufferOffset = v.Info.BufferOffset;

                    if (newBufferOffset < curBufferOffset) 
                        throw new Exception("Buffer offset was below the last one");

                    if (newBufferOffset > curBufferOffset) 
                    {
                        if (mode == ExportStructMode.IDA)
                            b.AppendLine($"\t_BYTE gap_{curBufferOffset:X8}[{newBufferOffset - curBufferOffset}];");
                        else if (mode == ExportStructMode.BinarySerializable)
                            serializeImplStr!.AppendLine($"s.SerializePadding({newBufferOffset - curBufferOffset});");

                        curBufferOffset = newBufferOffset;
                    }
                }

                if (!save ||
					v.Info.Flags.HasFlag(AI_VarInfoFlags.SaveAl) || // If this is 0, the values are read from the save file but not overwritten
					v.Info.Flags.HasFlag(AI_VarInfoFlags.Save)) 
                {
                    string varName = v.Name;
                    
                    if (v.Info.ArrayDimensionsCount != 0) 
                    {
                        if (mode == ExportStructMode.IDA)
                        {
                            b.AppendLine($"\tint {varName}__dimensions[{v.Info.ArrayDimensionsCount}];");
                        }
                        else if (mode == ExportStructMode.BinarySerializable)
                        {
                            string propName = $"{varName}__dimensions";
                            b.AppendLine($"{indent_member}public int[] {propName} {{ get; set; }}");
                            serializeImplStr!.AppendLine($"{indent_method}{propName} = s.SerializeArray<int>({propName}, {v.Info.ArrayDimensionsCount}, name: nameof({propName}));");
                        }

                        curBufferOffset += v.Info.ArrayDimensionsCount * 4;
                    }
                    
                    string arrayString = "";
                    
                    if (v.Info.ArrayDimensionsCount != 0) 
                        arrayString = $"[{v.Info.ArrayLength}]";
                    
                    string type = "int";
                    bool isValue = true;
                    
                    switch (v.Type) 
                    {
                        case AI_VarType.Float:
                            type = "float";
                            break;

                        case AI_VarType.Vector:
                            if (v.Link.Size == 12) {
                                type = "Vector";
                                isValue = false;
                            }
                            break;

                        case AI_VarType.Trigger:
                            type = "Var_Trigger";
                            isValue = false;
                            break;

                        case AI_VarType.Message:
                            type = "Var_Message";
                            isValue = false;
                            break;

                        case AI_VarType.Text:
                            type = "Var_Text";
                            isValue = false;
                            break;

                        case AI_VarType.Key:
                            type = "Var_Key";
                            isValue = false;
                            break;

                        case AI_VarType.PointerRef:
                            type = "Var_PointerRef";
                            isValue = false;
                            break;

                        case AI_VarType.MessageId:
                            type = "Var_MessageId";
                            isValue = false;
                            break;

                        case AI_VarType.GAO:
                            type = "Var_GAO";
                            isValue = false;
                            break;

                        case AI_VarType.String:
                            type = "Var_String";
                            isValue = false;
                            break;

						case AI_VarType.Function: // PointerRef
							type = "Var_Function";
							isValue = false;
							break;
						case AI_VarType.Model: // PointerRef
							type = "Var_Model";
							isValue = false;
							break;
						case AI_VarType.Network: // PointerRef
							type = "Var_Network";
							isValue = false;
							break;
					}

                    if (mode == ExportStructMode.IDA)
                    {
                        b.AppendLine($"\t{type} {varName}{arrayString};");
                    }
                    else if (mode == ExportStructMode.BinarySerializable)
                    {
                        bool isArray = v.Info.ArrayDimensionsCount != 0;
                        b.AppendLine($"{indent_member}public {type}{(isArray ? "[]" : "" )} {varName} {{ get; set; }}");
                        string objStr = isValue ? "" : "Object";

                        if (!isArray)
                            serializeImplStr!.AppendLine($"{indent_method}{varName} = s.Serialize{objStr}<{type}>({varName}, name: nameof({varName}));");
                        else
                            serializeImplStr!.AppendLine($"{indent_method}{varName} = s.Serialize{objStr}Array<{type}>({varName}, {v.Info.ArrayLength}, name: nameof({varName}));");
                    }

                    curBufferOffset += (int)(v.Info.ArrayLength * v.Link.Size);
                }
            }

            if (mode == ExportStructMode.IDA)
            {
                b.AppendLine($"}};");
            }
            else if (mode == ExportStructMode.BinarySerializable)
            {
                b.AppendLine($"{indent_member}public override void SerializeImpl(SerializerObject s)");
                b.AppendLine($"{indent_member}{{");
                b.Append(serializeImplStr);
                b.AppendLine($"{indent_member}}}");
                b.AppendLine($"}}");
            }

            return b.ToString();

        }

		public string ExportVarFile() {
			StringBuilder b = new StringBuilder();

            b.AppendLine("// Define at least one function in a track");
            for(int i = 0; i < Functions.Length; i++) {
                var func = Functions[i];
                var name = "name";
                if (func.IsNull) {
                    b.Append("//");
                } else {
                    name = func.Value.FunctionDef?.Name;
                }
                b.AppendLine($"function track{i} = \"{name}\"");
			}
            b.AppendLine();

            var vars = (IEnumerable<AI_Var>)Vars.OrderBy(v => v.Info.BufferOffset);

			foreach (AI_Var v in vars) {
                var type = v.Type switch {
                    AI_VarType.Bool => "bool",
                    AI_VarType.Int => "int",
					AI_VarType.Float => "float",
					AI_VarType.Vector => "vector",
					AI_VarType.String => "string",
                    AI_VarType.Function => "function",
					AI_VarType.GAO => "object",
					AI_VarType.Message => "message",
                    AI_VarType.Model => "model",
                    AI_VarType.Network => "network",
					AI_VarType.Text => "text",
					AI_VarType.Key => "key",
					AI_VarType.Color => "color",
					AI_VarType.Sound => "sound",
					AI_VarType.Byref => "byref",
					AI_VarType.Byrefarr => "byrefarr",
					AI_VarType.MessageId => "messageid",
					AI_VarType.Trigger => "trigger",

					AI_VarType.Void => "void",
					AI_VarType.Every => "every",
					AI_VarType.Hexa => "hexa",
					AI_VarType.Binary => "binary",
					AI_VarType.Private => "private",
					AI_VarType.Separator => "separator",
					AI_VarType.Enum => "enum",
					AI_VarType.Save => "save",
					AI_VarType.Reinit => "reinit",
					AI_VarType.Saveal => "saveal",
					AI_VarType.Optim => "optim",

					_ => "unknown"
                };
                var flags = v.Info.Flags;

                if (flags.HasFlag(AI_VarInfoFlags.Pointer)) {
					type = $"pointer {type}";
				}
                if (flags.HasFlag(AI_VarInfoFlags.SaveAl)) {
                    type = $"saveal {type}";
                } else if (flags.HasFlag(AI_VarInfoFlags.Save)) {
                    type = $"save {type}";
                }
				if (flags.HasFlag(AI_VarInfoFlags.Reinit)) {
					type = $"reinit {type}";
				}
				if (flags.HasFlag(AI_VarInfoFlags.Private)) {
                    type = $"private {type}";
                }
                var tabs = 8;
                tabs -= type.Length / 4;
                if(tabs <= 0) tabs = 1;
                b.Append(type);
                for(int i = 0; i < tabs; i++) b.Append('\t');
				b.Append(v.Name);

                bool usedVraiFauxEnum = false;

                if (v.Info.ArrayDimensionsCount != 0) {
                    for (int i = 0; i < v.Info.ArrayDimensionsCount; i++) {
                        b.Append('[');
                        b.Append(v.Value.Dimensions[i]);
                        b.Append(']');
                    }
                    // TODO: array values are like this = (-1, -1, -1, -1)
                } else {
                    switch (v.Type) {
                        case AI_VarType.Bool:
                            if(v.Value.ValueBool)
                                b.Append(" = (1==1)");
                            break;
                        case AI_VarType.Int:
                            if (v.Name.StartsWith("b_") && (v.Value.ValueInt == 0 || v.Value.ValueInt == 1)) {
                                usedVraiFauxEnum = true;
                                if (v.Value.ValueInt == 1)
                                    b.Append($" = faux_vrai");
                                else
                                    b.Append($" = vrai_faux");
                            } else {
                                if (v.Value.ValueInt != 0)
                                    b.Append($" = {v.Value.ValueInt}");
                            }
                            break;
						case AI_VarType.Float:
							if (v.Value.ValueFloat != 0)
								b.Append($" = {v.Value.ValueFloat}");
                            break;
						case AI_VarType.Vector:
                            var vec = v.Value.ValueVector;
							if (vec != Jade_Vector.Zero)
								b.Append($" = cvector({vec.X}, {vec.Y}, {vec.Z})");
                            else
                                b.Append(" = Cv_NullVector");
							break;
                        case AI_VarType.Color:
                            if (v.Value.ValueInt != 0) {
								b.Append($" = 0x{v.Value.ValueUInt:X8}");
							}
                            break;
					}
                }
                if (flags.HasFlag(AI_VarInfoFlags.Enum) && !usedVraiFauxEnum) {
					b.Append(" // Enum");
				}
                b.AppendLine();
			}

			return b.ToString();
		}
		public string ExportForUnbinarizeImport() {
			StringBuilder b = new StringBuilder();

            foreach (AI_Var v in Vars) {
                b.Append(v.Name);
                b.Append(",");
                b.Append(v.Type.ToString());
                b.Append(",");
				b.Append(((uint)(v.Info.Flags)).ToString("X4"));
				b.Append(",");
				b.Append(v.Info.ArrayDimensionsCount);
                b.Append(",");
                b.Append(v.Info.ArrayLength);
                b.AppendLine();
            }

            return b.ToString();
		}


		public enum ExportStructMode
        {
            IDA,
            BinarySerializable,
        }
    }
}
