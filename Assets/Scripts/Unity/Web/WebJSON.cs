using Newtonsoft.Json;
using R1Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WebJSON {
	public class Message {
		// Mandatory
		public MessageType Type { get; set; }

		// Optional
		public Settings Settings { get; set; }
		public Hierarchy Hierarchy { get; set; }
		public Localization Localization { get; set; }
		public Request Request { get; set; }
		public GameSettings GameSettings { get; set; }
		public Highlight Highlight { get; set; }
		public Selection Selection { get; set; }
	}
	public class GameSettings {
		public MajorEngineVersion MajorEngineVersion { get; set; }
		public EngineVersion EngineVersion { get; set; }
		public Game Game { get; set; }
		public GameModeSelection Mode { get; set; }
	}
	public class Selection {
		public Object Object { get; set; }
	}
	public class Highlight {
		public Object Object { get; set; }
		public Collision Collision { get; set; }
	}
	public class Collision {
		public string Type { get; set; }
	}
	public class Settings {
		public bool? ShowObjects { get; set; }
		public bool? ShowTiles { get; set; }
		public bool? ShowCollision { get; set; }
		public bool? AnimateSprites { get; set; }
		public bool? AnimateTiles { get; set; }
		public bool? ShowAlwaysEvents { get; set; }
		public bool? ShowDebugInfo { get; set; }
		public bool? ShowEditorEvents { get; set; }
		public bool? ShowDefaultObjIcons { get; set; }
		public bool? ShowObjOffsets { get; set; }
        public bool? ShowRayman { get; set; }
		public StateSwitchingMode? StateSwitchingMode { get; set; }
	}
	public class Hierarchy {
		public Object[] Objects { get; set; }
	}
	public class Object {
		public string Name { get; set; }
		public int Index { get; set; } // Identify by index, non-nullable
		public bool? IsAlways { get; set; }
		public bool? IsEditor { get; set; }
		public int? X { get; set; }
		public int? Y { get; set; }
		public string[] Commands { get; set; }
	}
	public class Request {
		public RequestType Type { get; set; }

		// Optional
		public int? Index { get; set; }
		public Screenshot Screenshot { get; set; }
	}
	public class Localization {
		// TODO
	}
	public class Screenshot {
		public int? Width { get; set; }
		public int? Height { get; set; }
		public bool? IsTransparent { get; set; }
		public float? SizeFactor { get; set; }
	}

	#region Enums
	public enum MessageType {
		Hierarchy,
		Settings,
		Highlight,
		Selection,
		Request,
		Commands
	}
	public enum ObjectType {
		Instance,
		Always,
	}
	public enum RequestType {
		None,
		Commands,
		Screenshot,
	}
	#endregion
}
