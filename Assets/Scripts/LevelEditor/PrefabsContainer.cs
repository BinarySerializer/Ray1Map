using UnityEngine;

namespace Ray1Map {
	public class PrefabsContainer : MonoBehaviour {
        // Reference to spritepart prefab
        public GameObject SpriteAnimation_Sprite;

        // Reference to the box prefab
        public GameObject SpriteAnimation_CollisionVisualizationBox;

        // Used in LegacyEditorUI_EventList.cs
        public GameObject LegacyEditorUI_EventListItem;

        // Prefab for a sprite animation object, to be removed and replaced with code creation of a Unity ObjBehaviour and all of its components
        public GameObject Object_Behaviour;

        // Used in LegacyEditorUIController_OBjects
        public GameObject LegacyEditorUI_ObjectCommandItem;
    }
}
