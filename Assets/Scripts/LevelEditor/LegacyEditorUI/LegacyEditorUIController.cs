using BinarySerializer.Ray1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine {
	public class LegacyEditorUIController : MonoBehaviour {
		public Controller Controller;

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
			Controller.levelEventController.FieldXPosition();
        }
        public void InputField_Events_OnEndEdit_YPosition() {
            Controller.levelEventController.FieldYPosition();
        }
        public void InputField_Events_OnEndEdit_DES() {
            Controller.levelEventController.FieldDes();
        }
        public void InputField_Events_OnEndEdit_ETA() {
            Controller.levelEventController.FieldEta();
        }
        public void InputField_Events_OnEndEdit_Etat() {
            Controller.levelEventController.FieldEtat();
        }
        public void InputField_Events_OnEndEdit_SubEtat() {
            Controller.levelEventController.FieldSubEtat();
        }
        public void InputField_Events_OnEndEdit_OffsetBx() {
            Controller.levelEventController.FieldOffsetBx();
        }
        public void InputField_Events_OnEndEdit_OffsetBy() {
            Controller.levelEventController.FieldOffsetBy();
        }
        public void InputField_Events_OnEndEdit_OffsetHy() {
            Controller.levelEventController.FieldOffsetHy();
        }
        public void InputField_Events_OnEndEdit_FollowSprite() {
            Controller.levelEventController.FieldFollowSprite();
        }
        public void InputField_Events_OnEndEdit_HitPoints() {
            Controller.levelEventController.FieldHitPoints();
        }
        public void InputField_Events_OnEndEdit_HitSprite() {
            Controller.levelEventController.FieldHitSprite();
        }
        public void InputField_Events_OnEndEdit_AnimIndex() {
            Controller.levelEventController.FieldAnimIndex();
        }
        #endregion

        #region Functions referenced by UI Toggles
        public void Toggle_Events_OnChanged_FollowEnabled() {
            Controller.levelEventController.FieldFollowEnabled();
        }
        public void Dropdown_Events_OnChanged_Type() {
            Controller.levelEventController.FieldType();
        }
        #endregion
    }
}