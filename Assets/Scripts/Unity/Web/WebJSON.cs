using R1Engine;
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
		public Object Object { get; set; }
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
		public Collision[] Collision { get; set; }
	}
	public class Collision {
		public string Type { get; set; }

		// 3D
		public string AdditionalType { get; set; }
		public string Shape { get; set; }
	}
	public class Settings {
		public bool? ShowObjects { get; set; }
		public bool? ShowTiles { get; set; }
		public bool? ShowCollision { get; set; }
		public bool? ShowLinks { get; set; }
		public bool? ShowObjCollision { get; set; }
		public bool? AnimateSprites { get; set; }
		public bool? AnimateTiles { get; set; }
		public bool? ShowAlwaysObjects { get; set; }
		public bool? ShowDebugInfo { get; set; }
		public bool? ShowEditorObjects { get; set; }
		public bool? ShowGizmos { get; set; }
		public bool? ShowObjOffsets { get; set; }
        public bool? ShowRayman { get; set; }
		public bool? FreeCameraMode { get; set; }
		public bool? ShowGridMap { get; set; }
		public StateSwitchingMode? StateSwitchingMode { get; set; }

		public bool? CrashTimeTrialMode { get; set; }

		public bool? CanUseFreeCameraMode { get; set; }
		public bool? CanUseStateSwitchingMode { get; set; }
		public bool? CanUseCrashTimeTrialMode { get; set; }
		public bool? HasAnimatedTiles { get; set; }
		
		public Layer[] Layers { get; set; }
		public string[] Palettes { get; set; }
		public int? Palette { get; set; }
		public Layer[] ObjectLayers { get; set; }

		public Color? BackgroundTint { get; set; }
		public Color? BackgroundTintDark { get; set; }
	}
	public class Hierarchy {
		public Object Rayman { get; set; }
		public Object[] Objects { get; set; }
	}
	public class Object {
		// Common
        public string Name { get; set; }
        public string SecondaryName { get; set; }
		public int Index { get; set; } // Identify by index, non-nullable
		public bool? IsAlways { get; set; }
		public bool? IsEditor { get; set; }
		public int? AnimIndex { get; set; }
		public bool? IsEnabled { get; set; }
		public int? X { get; set; }
		public int? Y { get; set; }
		public string[] StateNames { get; set; }
		public int? StateIndex { get; set; }

        // Rayman 1/2
		public string[] R1_DESNames { get; set; }
		public string[] R2_AnimGroupNames { get; set; }
		public string[] R1_ETANames { get; set; } // Not in R2
		public ushort? R1_Type { get; set; }
		public int? R1_DESIndex { get; set; }
		public int? R2_AnimGroupIndex { get; set; }
		public string R2_MapLayer { get; set; }
		public int? R1_ETAIndex { get; set; } // Not in R2
		public byte? R1_Etat { get; set; }
	    public byte? R1_SubEtat { get; set; }
        public byte? R1_OffsetBX { get; set; }
        public byte? R1_OffsetBY { get; set; }
        public byte? R1_OffsetHY { get; set; }
        public byte? R1_FollowSprite { get; set; } // Not in R2
        public uint? R1_HitPoints { get; set; }
		public byte? R1_HitSprite { get; set; } // Not in R2
		public bool? R1_FollowEnabled { get; set; } // Not in R2
		public byte? R1_DisplayPrio { get; set; }
        public string[] R1_Commands { get; set; } // Not in R2

		// Jaguar
		public string[] R1Jaguar_EventDefinitionNames { get; set; }
		public int? R1Jaguar_EventDefinitionIndex { get; set; }
		public byte? R1Jaguar_ComplexState { get; set; }
		public byte? R1Jaguar_State { get; set; }

		// GBA
		public string[] GBA_ActorModelNames { get; set; }
		public byte? GBA_ActorID { get; set; }
		public int? GBA_ActorModelIndex { get; set; }
		public byte? GBA_Action { get; set; }

		// GBC
		public ushort? GBC_XlateID { get; set; }
		public string[] GBC_ActorModelNames { get; set; }
		public int? GBC_ActorModelIndex { get; set; }

		// Isometric: RHR & Spyro
		public Vector3? Position3D { get; set; }
		public int? GBAIsometric_AnimSetIndex { get; set; }
		public string[] GBAIsometric_AnimSetNames { get; set; }

		// RRR
		public int? GBARRR_AnimationGroupIndex { get; set; }
		public string[] GBARRR_AnimationGroupNames { get; set; }
		public int? GBARRR_GraphicsIndex { get; set; }
		public int? GBARRR_GraphicsKey { get; set; }

		// SNES
		public int? SNES_GraphicsGroupIndex { get; set; }
		public string[] SNES_GraphicsGroupNames { get; set; }

		// GBA Crash
		public int? GBAVV_AnimSetIndex { get; set; }
		public string[] GBAVV_AnimSetNames { get; set; }
	}
	public class Layer {
		public int Index { get; set; }
		public bool? IsVisible { get; set; }
	}
	public class Request {
		public RequestType Type { get; set; }

		// Optional
		public int? Index { get; set; }
		public Screenshot Screenshot { get; set; }
	}
	public class Localization {
		public Language[] Languages { get; set; }
		public class Language {
			public string Name { get; set; }
			public string[] Entries { get; set; }
		}
	}
	public class Screenshot {
		public ScreenshotType? Type { get; set; }
		public int? Width { get; set; }
		public int? Height { get; set; }
		public bool? IsTransparent { get; set; }
		public float? SizeFactor { get; set; }

		public enum ScreenshotType {
			Normal,
			FullLevel
		}
	}

	#region Enums
	public enum MessageType {
		Hierarchy,
		Settings,
		Highlight,
		Selection,
		Request,
		Commands,
		Awake
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
