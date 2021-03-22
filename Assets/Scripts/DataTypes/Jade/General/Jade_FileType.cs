﻿using System.Collections.Generic;

namespace R1Engine.Jade {
	public class Jade_FileType : R1Serializable {
		public string Extension { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Extension = s.SerializeString(Extension, 4, name: nameof(Extension));
		}

		public FileType Type {
			get {
				if (Types.ContainsKey(Extension)) {
					return Types[Extension];
				} else if (string.IsNullOrEmpty(Extension)) {
					return FileType.None;
				} else {
					return FileType.Unknown;
				}
			}
		}

		public static readonly Dictionary<string, FileType> Types = new Dictionary<string, FileType>() {
			[""] = FileType.None,
			[".gao"] = FileType.OBJ_GameObject,
			[".wow"] = FileType.WOR_World,
			[".oin"] = FileType.AI_Instance,
			[".omd"] = FileType.AI_Model,
			[".ova"] = FileType.AI_Vars,
			[".ofc"] = FileType.AI_Function,
			[".fce"] = FileType.AI_ProcList
		};

		public enum FileType {
			None,
			Unknown,
			OBJ_GameObject,
			WOR_World,
			AI_Instance,
			AI_Vars,
			AI_Model,
			AI_ProcList,
			AI_Function
		}
	}
}