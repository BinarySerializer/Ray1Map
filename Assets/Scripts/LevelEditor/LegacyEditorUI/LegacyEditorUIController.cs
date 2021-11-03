using BinarySerializer.Ray1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ray1Map {
	public class LegacyEditorUIController : MonoBehaviour {
		public Controller Controller;
        public LegacyEditorUIController_Objects ObjectsUI;

		#region Functions referenced by UI buttons
		public void Button_Tab_TabClicked(int tabIndex) {
			Controller.levelController.TabClicked(tabIndex);
		}
		public void Button_Tiles_SetPalette(int paletteIndex) {
			Controller.levelController.controllerTilemap.RefreshTiles(paletteIndex);
		}
		public void Button_Tiles_ExportTileset() {
			Controller.levelController.ExportTileset();
		}
		public void Button_Tiles_ShowHideTemplate() {
			Controller.levelController.controllerTilemap.ShowHideTemplate();
		}
		public void Button_Types_SetCurrentType(int type) {
			Controller.levelController.editor.SetCurrentType(type);
		}
		#endregion

		#region Functions referenced by UI Input Fields
		public void InputField_Events_OnEndEdit_XPosition() {
			ObjectsUI.FieldXPosition();
        }
        public void InputField_Events_OnEndEdit_YPosition() {
            ObjectsUI.FieldYPosition();
        }
        public void InputField_Events_OnEndEdit_DES() {
            ObjectsUI.FieldDes();
        }
        public void InputField_Events_OnEndEdit_ETA() {
            ObjectsUI.FieldEta();
        }
        public void InputField_Events_OnEndEdit_Etat() {
            ObjectsUI.FieldEtat();
        }
        public void InputField_Events_OnEndEdit_SubEtat() {
            ObjectsUI.FieldSubEtat();
        }
        public void InputField_Events_OnEndEdit_OffsetBx() {
            ObjectsUI.FieldOffsetBx();
        }
        public void InputField_Events_OnEndEdit_OffsetBy() {
            ObjectsUI.FieldOffsetBy();
        }
        public void InputField_Events_OnEndEdit_OffsetHy() {
            ObjectsUI.FieldOffsetHy();
        }
        public void InputField_Events_OnEndEdit_FollowSprite() {
            ObjectsUI.FieldFollowSprite();
        }
        public void InputField_Events_OnEndEdit_HitPoints() {
            ObjectsUI.FieldHitPoints();
        }
        public void InputField_Events_OnEndEdit_HitSprite() {
            ObjectsUI.FieldHitSprite();
        }
        public void InputField_Events_OnEndEdit_AnimIndex() {
            ObjectsUI.FieldAnimIndex();
        }
        #endregion

        #region Functions referenced by UI Toggles
        public void Toggle_Events_OnChanged_FollowEnabled() {
            ObjectsUI.FieldFollowEnabled();
        }
        #endregion

        #region Functions referenced by UI Dropdowns
        public void Dropdown_Events_OnChanged_Type() {
            ObjectsUI.FieldType();
        }

        public void Dropdown_Events_OnChanged_EventToCreate() {
            ObjectsUI.FieldEventToCreate();
        }
        #endregion
    }
}