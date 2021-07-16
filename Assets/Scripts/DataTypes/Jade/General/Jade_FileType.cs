using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine.Jade {
	public class Jade_FileType : BinarySerializable {
		public string Extension { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Extension = s.SerializeString(Extension, 4, name: nameof(Extension));
		}

		public FileType Type {
			get {
				var lowerExtension = Extension.ToLower();
				if (Types.ContainsKey(lowerExtension)) {
					return Types[lowerExtension];
				} else if (string.IsNullOrEmpty(lowerExtension)) {
					return FileType.None;
				} else {
					return FileType.Unknown;
				}
			}
		}

		public static readonly Dictionary<string, FileType> Types = new Dictionary<string, FileType>() {
			[""] = FileType.None,
			[".gao"] = FileType.OBJ_GameObject,
			[".wol"] = FileType.WOR_WorldList,
			[".wow"] = FileType.WOR_World,
			[".gri"] = FileType.GRID_WorldGrid,

			// AI
			[".oin"] = FileType.AI_Instance,
			[".omd"] = FileType.AI_Model,
			[".ova"] = FileType.AI_Vars,
			[".ofc"] = FileType.AI_Function,
			[".fce"] = FileType.AI_ProcList,
			[".ttt"] = FileType.AI_TT,

			// Sound
			[".snk"] = FileType.SND_Metabank,
			[".smd"] = FileType.SND_SModifier,
			[".wav"] = FileType.SND_Sound,
			[".wac"] = FileType.SND_LoadingSound,
			[".wad"] = FileType.SND_Dialog,
			[".wam"] = FileType.SND_Music,
			[".waa"] = FileType.SND_Ambience,

			// Texture
			[".pal"] = FileType.TEX_Palette,

			// Text
			[".txt"] = FileType.TEXT_AllText,
			[".txg"] = FileType.TEXT_TextGroup,
			[".txl"] = FileType.TEXT_TextList,
		};

		public enum FileType {
			None,
			Unknown,
			OBJ_GameObject,
			WOR_WorldList,
			WOR_World,
			AI_Instance,
			AI_Vars,
			AI_Model,
			AI_ProcList,
			AI_TT,
			AI_Function,
			SND_Metabank,
			SND_Sound,
			SND_LoadingSound,
			SND_Dialog,
			SND_Music,
			SND_Ambience,
			SND_SModifier,
			GRID_WorldGrid,

			TEX_File,
			TEX_Palette,

			TEXT_AllText,
			TEXT_TextGroup,
			TEXT_TextList,
		}
	}
}
