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

	Unity_ObjBehaviour highlightedObject_;
	Unity_ObjBehaviour selectedObject_;
	int x_, y_;

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
    }

    public void Update() {
        if (Controller.LoadState == Controller.State.Finished && !sentHierarchy) {
            SendHierarchy();
            sentHierarchy = true;
		}
        if ((Application.platform == RuntimePlatform.WebGLPlayer || debugMessages) && Controller.LoadState == Controller.State.Finished) {
			// TODO: Handle highlight & selection changes like in raymap:
			var hl = Controller.obj.levelController.editor.objectHighlight;
			// TODO: Also check highlighted collision?
			if (highlightedObject_ != hl.highlightedObject) {
				highlightedObject_ = hl.highlightedObject;
				Send(GetHighlightMessageJSON());
			}

			// Check selected object
			if (selectedObject_ != Controller.obj.levelEventController.SelectedEvent) {
				selectedObject_ = Controller.obj.levelEventController.SelectedEvent;
				if (selectedObject_ != null) {
					x_ = selectedObject_.ObjData.XPosition;
					y_ = selectedObject_.ObjData.YPosition;
					// TODO: keep state indices so updates on animation speed, etc. can be sent
					//selectedPersoStateIndex_ = selectedPerso_.currentState;
					Send(GetSelectionMessageJSON(includeLists: true, includeDetails: true));
				}
			}

			// Check selected object's changed values
			if (selectedObject_ != null) {
				if (selectedObject_.ObjData.XPosition != x_ || selectedObject_.ObjData.YPosition != y_) {
					x_ = selectedObject_.ObjData.XPosition;
					y_ = selectedObject_.ObjData.YPosition;
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
    public void Send(WebJSON.Message obj) {
		if (Application.platform == RuntimePlatform.WebGLPlayer) {
			if (Controller.LoadState == Controller.State.Finished) {
				string json = SerializeMessage(obj);
				UnityJSMessage(json);
			}
		} else if(debugMessages) {
			if (Controller.LoadState == Controller.State.Finished) {
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
				Object = GetObjectJSON(highlightedObject_),
				// Collision
			}
		};
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
					webObj.R1_DisplayPrio = r1obj.EventData.Layer;

					if (includeLists) {
						webObj.R1_Commands = r1obj.EventData.Commands?.ToTranslatedStrings(r1obj.EventData.LabelOffsets);
						webObj.R1_DESNames = r1obj.ObjManager.DES.Select(x => x.DisplayName).ToArray();
						webObj.R1_ETANames = r1obj.ObjManager.ETA.Select(x => x.DisplayName).ToArray();
					}
					break;

				case Unity_Object_R2 r2obj:
					webObj.R1_Type = (ushort)r2obj.EventData.EventType;
					webObj.R2_AnimGroupIndex = r2obj.AnimGroupIndex;
					webObj.R2_MapLayer = r2obj.MapLayer;
					webObj.R1_Etat = r2obj.EventData.Etat;
					webObj.R1_SubEtat = r2obj.EventData.SubEtat;
					webObj.R1_OffsetBX = r2obj.EventData.CollisionData?.OffsetBX;
					webObj.R1_OffsetBY = r2obj.EventData.CollisionData?.OffsetBY;
					webObj.R1_OffsetHY = r2obj.EventData.CollisionData?.OffsetHY;
					webObj.R1_DisplayPrio = r2obj.EventData.Layer;

                    if (includeLists)
                        webObj.R2_AnimGroupNames = r2obj.ObjManager.AnimGroups.Select(x => x.Pointer?.ToString() ?? "N/A").ToArray();
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
					webObj.GBA_GraphicsDataIndex = gbaObj.GraphicsDataIndex;
					webObj.GBA_State = gbaObj.Actor.StateIndex;

                    if (includeLists)
                        webObj.GBA_GraphicsDataNames = gbaObj.ObjManager.GraphicsDatas.Select(x => x.Index.ToString()).ToArray();
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
			ShowAlwaysEvents = Settings.ShowAlwaysEvents,
			ShowEditorEvents = Settings.ShowEditorEvents,
			ShowDebugInfo = Settings.ShowDebugInfo,
			ShowDefaultObjIcons = Settings.ShowDefaultObjIcons,
			ShowObjOffsets = Settings.ShowObjOffsets,
			ShowRayman = Settings.ShowRayman,
			StateSwitchingMode = Settings.StateSwitchingMode
		};

		// Add layers
		var lvl = LevelEditorData.Level;
		if (lvl != null) {
			List<WebJSON.Layer> layers = new List<WebJSON.Layer>();
			if (lvl.Background != null && Controller.obj?.levelController?.controllerTilemap?.background != null) {
				layers.Add(new WebJSON.Layer() {
					Index = -2,
					IsVisible = Controller.obj.levelController.controllerTilemap.background.gameObject.activeSelf
				});
			}

			if (lvl.ParallaxBackground != null && Controller.obj?.levelController?.controllerTilemap?.backgroundParallax != null) {
				layers.Add(new WebJSON.Layer() {
					Index = -1,
					IsVisible = Controller.obj.levelController.controllerTilemap.backgroundParallax.gameObject.activeSelf
				});
			}

			if (Controller.obj?.levelController?.controllerTilemap?.GraphicsTilemaps != null) {
				for (int i = 0; i < lvl.Maps.Length; i++) {
					var tilemaps = Controller.obj.levelController.controllerTilemap.GraphicsTilemaps;
					layers.Add(new WebJSON.Layer() {
						Index = i,
						IsVisible = tilemaps[i].gameObject.activeSelf
					});
				}
			}
			if (layers.Count > 0) {
				s.Layers = layers.ToArray();
			}
			s.Palettes = new string[] { "Auto" }
			.Concat(Enumerable.Range(0, lvl.Maps.Max(x => x.TileSet.Length)).Select(x => x.ToString())).ToArray();

			if (Controller.obj?.levelController?.controllerTilemap != null)
				s.Palette = Controller.obj.levelController.controllerTilemap.currentPalette;
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
		if (msg.ShowAlwaysEvents.HasValue) Settings.ShowAlwaysEvents = msg.ShowAlwaysEvents.Value;
		if (msg.ShowEditorEvents.HasValue) Settings.ShowEditorEvents = msg.ShowEditorEvents.Value;
		if (msg.ShowObjOffsets.HasValue) Settings.ShowObjOffsets = msg.ShowObjOffsets.Value;
		if (msg.ShowDefaultObjIcons.HasValue) Settings.ShowDefaultObjIcons = msg.ShowDefaultObjIcons.Value;
		if (msg.ShowRayman.HasValue) Settings.ShowRayman = msg.ShowRayman.Value;
		if (msg.ShowDebugInfo.HasValue) Settings.ShowDebugInfo = msg.ShowDebugInfo.Value;
		if (msg.StateSwitchingMode.HasValue) Settings.StateSwitchingMode = msg.StateSwitchingMode.Value;

		if (msg.Layers != null && msg.Layers.Length > 0) {
			var lvl = LevelEditorData.Level;
			var tilemapController = Controller.obj?.levelController?.controllerTilemap;
			if (lvl != null && tilemapController != null) {
				foreach (var layer in msg.Layers) {
					switch (layer.Index) {
						case -2:
							if (lvl.Background != null && tilemapController.background != null) {
								var bg = tilemapController.background;
								if (layer.IsVisible.HasValue && layer.IsVisible.Value != bg.gameObject.activeSelf) {
									bg.gameObject.SetActive(layer.IsVisible.Value);
								}
							}
							break;
						case -1:
							if (lvl.ParallaxBackground != null && tilemapController.backgroundParallax != null) {
								var bg = tilemapController.backgroundParallax;
								if (layer.IsVisible.HasValue && layer.IsVisible.Value != bg.gameObject.activeSelf) {
									bg.gameObject.SetActive(layer.IsVisible.Value);
								}
							}
							break;
						default:
							if (layer.Index < lvl.Maps.Length && tilemapController.GraphicsTilemaps != null) {
								var tilemap = tilemapController.GraphicsTilemaps[layer.Index];
								if (layer.IsVisible.HasValue && layer.IsVisible.Value != tilemap.gameObject.activeSelf) {
									tilemap.gameObject.SetActive(layer.IsVisible.Value);
								}
							}
							break;
					}
				}
			}
		}
		if (msg.Palette.HasValue) {
			if (Controller.obj?.levelController?.controllerTilemap != null)
				Controller.obj.levelController.controllerTilemap.currentPalette = msg.Palette.Value;
		}

		if (msg.BackgroundTint.HasValue) {
			var bgTint = Controller.obj?.levelController?.controllerTilemap?.backgroundTint;
			if (bgTint != null) bgTint.color = msg.BackgroundTint.Value;
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
			case Unity_Object_GBA go:
				if (msg.GBA_GraphicsDataIndex.HasValue && go.GraphicsDataIndex != msg.GBA_GraphicsDataIndex.Value) {
					go.GraphicsDataIndex = msg.GBA_GraphicsDataIndex.Value;
					refreshObjectLists = true;
				}
				break;
		}
		// TODO: More object settings?

		if (refreshObjectLists) {
			Send(GetSelectionMessageJSON(includeLists: true, includeDetails: true));
		}

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
