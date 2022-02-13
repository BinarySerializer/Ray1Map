using BinarySerializer.Ray1;
using System;
using System.Collections.Generic;
using System.Linq;
using Ray1Map.Rayman1;
using UnityEngine;
using UnityEngine.UI;

namespace Ray1Map {
	public class LegacyEditorUIController_Objects : MonoBehaviour {
		public LegacyEditorUIController UIController;

        public Dropdown eventDropdown;

        // Event info things for the ui
        public Text infoName;
        public InputField infoX;
        public InputField infoY;
        public Dropdown infoDes;
        public Dropdown infoEta;
        public Dropdown infoEtat;
        public Dropdown infoSubEtat;
        public InputField infoOffsetBx;
        public InputField infoOffsetBy;
        public InputField infoOffsetHy;
        public InputField infoFollowSprite;
        public InputField infoHitPoints;
        public InputField infoHitSprite;
        public InputField infoAnimIndex;
        public InputField infoLayer;
        public Toggle infoFollow;
        public Dropdown infoType;

        //Command ui stuff
        public List<LegacyEditorUI_ObjectCommandItem> commandLines;
        public Transform commandListParent;

        // Properties
        public Unity_SpriteObjBehaviour SelectedObject => UIController.Controller.levelEventController.SelectedEvent;
        public Unity_SpriteObjBehaviour PrevSelectedObject { get; set; }

        public bool HasLoaded { get; private set; }
        public bool LogModifications => false;


        #region Field Changed Methods

        private void FieldUpdated<T>(Action<T> updateAction, T newValue, Func<T> currentValue, string logName)
            where T : IComparable {
            if (SelectedObject != null && !newValue.Equals(currentValue())) {
                updateAction(newValue);
                SelectedObject.ObjData.HasPendingEdits = true;

                if (LogModifications)
                    Debug.Log($"{logName} has been modified");
            }
        }

