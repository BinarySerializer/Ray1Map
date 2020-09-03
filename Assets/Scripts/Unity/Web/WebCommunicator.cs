using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using R1Engine;
using R1Engine.Serialize;

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
        if (Application.platform == RuntimePlatform.WebGLPlayer && Controller.LoadState == Controller.State.Finished) {
			// TODO: Handle highlight & selection changes like in raymap:
			// Check highlighted object, highlighted collision, highlighted link?
			// Check selected object
			// Check selected object's state
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
			Hierarchy = new WebJSON.Hierarchy(),
			GameSettings = GetGameSettingsJSON()
		};
		// TODO: Fill in objects info in hierarchy
		return message;
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
		// TODO
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
		return new WebJSON.Settings() {
			AnimateSprites = Settings.AnimateSprites,
			AnimateTiles = Settings.AnimateTiles,
			ShowAlwaysEvents = Settings.ShowAlwaysEvents,
			ShowEditorEvents = Settings.ShowEditorEvents,
			ShowDebugInfo = Settings.ShowDebugInfo,
            ShowDefaultObjIcons = Settings.ShowDefaultObjIcons,
			ShowRayman = Settings.ShowRayman,
			StateSwitchingMode = Settings.StateSwitchingMode
		};
	}

    public void ParseMessage(string msgString) {
		WebJSON.Message msg = Newtonsoft.Json.JsonConvert.DeserializeObject<WebJSON.Message>(msgString, JsonSettings);

		if (msg.Settings != null) {
            ParseSettingsJSON(msg.Settings);
        }
		if (msg.Request != null) {
			ParseRequestJSON(msg.Request);
		}
    }
    private void ParseSettingsJSON(WebJSON.Settings msg) {
		if (msg.AnimateSprites.HasValue) Settings.AnimateSprites = msg.AnimateSprites.Value;
		if (msg.AnimateTiles.HasValue) Settings.AnimateTiles = msg.AnimateTiles.Value;
		if (msg.ShowAlwaysEvents.HasValue) Settings.ShowAlwaysEvents = msg.ShowAlwaysEvents.Value;
		if (msg.ShowEditorEvents.HasValue) Settings.ShowEditorEvents = msg.ShowEditorEvents.Value;
		if (msg.ShowDefaultObjIcons.HasValue) Settings.ShowDefaultObjIcons = msg.ShowDefaultObjIcons.Value;
		if (msg.ShowRayman.HasValue) Settings.ShowRayman = msg.ShowRayman.Value;
		if (msg.ShowDebugInfo.HasValue) Settings.ShowDebugInfo = msg.ShowDebugInfo.Value;
		if (msg.StateSwitchingMode.HasValue) Settings.StateSwitchingMode = msg.StateSwitchingMode.Value;
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
		// TODO: Fix
		if (msg != null) {
			TransparencyCaptureBehaviour tcb = Camera.main.GetComponent<TransparencyCaptureBehaviour>();
			if (tcb != null) {
				try {
					Resolution res = TransparencyCaptureBehaviour.GetCurrentResolution();
					int height = msg.Height ?? Mathf.RoundToInt(res.height * (msg.SizeFactor ?? 1));
					int width = msg.Width ?? Mathf.RoundToInt(res.width * (msg.SizeFactor ?? 1));
					if (width > 0 && height > 0) {
						System.DateTime dateTime = System.DateTime.Now;
						byte[] screenshotBytes = await tcb.Capture(width, height, msg.IsTransparent ?? true);
						SaveFile(screenshotBytes, screenshotBytes.Length, $"Screenshot_{dateTime.ToString("yyyy_MM_dd HH_mm_ss")}.png");
					}
				} catch (Exception) {
					Debug.Log("Screenshot failed");
				}
			}
		}
	}
}
