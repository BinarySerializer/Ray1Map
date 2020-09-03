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
	}
	public class GameSettings {
		public MajorEngineVersion MajorEngineVersion { get; set; }
		public EngineVersion EngineVersion { get; set; }
		public Game Game { get; set; }
		public GameModeSelection Mode { get; set; }
	}
	public class Settings {
		public bool? AnimateSprites { get; set; }
		public bool? AnimateTiles { get; set; }
		public bool? ShowAlwaysEvents { get; set; }
		public bool? ShowDebugInfo { get; set; }
		public bool? ShowEditorEvents { get; set; }
		public bool? ShowDefaultObjIcons { get; set; }
        public bool? ShowRayman { get; set; }
		public StateSwitchingMode? StateSwitchingMode { get; set; }
	}
	public class Hierarchy {
		public Object[] Always { get; set; }
		public Object[] Objects { get; set; }
	}
	public class Object {
		public string Name { get; set; }
	}
	public class Request {
		public RequestType Type { get; set; }

		// Optional
		public Pointer Offset { get; set; }
		public Screenshot Screenshot { get; set; }
	}
	public class Localization {
		public Language Common { get; set; }
		public Language[] Languages { get; set; }
		public int CommonStart { get; set; }
		public int LanguageStart { get; set; }

		public class Language {
			public string Name { get; set; }
			public string NameLocalized { get; set; }
			public string[] Entries { get; set; }
		}
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