        public void FieldXPosition() => FieldUpdated(x => SelectedObject.ObjData.XPosition = x, Int16.TryParse(infoX.text, out var v) ? v : (short)0, () => SelectedObject.ObjData.XPosition, "XPos");
        public void FieldYPosition() => FieldUpdated(x => SelectedObject.ObjData.YPosition = x, Int16.TryParse(infoY.text, out var v) ? v : (short)0, () => SelectedObject.ObjData.YPosition, "YPos");
        public void FieldDes() => FieldUpdated(x => Wrapper.DES = x, infoDes.value, () => Wrapper.DES, "DES");
        public void FieldEta() => FieldUpdated(x => Wrapper.ETA = x, infoEta.value, () => Wrapper.ETA, "ETA");
        public void FieldEtat() => FieldUpdated(x => Wrapper.Etat = x, (byte)infoEtat.value, () => Wrapper.Etat, "Etat");
        public void FieldSubEtat() => FieldUpdated(x => Wrapper.SubEtat = x, (byte)infoSubEtat.value, () => Wrapper.SubEtat, "SubEtat");
        public void FieldOffsetBx() => FieldUpdated(x => {
            if (SelectedObject.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetBX = x;
        }, byte.TryParse(infoOffsetBx.text, out var v) ? v : (byte)0, () => SelectedObject.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetBX : default, "BX");
        public void FieldOffsetBy() => FieldUpdated(x => {
            if (SelectedObject.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetBY = x;
        }, byte.TryParse(infoOffsetBy.text, out var v) ? v : (byte)0, () => SelectedObject.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetBY : default, "BY");
        public void FieldOffsetHy() => FieldUpdated(x => {
            if (SelectedObject.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.OffsetHY = x;
        }, byte.TryParse(infoOffsetHy.text, out var v) ? v : (byte)0, () => SelectedObject.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.OffsetHY : default, "HY");
        public void FieldFollowSprite() => FieldUpdated(x => {
            if (SelectedObject.ObjData is Unity_Object_R1 r1obj)
                r1obj.EventData.FollowSprite = x;
        }, byte.TryParse(infoFollowSprite.text, out var v) ? v : (byte)0, () => SelectedObject.ObjData is Unity_Object_R1 r1obj ? r1obj.EventData.FollowSprite : default, "FollowSprite");
        public void FieldHitPoints() => FieldUpdated(x => Wrapper.HitPoints = x, UInt32.TryParse(infoHitPoints.text, out var v) ? v : 0, () => Wrapper.HitPoints, "HitPoints");
        public void FieldHitSprite() => FieldUpdated(x => Wrapper.HitSprite = x, byte.TryParse(infoHitSprite.text, out var v) ? v : (byte)0, () => Wrapper.HitSprite, "HitSprite");
        public void FieldFollowEnabled() => FieldUpdated(x => Wrapper.FollowEnabled = x, infoFollow.isOn, () => Wrapper.FollowEnabled, "FollowEnabled");

        public void FieldType() => FieldUpdated(x => Wrapper.Type = (ushort)x, infoType.value, () => Wrapper.Type, "Type");

        public void FieldEventToCreate() {
            Controller.obj.levelEventController.ObjectIndexToCreate = eventDropdown.value;
        }
        public void FieldAnimIndex() => throw new NotImplementedException("The animation index can not be updated");

		#endregion

        private BaseLegacyEditorWrapper Wrapper => SelectedObject?.ObjData?.LegacyWrapper;

		private void Start() {
            //Create empty list for commandlines
            commandLines = new List<LegacyEditorUI_ObjectCommandItem>();
        }

		private void Update() {
            if (!HasLoaded) {
                if (Controller.LoadState == Controller.State.Finished) {
                    InitUI();
                } else {
                    return;
                }
            }
			UpdateUI();
		}


        public void InitUI() {
            HasLoaded = true;

            // Fill eventinfo dropdown with the event types
            if (LevelEditorData.CurrentSettings.MajorEngineVersion == MajorEngineVersion.Rayman1) {
                infoType.options.AddRange(EnumHelpers.GetValues<ObjType>().Select(x => new Dropdown.OptionData {
                    text = x.ToString()
                }));
            } else {
                infoType.options.AddRange(Enumerable.Range(0, 1000).Select(x => new Dropdown.OptionData {
                    text = x.ToString()
                }));
            }

            // Fill the dropdown menu
            var objs = LevelEditorData.ObjManager.GetAvailableObjects;
            var list = new List<Dropdown.OptionData>();
            for (int i = 0; i < objs.Length; i++) {
                var entry = new Dropdown.OptionData();

                if (LevelEditorData.ObjManager.IsObjectAlways(i)) {
                    entry.text = "A-" + objs[i];
                } else {
                    entry.text = objs[i];
                }

                list.Add(entry);
            }
            eventDropdown.options = list;
            //eventDropdown.options = LevelEditorData.ObjManager.GetAvailableObjects.Select(x => new Dropdown.OptionData {
            //    text = x               
            //}).ToList();

            // Default to the first event
            eventDropdown.captionText.text = eventDropdown.options.FirstOrDefault()?.text;

            // Fill Des and Eta dropdowns with their max values
            infoDes.options = LevelEditorData.ObjManager.LegacyDESNames.Select(x => new Dropdown.OptionData(x)).ToList();
            infoEta.options = LevelEditorData.ObjManager.LegacyETANames.Select(x => new Dropdown.OptionData(x)).ToList();
        }

        private void UpdateUI() {
            var wrapper = Wrapper;

            // Update Etat and SubEtat drop downs
            var etatLength = wrapper?.EtatLength ?? 0;
            var subEtatLength = wrapper?.SubEtatLength ?? 0;

            if (infoEtat.options.Count != etatLength) {
                // Clear old options
                infoEtat.options.Clear();

                // Set new options
                infoEtat.options.AddRange(Enumerable.Range(0, etatLength).Select(x => new Dropdown.OptionData {
                    text = x.ToString()
                }));
            }
            if (infoSubEtat.options.Count != subEtatLength) {
                // Clear old options
                infoSubEtat.options.Clear();

                // Set new options
                infoSubEtat.options.AddRange(Enumerable.Range(0, subEtatLength).Select(x => new Dropdown.OptionData {
                    text = x.ToString()
                }));
            }

            // Helper method for updating a field
            void updateInputField<T>(InputField field, T value, Func<string, T> parser)
                where T : IComparable {
                T parsed = parser(field.text);

                if ((field.isFocused || EqualityComparer<T>.Default.Equals(parsed, value)) && !String.IsNullOrWhiteSpace(field.text) && PrevSelectedObject == SelectedObject)
                    return;

                field.text = value.ToString();
            }

            // Helper method for updating a drop down
            void updateDropDownIndex(Dropdown field, int index) {
                var selectedIndex = field.value;

                if (index == -1)
                    index = 0;

                if (selectedIndex == index && PrevSelectedObject == SelectedObject)
                    return;

                field.value = index;
            }

            // Helper method for updating a toggle
            void updateToggle(Toggle field, bool value) {
                if (field.isOn == value && PrevSelectedObject == SelectedObject)
                    return;

                field.isOn = value;
            }

            // X Position
            updateInputField<short>(infoX, (short)(SelectedObject?.ObjData.XPosition ?? -1), x => Int16.TryParse(x, out var r) ? r : (short)0);

            // Y Position
            updateInputField<short>(infoY, (short)(SelectedObject?.ObjData.YPosition ?? -1), x => Int16.TryParse(x, out var r) ? r : (short)0);

            // DES
            updateDropDownIndex(infoDes, wrapper?.DES ?? 0);

            // ETA
            updateDropDownIndex(infoEta, wrapper?.ETA ?? 0);

            // Etat
            updateDropDownIndex(infoEtat, wrapper?.Etat ?? 0);

            // SubEtat
            updateDropDownIndex(infoSubEtat, wrapper?.SubEtat ?? 0);

            // OffsetBX
            updateInputField<byte>(infoOffsetBx, wrapper?.OffsetBX ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // OffsetBY
            updateInputField<byte>(infoOffsetBy, wrapper?.OffsetBY ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // OffsetHY
            updateInputField<byte>(infoOffsetHy, wrapper?.OffsetHY ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // FollowSprite
            updateInputField<byte>(infoFollowSprite, wrapper?.FollowSprite ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // HitPoints
            updateInputField<uint>(infoHitPoints, wrapper?.HitPoints ?? 0, x => UInt32.TryParse(x, out var r) ? r : 0);

            // HitSprite
            updateInputField<byte>(infoHitSprite, wrapper?.HitSprite ?? 0, x => Byte.TryParse(x, out var r) ? r : (byte)0);

            // FollowEnabled
            updateToggle(infoFollow, wrapper?.FollowEnabled ?? false);

            // Type
            updateDropDownIndex(infoType, wrapper?.Type ?? 0);

            // Set other fields
            infoName.text = SelectedObject?.ObjData.Name ?? String.Empty;

            if (PrevSelectedObject != SelectedObject) {
                if (SelectedObject == null) {
                    ClearInfoWindow();
                } else {
                    // Change event info if event is selected
                    infoAnimIndex.text = SelectedObject.ObjData.AnimationIndex.ToString();
                    infoLayer.text = SelectedObject.Layer.ToString();

                    // Clear old commands
                    ClearCommands();

                    if (FileSystem.mode == FileSystem.Mode.Normal) {
                        if (SelectedObject.ObjData is Unity_Object_R1 r1obj) {
                            // Fill out the commands
                            foreach (var c in r1obj.EventData.Commands?.Commands ?? new Command[0]) {
                                LegacyEditorUI_ObjectCommandItem cmd = Instantiate<GameObject>(Controller.obj?.Prefabs?.LegacyEditorUI_ObjectCommandItem, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<LegacyEditorUI_ObjectCommandItem>();
                                cmd.command = c;
                                cmd.transform.SetParent(commandListParent, false);
                                commandLines.Add(cmd);
                            }
                        }
                    }
                }
            }

            PrevSelectedObject = SelectedObject;
        }



        private void ClearInfoWindow() {
            infoName.text = "";
            infoAnimIndex.text = "";
            infoLayer.text = "";

            ClearCommands();
        }

        private void ClearCommands() {
            foreach (var c in commandLines) {
                Destroy(c.gameObject);
            }
            commandLines.Clear();
        }
    }
}