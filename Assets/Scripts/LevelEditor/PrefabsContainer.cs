using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace R1Engine {
	public class PrefabsContainer : MonoBehaviour {
        // Reference to spritepart prefab
        public GameObject SpriteAnimation_Sprite;

        // Reference to the box prefab
        public GameObject SpriteAnimation_CollisionVisualizationBox;

        // Used in LegacyEditorUI_EventList.cs
        public GameObject LegacyEditorUI_EventListItem;
    }
}
