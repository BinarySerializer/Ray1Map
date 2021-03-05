using Cysharp.Threading.Tasks;
using R1Engine;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebCommunicator : MonoBehaviour {
    [DllImport("__Internal")]
    private static extern void SetAllJSON(string jsonString);
    [DllImport("__Internal")]
    private static extern void UnityJSMessage(string jsonString);
	[DllImport("__Internal")]
	private static extern void SaveFile(byte[] array, int size, string filename);

	public Controller controller;
    bool sentHierarchy = false;
    string allJSON = null;
	public bool debugMessages = false;

	Unity_Tile[] highlightedCollision_;
	Unity_IsometricCollisionTile highlightedCollision3D_;
	Unity_CollisionLine highlightedCollisionLine_;
	Unity_ObjBehaviour highlightedObject_;
	Unity_ObjBehaviour selectedObject_;
	int x_, y_;
	Vector3 pos3D_;

	private Newtonsoft.Json.JsonSerializerSettings _jsonSettings;
	public Newtonsoft.Json.JsonSerializerSettings JsonSettings {
		get {
			if (_jsonSettings == null) {
				_jsonSettings = new Newtonsoft.Json.JsonSerializerSettings() {
					Formatting = Newtonsoft.Json.Formatting.None,
					NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
					MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
					
				};
				_jsonSettings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.Vector3Converter());
				_jsonSettings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.Vector2Converter());
				_jsonSettings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.Vector4Converter());
				_jsonSettings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.QuaternionConverter());
				_jsonSettings.Converters.Add(new Newtonsoft.Json.UnityConverters.Math.ColorConverter());
			}
			return _jsonSettings;
		}
	}

	public void Start() {
		Send(new WebJSON.Message() { Type = WebJSON.MessageType.Awake }, allowIfUnloaded: true);
    }

    public void Update() {
        if (Controller.LoadState == Controller.State.Finished && !sentHierarchy) {
            SendHierarchy();
            sentHierarchy = true;
		}
        if ((Application.platform == RuntimePlatform.WebGLPlayer || debugMessages) && Controller.LoadState == Controller.State.Finished) {
			// TODO: Handle highlight & selection changes like in raymap:
			var hl = Controller.obj.levelController.editor.objectHighlight;

			var hlCollision3D = hl.highlightedCollision3D;
			var hlCollision = hl.highlightedCollision;
			var hlCollisionLine = hl.highlightedCollisionLine;
			if (!Settings.ShowCollision) {
				hlCollisionLine = null;
				hlCollision3D = null;
				hlCollision = null;
			}
			if (highlightedObject_ != hl.highlightedObject ||
				(highlightedCollision3D_ != hlCollision3D ||
				highlightedCollisionLine_ != hlCollisionLine ||
				(highlightedCollision_ != hlCollision &&
				(highlightedCollision_ == null || hlCollision == null ||
				!highlightedCollision_.SequenceEqual(hlCollision))))) {
				highlightedObject_ = hl.highlightedObject;
				highlightedCollision_ = hlCollision;
				highlightedCollision3D_ = hlCollision3D;
				highlightedCollisionLine_ = hlCollisionLine;
				Send(GetHighlightMessageJSON());
			}

			// Check selected object
			if (selectedObject_ != Controller.obj.levelEventController.SelectedEvent) {
				selectedObject_ = Controller.obj.levelEventController.SelectedEvent;
				if (selectedObject_ != null) {
					x_ = selectedObject_.ObjData.XPosition;
					y_ = selectedObject_.ObjData.YPosition;
					if (selectedObject_.ObjData is Unity_Object_3D o3d && LevelEditorData.Level?.IsometricData != null) {
						pos3D_ = o3d.Position;
					}
					// TODO: keep state indices so updates on animation speed, etc. can be sent
					//selectedPersoStateIndex_ = selectedPerso_.currentState;
				}
				Send(GetSelectionMessageJSON(includeLists: true, includeDetails: true));
			}

			// Check selected object's changed values
			if (selectedObject_ != null) {
				if (selectedObject_.ObjData.XPosition != x_ || selectedObject_.ObjData.YPosition != y_ ||
					(selectedObject_.ObjData is Unity_Object_3D o3d && LevelEditorData.Level?.IsometricData != null && o3d.Position != pos3D_)) {
					x_ = selectedObject_.ObjData.XPosition;
					y_ = selectedObject_.ObjData.YPosition;
					if (selectedObject_.ObjData is Unity_Object_3D o3d2 && LevelEditorData.Level?.IsometricData != null) {
						pos3D_ = o3d2.Position;
					}
					Send(GetSelectionMessageJSON(includeLists: false, includeDetails: false));
				}
			}
		}
    }

    public void SendHierarchy() {
        if (Application.platform == RuntimePlatform.WebGLPlayer && Controller.LoadState == Controller.State.Finished) {
			allJSON = SerializeMessage(GetHierarchyMessageJSON());
            SetAllJSON(allJSON);
        }
    }
	public void SendSettings() {
		if (Application.platform == RuntimePlatform.WebGLPlayer && Controller.LoadState == Controller.State.Finished) {
			Send(GetSettingsMessageJSON());
		}
	}
    public void Send(WebJSON.Message obj, bool allowIfUnloaded = false) {
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			if (allowIfUnloaded || Controller.LoadState == Controller.State.Finished) {
				string json = SerializeMessage(obj);
				UnityJSMessage(json);
			}
		} else if(debugMessages) {
			if (allowIfUnloaded || Controller.LoadState == Controller.State.Finished) {
				string json = SerializeMessage(obj);
				print(json);
			}
		}
    }
	public string SerializeMessage(WebJSON.Message obj) {
		string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, JsonSettings);
		return json;
	}

	private WebJSON.Message GetHierarchyMessageJSON() {
		WebJSON.Message message = new WebJSON.Message() {
			Type = WebJSON.MessageType.Hierarchy,
			Settings = GetSettingsJSON(),
			Localization = GetLocalizationJSON(),
			Hierarchy = GetHierarchyJSON(),
			GameSettings = GetGameSettingsJSON()
		};
		return message;
    }
	private WebJSON.Hierarchy GetHierarchyJSON() {
		var h = new WebJSON.Hierarchy();
		var objects = Controller.obj.levelController.Objects;
		h.Rayman = GetObjectJSON(Controller.obj.levelController.RaymanObject);
		h.Objects = objects.Select(o => GetObjectJSON(o)).ToArray();
		return h;
	}
	private WebJSON.Message GetSelectionMessageJSON(bool includeLists = false, bool includeDetails = true) {
		WebJSON.Message selectionJSON = new WebJSON.Message() {
			Type = WebJSON.MessageType.Selection,
			Selection = new WebJSON.Selection() {
				Object = GetObjectJSON(selectedObject_, includeLists: includeLists, includeDetails: includeDetails)
			}
		};
		return selectionJSON;
	}
	private WebJSON.Message GetHighlightMessageJSON() {
		WebJSON.Message selectionJSON = new WebJSON.Message() {
			Type = WebJSON.MessageType.Highlight,
			Highlight = new WebJSON.Highlight() {
				Object = GetObjectJSON(highlightedObject_)
			}
		};
		// Collision
		if (highlightedCollision3D_ != null) {
			selectionJSON.Highlight.Collision = new WebJSON.Collision[1] {
				new WebJSON.Collision() {
					Type = highlightedCollision3D_.Type.ToString(),
					Shape = highlightedCollision3D_.Shape != Unity_IsometricCollisionTile.ShapeType.None ? highlightedCollision3D_.Shape.ToString() : null,
					AdditionalType = highlightedCollision3D_.AddType != Unity_IsometricCollisionTile.AdditionalTypeFlags.None ? highlightedCollision3D_.AddType.ToString() : null
				}
			};
		} else if(highlightedCollisionLine_ != null) {
			selectionJSON.Highlight.Collision = new WebJSON.Collision[1] {
				// TODO: Replace with highlighted line properties
				new WebJSON.Collision() {
					Type = highlightedCollisionLine_.TypeName,
				}
			};
		} else if (highlightedCollision_ != null && highlightedCollision_.Length > 0) {
			selectionJSON.Highlight.Collision = highlightedCollision_
				.Where(c => c != null)
				.Select(c => new WebJSON.Collision() {
					Type = LevelEditorData.Level.GetCollisionTypeNameFunc(c.Data?.CollisionType ?? 0),
					Shape = c.Data?.GBAVV_CollisionShape?.ToString()
				})
				.Where(c => c.Type != "Empty" && c.Type != "None" && !(c.Type == "Solid" && c.Shape == "None")) // Filter out empty types
				.ToArray();
			if(selectionJSON.Highlight.Collision?.Length == 0)
				selectionJSON.Highlight.Collision = null;
		}

		return selectionJSON;
	}
	private WebJSON.Object GetObjectJSON(Unity_ObjBehaviour obj, bool includeLists = false, bool includeDetails = false) {
		if (obj == null) return null;
		var webObj = new WebJSON.Object() {
			Name = obj.ObjData.PrimaryName,
			SecondaryName = obj.ObjData.SecondaryName,
			Index = obj.Index,
			IsAlways = obj.ObjData.IsAlways,
			IsEnabled = obj.IsEnabled,
			IsEditor = obj.ObjData.IsEditor,
			// Updateable fields
			X = obj.ObjData.XPosition,
			Y = obj.ObjData.YPosition
		};
		if (obj.ObjData is Unity_Object_3D o3d && LevelEditorData.Level?.IsometricData != null) {
			webObj.Position3D = o3d.Position;
		}

		if (includeDetails) {
			// Common details
			webObj.AnimIndex = obj.ObjData.AnimationIndex;

			if (includeLists) {
				webObj.StateNames = obj.ObjData.UIStateNames;
			}
			webObj.StateIndex = obj.ObjData.CurrentUIState;

            // Specific properties for type
			switch (obj.ObjData) {
				case Unity_Object_R1 r1obj:
					webObj.R1_Type = (ushort)r1obj.EventData.Type;
					webObj.R1_DESIndex = r1obj.DESIndex;
					webObj.R1_ETAIndex = r1obj.ETAIndex;
					webObj.R1_Etat = r1obj.EventData.Etat;
					webObj.R1_SubEtat = r1obj.EventData.SubEtat;
					webObj.R1_OffsetBX = r1obj.EventData.OffsetBX;
					webObj.R1_OffsetBY = r1obj.EventData.OffsetBY;
					webObj.R1_OffsetHY = r1obj.EventData.OffsetHY;
					webObj.R1_FollowSprite = r1obj.EventData.FollowSprite;
					webObj.R1_HitPoints = r1obj.EventData.ActualHitPoints;
					webObj.R1_HitSprite = r1obj.EventData.HitSprite;
					webObj.R1_FollowEnabled = r1obj.EventData.GetFollowEnabled(LevelEditorData.CurrentSettings);
					webObj.R1_DisplayPrio = r1obj.EventData.DisplayPrio;

					if (includeLists) {
						webObj.R1_Commands = r1obj.EventData.Commands?.ToTranslatedStrings(r1obj.EventData.LabelOffsets);
						webObj.R1_DESNames = r1obj.ObjManager.DES.Select(x => x.DisplayName).ToArray();
						webObj.R1_ETANames = r1obj.ObjManager.ETA.Select(x => x.DisplayName).ToArray();
					}
					break;

				case Unity_Object_R2 r2obj:
					webObj.R1_Type = (ushort)r2obj.EventData.EventType;
					webObj.R2_MapLayer = r2obj.EventData.MapLayer.ToString();
					webObj.R1_Etat = r2obj.EventData.Etat;
					webObj.R1_SubEtat = r2obj.EventData.SubEtat;
					webObj.R1_OffsetBX = r2obj.EventData.CollisionData?.OffsetBX;
					webObj.R1_OffsetBY = r2obj.EventData.CollisionData?.OffsetBY;
					webObj.R1_OffsetHY = r2obj.EventData.CollisionData?.OffsetHY;
					webObj.R1_HitPoints = r2obj.EventData.HitPoints;
					webObj.R1_DisplayPrio = r2obj.EventData.DisplayPrio;

					if (r2obj.AnimGroupIndex != -1)
                    {
                        webObj.R2_AnimGroupIndex = r2obj.AnimGroupIndex;
                        if (includeLists)
                            webObj.R2_AnimGroupNames = r2obj.ObjManager.AnimGroups.Select(x => x.Pointer?.ToString() ?? "N/A").ToArray();
                    }
					break;

				case Unity_Object_R1Jaguar r1jaguarObj:
					webObj.R1Jaguar_EventDefinitionIndex = r1jaguarObj.EventDefinitionIndex;
					webObj.R1Jaguar_ComplexState = r1jaguarObj.ComplexStateIndex;
					webObj.R1Jaguar_State = r1jaguarObj.StateIndex;

					if (includeLists)
						webObj.R1Jaguar_EventDefinitionNames = r1jaguarObj.ObjManager.EventDefinitions.Select(x => x.DisplayName).ToArray();
					break;

				case Unity_Object_GBA gbaObj:
					webObj.GBA_ActorID = gbaObj.Actor.ActorID;
					webObj.GBA_Action = gbaObj.Actor.ActionIndex;

					if (gbaObj.ActorModelIndex != -1)
                    {
                        webObj.GBA_ActorModelIndex = gbaObj.ActorModelIndex;

                        if (includeLists)
                            webObj.GBA_ActorModelNames = gbaObj.ObjManager.ActorModels.Select(x => x.DisplayName).ToArray();
                    }
					break;
				case Unity_Object_GBAIsometricRHR rhrObj:
					webObj.GBAIsometric_AnimSetIndex = rhrObj.AnimSetIndex;
					if(includeLists)
						webObj.GBAIsometric_AnimSetNames = rhrObj.ObjManager.AnimSets.Select(x => x.Name).ToArray();
					break;
				case Unity_Object_GBAIsometricSpyro spyroObj:
					if (spyroObj.AnimSetIndex != -1)
                    {
                        webObj.GBAIsometric_AnimSetIndex = spyroObj.AnimSetIndex;
                        if (includeLists)
                            webObj.GBAIsometric_AnimSetNames = spyroObj.ObjManager.AnimSets.Select((x, i) => i.ToString()).ToArray();
                    }
					break;
				case Unity_Object_GBAIsometricSpyro2_2D spyro2DObj:
					if (spyro2DObj.AnimSetIndex != -1)
                    {
                        webObj.GBAIsometric_AnimSetIndex = spyro2DObj.AnimSetIndex;
                        if (includeLists)
                            webObj.GBAIsometric_AnimSetNames = spyro2DObj.ObjManager.AnimSets.Select((x, i) => i.ToString()).ToArray();
                    }
					break;
				case Unity_Object_GBC gbcObj:
					webObj.GBC_XlateID = gbcObj.Actor.XlateID;

					if (gbcObj.ActorModelIndex != -1)
                    {
                        webObj.GBC_ActorModelIndex = gbcObj.ActorModelIndex;
                        if (includeLists)
                            webObj.GBC_ActorModelNames = gbcObj.ObjManager.ActorModels.Select(x => x.Index.ToString()).ToArray();
                    }
					break;
				case Unity_Object_GBARRR rrrObj:
					webObj.GBARRR_GraphicsIndex = (int)rrrObj.Object.P_GraphicsIndex;
					webObj.GBARRR_GraphicsKey = rrrObj.Object.Ushort_0C;
					if (rrrObj.AnimationGroupIndex != -1)
                    {
                        webObj.GBARRR_AnimationGroupIndex = rrrObj.AnimationGroupIndex;
                        if (includeLists)
                            webObj.GBARRR_AnimationGroupNames = rrrObj.ObjManager.GraphicsDatas.Select((g, i) => i.ToString()).ToArray();
                    }
					break;
				case Unity_Object_GBARRRMode7Unused rrrMode7UnusedObj:
                    webObj.GBARRR_AnimationGroupIndex = rrrMode7UnusedObj.AnimationGroupIndex;
                    if (includeLists)
                        webObj.GBARRR_AnimationGroupNames = rrrMode7UnusedObj.ObjManager.GraphicsDatas.Select((g, i) => g.BlockIndex.ToString()).ToArray();
					break;

				case Unity_Object_SNES snesObj:
					webObj.SNES_GraphicsGroupIndex = snesObj.GraphicsGroupIndex;
					if(includeLists)
						webObj.SNES_GraphicsGroupNames = snesObj.ObjManager.GraphicsGroups.Select((g, i) => g.Name).ToArray();
					break;

				case Unity_Object_GBAVV crashObj:
					if (crashObj.ObjParams?.Any() == true)
					    webObj.GBAVV_ObjParams = Util.ByteArrayToHexString(crashObj.ObjParams);

					if (crashObj.AnimSetIndex != -1)
                    {
                        webObj.GBAVV_AnimSetIndex = crashObj.AnimSetIndex;
                        if (includeLists)
                            webObj.GBAVV_AnimSetNames = crashObj.ObjManager.AnimSets.SelectMany((graphics, graphicsIndex) => graphics.Select((animSet, animSetIndex) => crashObj.ObjManager.MultipleAnimSetArrays ? $"{graphicsIndex}-{animSet.GetDisplayName(animSetIndex)}" : $"{animSet.GetDisplayName(animSetIndex)}")).ToArray();
                    }

                    if (includeLists)
                        webObj.R1_Commands = crashObj.GetTranslatedScript;
                    break;

				case Unity_Object_GBAVVMode7 crashObj:
					if (crashObj.AnimSetIndex != -1)
                    {
                        webObj.GBAVV_AnimSetIndex = crashObj.AnimSetIndex;
                        if (includeLists)
                            webObj.GBAVV_AnimSetNames = crashObj.ObjManager.AnimSets.Select((x, i) => i.ToString()).ToArray();
                    }
					break;

                case Unity_Object_GBAVVNitroKart crashObj:
                    if (crashObj.Object.ParamsPointer != null)
                        webObj.GBAVV_ObjParams = $"0x{crashObj.Object.ParamsPointer.AbsoluteOffset:X8}";
					else if (crashObj.Object.NGage_Params != null)
                        webObj.GBAVV_ObjParams = Util.ByteArrayToHexString(crashObj.Object.NGage_Params);

                    if (crashObj.AnimSetIndex != -1)
                    {
                        webObj.GBAVV_AnimSetIndex = crashObj.AnimSetIndex;
                        if (includeLists)
                            webObj.GBAVV_AnimSetNames = crashObj.ObjManager.AnimSets.SelectMany((graphics, graphicsIndex) => graphics.Select((animSet, animSetIndex) => crashObj.ObjManager.MultipleAnimSetArrays ? $"{graphicsIndex}-{animSetIndex}" : $"{animSetIndex}")).ToArray();
                    }
                    
                    break;

				case Unity_Object_GameloftRRR glRRRObj:
					if (glRRRObj.PuppetIndex != -1) {
						webObj.Gameloft_PuppetIndex = glRRRObj.PuppetIndex;
						if(includeLists)
							webObj.Gameloft_PuppetNames = glRRRObj.ObjManager.Puppets.Select((x,i) => x.DisplayName).ToArray();
					}
					webObj.GameloftRRR_ObjectID = glRRRObj.Object.ObjectID;
					if (glRRRObj?.Object?.Shorts != null) {
						webObj.GameloftRRR_ObjectParams = string.Join(",", glRRRObj.Object.Shorts);
					}
					break;
            }
		}
		return webObj;
	}
	public WebJSON.GameSettings GetGameSettingsJSON() {
		Context c = LevelEditorData.MainContext;
		GameSettings g = c.Settings;
		return new WebJSON.GameSettings() {
			MajorEngineVersion = g.MajorEngineVersion,
			EngineVersion = g.EngineVersion,
			Game = g.Game,
			Mode = g.GameModeSelection
		};
	}
	public WebJSON.Localization GetLocalizationJSON() {
		var lvl = LevelEditorData.Level;
		if (lvl?.Localization != null) {
			var loc = lvl.Localization;
			return new WebJSON.Localization() {
				Languages = loc.Select(kv => new WebJSON.Localization.Language() {
						Name = kv.Key,
						Entries = kv.Value
				}).ToArray()
			};
		}
		return null;
	}
	private WebJSON.Message GetSettingsMessageJSON() {
		WebJSON.Message message = new WebJSON.Message() {
			Type = WebJSON.MessageType.Settings,
			Settings = GetSettingsJSON()
		};
		return message;
    }

	private WebJSON.Settings GetSettingsJSON() {
		WebJSON.Settings s = new WebJSON.Settings() {
			ShowObjects = Settings.ShowObjects,
			ShowTiles = Settings.ShowTiles,
			ShowCollision = Settings.ShowCollision,
			ShowLinks = Settings.ShowLinks,
            ShowObjCollision = Settings.ShowObjCollision,
			AnimateSprites = Settings.AnimateSprites,
			AnimateTiles = Settings.AnimateTiles,
			ShowAlwaysObjects = Settings.ShowAlwaysObjects,
			ShowEditorObjects = Settings.ShowEditorObjects,
			ShowDebugInfo = Settings.ShowDebugInfo,
			ShowGizmos = Settings.ShowDefaultObjIcons,
			ShowObjOffsets = Settings.ShowObjOffsets,
			ShowRayman = Settings.ShowRayman,
			StateSwitchingMode = Settings.StateSwitchingMode,
			ShowGridMap = Settings.ShowGridMap,
			CrashTimeTrialMode = Settings.GBAVV_Crash_TimeTrialMode
		};

		// Add layers
		var c = LevelEditorData.MainContext;
		s.CanUseStateSwitchingMode = (c?.Settings?.MajorEngineVersion == MajorEngineVersion.Rayman1) == true;
		s.CanUseCrashTimeTrialMode = (c?.Settings?.EngineVersion == EngineVersion.GBAVV_Crash1 || c?.Settings?.EngineVersion == EngineVersion.GBAVV_Crash2) == true;
		var lvl = LevelEditorData.Level;
		if (Controller.obj?.levelController?.controllerTilemap != null) {
			var tc = Controller.obj?.levelController?.controllerTilemap;
			s.HasAnimatedTiles = tc.HasAnimatedTiles;
		}
		if (lvl != null) {
			s.CanUseFreeCameraMode = lvl.IsometricData != null;
			List<WebJSON.Layer> layers = new List<WebJSON.Layer>();
			if (lvl.Background != null && Controller.obj?.levelController?.controllerTilemap?.background != null) {
				layers.Add(new WebJSON.Layer() {
					Index = LevelTilemapController.Index_Background,
					IsVisible = Controller.obj.levelController.controllerTilemap.background.gameObject.activeSelf
				});
			}

			if (lvl.ParallaxBackground != null && Controller.obj?.levelController?.controllerTilemap?.backgroundParallax != null) {
				layers.Add(new WebJSON.Layer() {
					Index = LevelTilemapController.Index_ParallaxBackground,
					IsVisible = Controller.obj.levelController.controllerTilemap.backgroundParallax.gameObject.activeSelf
				});
			}

			var visibility = Controller.obj?.levelController?.controllerTilemap?.IsLayerVisible;
			if (visibility != null) {
				for (int i = 0; i < visibility.Length; i++) 
                {
                    /*if (!LevelEditorData.Level.Maps[i].Type.HasFlag(Unity_Map.MapType.Graphics))
                        continue;*/
					layers.Add(new WebJSON.Layer() {
						Index = i,
						IsVisible = visibility[i]
					});
				}
			}
			if (layers.Count > 0) {
				s.Layers = layers.ToArray();
			}
			if (c?.Settings.EngineVersion == EngineVersion.R2_PS1) {
				var objLayers = Controller.obj.levelController.Objects
					.Select(o => o.ObjData.MapLayer).Distinct()
					.Where(l => l.HasValue && (LevelEditorData.ShowEventsForMaps?.Length ?? 0) > l.Value);
				s.ObjectLayers = objLayers.Select(l => new WebJSON.Layer() {
					Index = l.Value,
					IsVisible = LevelEditorData.ShowEventsForMaps[l.Value]
					}).OrderBy(ol => ol.Index).ToArray();
			}

            if (Controller.obj.levelController.controllerTilemap.HasAutoPaletteOption)
            {
                s.Palettes = new string[] { "Auto" }
                    .Concat(Enumerable.Range(0, lvl.Maps.Max(x => x.TileSet.Length)).Select(x => x.ToString())).ToArray();

                if (Controller.obj?.levelController?.controllerTilemap != null)
                    s.Palette = Controller.obj.levelController.controllerTilemap.currentPalette;
            }
			else
            {
                s.Palettes = Enumerable.Range(0, lvl.Maps.Max(x => x.TileSet.Length)).Select(x => x.ToString()).ToArray();

                if (Controller.obj?.levelController?.controllerTilemap != null)
                    s.Palette = Controller.obj.levelController.controllerTilemap.currentPalette - 1;
            }
		}

		// Add free camera mode
		if (LevelEditorData.Level?.IsometricData != null) {
			var cam = Controller.obj?.levelController?.editor?.cam;
			if (cam != null) {
				s.FreeCameraMode = cam.FreeCameraMode;
			}
		}

		return s;
	}

    public void ParseMessage(string msgString) {
		WebJSON.Message msg = Newtonsoft.Json.JsonConvert.DeserializeObject<WebJSON.Message>(msgString, JsonSettings);

		if (msg.Settings != null) {
            ParseSettingsJSON(msg.Settings);
        }
		if (msg.Request != null) {
			ParseRequestJSON(msg.Request);
		}
		if (msg.Selection != null) {
			ParseSelectionJSON(msg.Selection);
		}
		if (msg.Object != null) {
			ParseObjectJSON(msg.Object);
		}
    }
    private void ParseSettingsJSON(WebJSON.Settings msg) {
		if (msg.ShowObjects.HasValue) Settings.ShowObjects = msg.ShowObjects.Value;
		if (msg.ShowTiles.HasValue) Settings.ShowTiles = msg.ShowTiles.Value;
		if (msg.ShowCollision.HasValue) Settings.ShowCollision = msg.ShowCollision.Value;
		if (msg.ShowLinks.HasValue) Settings.ShowLinks = msg.ShowLinks.Value;
		if (msg.ShowObjCollision.HasValue) Settings.ShowObjCollision = msg.ShowObjCollision.Value;
		if (msg.AnimateSprites.HasValue) Settings.AnimateSprites = msg.AnimateSprites.Value;
		if (msg.AnimateTiles.HasValue) Settings.AnimateTiles = msg.AnimateTiles.Value;
		if (msg.ShowAlwaysObjects.HasValue) Settings.ShowAlwaysObjects = msg.ShowAlwaysObjects.Value;
		if (msg.ShowEditorObjects.HasValue) Settings.ShowEditorObjects = msg.ShowEditorObjects.Value;
		if (msg.ShowObjOffsets.HasValue) Settings.ShowObjOffsets = msg.ShowObjOffsets.Value;
		if (msg.ShowGizmos.HasValue) Settings.ShowDefaultObjIcons = msg.ShowGizmos.Value;
		if (msg.ShowRayman.HasValue) Settings.ShowRayman = msg.ShowRayman.Value;
		if (msg.ShowDebugInfo.HasValue) Settings.ShowDebugInfo = msg.ShowDebugInfo.Value;
		if (msg.StateSwitchingMode.HasValue) Settings.StateSwitchingMode = msg.StateSwitchingMode.Value;
		if (msg.ShowGridMap.HasValue) Settings.ShowGridMap = msg.ShowGridMap.Value;
		if (msg.CrashTimeTrialMode.HasValue) Settings.GBAVV_Crash_TimeTrialMode = msg.CrashTimeTrialMode.Value;

		if (msg.Layers != null && msg.Layers.Length > 0) {
			var lvl = LevelEditorData.Level;
			var tilemapController = Controller.obj?.levelController?.controllerTilemap;
			if (lvl != null && tilemapController != null) {
				foreach (var layer in msg.Layers) {
					switch (layer.Index) {
						case LevelTilemapController.Index_Background:
							if (lvl.Background != null && tilemapController.background != null) {
								var bg = tilemapController.background;
								if (layer.IsVisible.HasValue && layer.IsVisible.Value != bg.gameObject.activeSelf) {
									bg.gameObject.SetActive(layer.IsVisible.Value);
								}
							}
							break;
						case LevelTilemapController.Index_ParallaxBackground:
							if (lvl.ParallaxBackground != null && tilemapController.backgroundParallax != null) {
								var bg = tilemapController.backgroundParallax;
								if (layer.IsVisible.HasValue && layer.IsVisible.Value != bg.gameObject.activeSelf) {
									bg.gameObject.SetActive(layer.IsVisible.Value);
								}
							}
							break;
						default:
							if (layer.Index < lvl.Maps.Length && tilemapController.IsLayerVisible != null) {
								tilemapController.IsLayerVisible[layer.Index] = layer.IsVisible.Value;
							}
							break;
					}
				}
			}
		}
		if (msg.ObjectLayers != null && msg.ObjectLayers.Length > 0 && LevelEditorData.ShowEventsForMaps != null) {
			foreach (var layer in msg.ObjectLayers) {
				if (layer.IsVisible.HasValue) {
					if (LevelEditorData.ShowEventsForMaps.Length > layer.Index) {
						LevelEditorData.ShowEventsForMaps[layer.Index] = layer.IsVisible.Value;
					}
				}
			}
		}
		bool updatedShowCollision = false;
		if (msg.FreeCameraMode.HasValue) {
			if (LevelEditorData.Level?.IsometricData != null) {
				var cam = Controller.obj?.levelController?.editor?.cam;
				if (cam != null) {
					if (cam.ToggleFreeCameraMode(msg.FreeCameraMode.Value)) {
						updatedShowCollision = true;
					}
				}
			}
		}
		if (msg.Palette.HasValue) 
        {
            if (Controller.obj?.levelController?.controllerTilemap != null)
            {
                if (Controller.obj.levelController.controllerTilemap.HasAutoPaletteOption)
                    Controller.obj.levelController.controllerTilemap.currentPalette = msg.Palette.Value;
				else
                    Controller.obj.levelController.controllerTilemap.currentPalette = msg.Palette.Value + 1;
            }
		}

		if (msg.BackgroundTint.HasValue) {
			var bgTint = Controller.obj?.levelController?.controllerTilemap?.backgroundTint;
			if (bgTint != null) bgTint.color = msg.BackgroundTint.Value;
		}
		if (msg.BackgroundTintDark.HasValue) {
			Camera.main.backgroundColor = msg.BackgroundTintDark.Value;
		}
		if (updatedShowCollision) {
			SendSettings();
		}
	}
	private void ParseSelectionJSON(WebJSON.Selection msg) {
		if (msg?.Object != null) {
			Controller.obj.levelEventController.SelectEvent(msg.Object.Index, true);
		}
	}
	private void ParseObjectJSON(WebJSON.Object msg) {
		if (msg == null || msg.Index < -1) return;
		var objects = Controller.obj.levelController.Objects;
		if (msg.Index > objects.Count) return;
		Unity_ObjBehaviour o = msg.Index == -1 ? Controller.obj.levelController.RaymanObject : objects[msg.Index];
		if (o == null) return;

		bool refreshObjectLists = false;

		// Now we have the object, parse it
		if (msg.X.HasValue) o.ObjData.XPosition = (short)msg.X.Value;
		if (msg.Y.HasValue) o.ObjData.YPosition = (short)msg.Y.Value;
		if (msg.IsEnabled.HasValue) o.IsEnabled = msg.IsEnabled.Value;
		if (msg.StateIndex.HasValue) o.ObjData.CurrentUIState = msg.StateIndex.Value;
		if (msg.Position3D.HasValue && o.ObjData is Unity_Object_3D o3d && LevelEditorData.Level?.IsometricData != null) {
			o3d.Position = msg.Position3D.Value;
		}
		switch (o.ObjData) {
			case Unity_Object_R1 r1o:
				if (msg.R1_ETAIndex.HasValue && r1o.ETAIndex != msg.R1_ETAIndex.Value) {
					r1o.ETAIndex = msg.R1_ETAIndex.Value;
					refreshObjectLists = true;
				}
				if (msg.R1_DESIndex.HasValue && r1o.DESIndex != msg.R1_DESIndex.Value) {
					r1o.DESIndex = msg.R1_DESIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_R2 r2o:
				if (msg.R2_AnimGroupIndex.HasValue && r2o.AnimGroupIndex != msg.R2_AnimGroupIndex.Value) {
					r2o.AnimGroupIndex = msg.R2_AnimGroupIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_R1Jaguar r1jo:
				if (msg.R1Jaguar_EventDefinitionIndex.HasValue && r1jo.EventDefinitionIndex != msg.R1Jaguar_EventDefinitionIndex.Value) {
					r1jo.EventDefinitionIndex = msg.R1Jaguar_EventDefinitionIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBA gbao:
				if (msg.GBA_ActorModelIndex.HasValue && gbao.ActorModelIndex != msg.GBA_ActorModelIndex.Value) {
					gbao.ActorModelIndex = msg.GBA_ActorModelIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBC gbco:
				if (msg.GBC_ActorModelIndex.HasValue && gbco.ActorModelIndex != msg.GBC_ActorModelIndex.Value) {
					gbco.ActorModelIndex = msg.GBC_ActorModelIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBARRR rrro:
				if (msg.GBARRR_AnimationGroupIndex.HasValue && rrro.AnimationGroupIndex != msg.GBARRR_AnimationGroupIndex.Value) {
					rrro.AnimationGroupIndex = msg.GBARRR_AnimationGroupIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBARRRMode7Unused rrrMode7Unused_o:
				if (msg.GBARRR_AnimationGroupIndex.HasValue && rrrMode7Unused_o.AnimationGroupIndex != msg.GBARRR_AnimationGroupIndex.Value) {
                    rrrMode7Unused_o.AnimationGroupIndex = msg.GBARRR_AnimationGroupIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBAIsometricRHR rhro:
				if (msg.GBAIsometric_AnimSetIndex.HasValue && rhro.AnimSetIndex != msg.GBAIsometric_AnimSetIndex.Value) {
					rhro.AnimSetIndex = msg.GBAIsometric_AnimSetIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBAIsometricSpyro spyro_o:
				if (msg.GBAIsometric_AnimSetIndex.HasValue && spyro_o.AnimSetIndex != msg.GBAIsometric_AnimSetIndex.Value) {
					spyro_o.AnimSetIndex = msg.GBAIsometric_AnimSetIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBAIsometricSpyro2_2D spyro2D_o:
				if (msg.GBAIsometric_AnimSetIndex.HasValue && spyro2D_o.AnimSetIndex != msg.GBAIsometric_AnimSetIndex.Value) {
					spyro2D_o.AnimSetIndex = msg.GBAIsometric_AnimSetIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_SNES snes_o:
				if (msg.SNES_GraphicsGroupIndex.HasValue && snes_o.GraphicsGroupIndex != msg.SNES_GraphicsGroupIndex.Value) {
					snes_o.GraphicsGroupIndex = msg.SNES_GraphicsGroupIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBAVV crashObj:
				if (msg.GBAVV_AnimSetIndex.HasValue && crashObj.AnimSetIndex != msg.GBAVV_AnimSetIndex.Value) {
                    crashObj.AnimSetIndex = msg.GBAVV_AnimSetIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBAVVMode7 crashObj:
				if (msg.GBAVV_AnimSetIndex.HasValue && crashObj.AnimSetIndex != msg.GBAVV_AnimSetIndex.Value) {
                    crashObj.AnimSetIndex = msg.GBAVV_AnimSetIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GBAVVNitroKart crashObj:
				if (msg.GBAVV_AnimSetIndex.HasValue && crashObj.AnimSetIndex != msg.GBAVV_AnimSetIndex.Value) {
                    crashObj.AnimSetIndex = msg.GBAVV_AnimSetIndex.Value;
					refreshObjectLists = true;
				}
				break;
			case Unity_Object_GameloftRRR glRRRObj:
				if (msg.Gameloft_PuppetIndex.HasValue && glRRRObj?.PuppetIndex != msg.Gameloft_PuppetIndex.Value) {
					glRRRObj.PuppetIndex = msg.Gameloft_PuppetIndex.Value;
					refreshObjectLists = true;
				}
				break;
		}

		if (refreshObjectLists)
			Send(GetSelectionMessageJSON(includeLists: true, includeDetails: true));
    }
	private void ParseRequestJSON(WebJSON.Request msg) {
		switch (msg.Type) {
			// TODO: Other types of requests, like the web version asking for commands of an event
			// (these are sent separately, otherwise too much to parse at once)
			case WebJSON.RequestType.Screenshot:
				TakeScreenshot(msg.Screenshot).Forget(); // Start the async task for taking a screenshot
				break;
		}
	}
	async UniTaskVoid TakeScreenshot(WebJSON.Screenshot msg) {
		if (msg != null) {
			TransparencyCaptureBehaviour tcb = Camera.main.GetComponent<TransparencyCaptureBehaviour>();
			if (tcb != null) {
				try {
					if (msg.Type == WebJSON.Screenshot.ScreenshotType.FullLevel) {
						System.DateTime dateTime = System.DateTime.Now;
						byte[] screenshotBytes = await tcb.CaptureFulllevel(msg.IsTransparent ?? true);
						SaveFile(screenshotBytes, screenshotBytes.Length, $"Screenshot_{dateTime.ToString("yyyy_MM_dd HH_mm_ss")}.png");
					} else {
						Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
						int height = msg.Height ?? Mathf.RoundToInt(res.height * (msg.SizeFactor ?? 1));
						int width = msg.Width ?? Mathf.RoundToInt(res.width * (msg.SizeFactor ?? 1));
						if (width > 0 && height > 0) {
							System.DateTime dateTime = System.DateTime.Now;
							byte[] screenshotBytes = await tcb.Capture(width, height, msg.IsTransparent ?? true);
							SaveFile(screenshotBytes, screenshotBytes.Length, $"Screenshot_{dateTime.ToString("yyyy_MM_dd HH_mm_ss")}.png");
						}
					}
				} catch (Exception) {
					Debug.Log("Screenshot failed");
				}
			}
		}
	}
}
