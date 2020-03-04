using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine {
    public class LevelEventController : MonoBehaviour {

        public GameObject eventParent;
        public GameObject prefabEvent;

        //public GameObject[] prefabEventList;

        public void InitializeEvents() {
            // Make a copy of the original events
            List<Common_Event> prefabList = new List<Common_Event>();

            // Instantiate event prefabs under the parent class
            foreach (var eve in Controller.obj.levelController.currentLevel.Events) {
                // Instantiate prefab
                Common_Event newEvent = Instantiate(prefabEvent, new Vector3(eve.XPosition/16f, -(eve.YPosition/16f), 5f), Quaternion.identity).GetComponent<Common_Event>();
                newEvent.EventInfoData = eve.EventInfoData;
                newEvent.XPosition = eve.XPosition;
                newEvent.YPosition = eve.YPosition;
                // Set as child of events gameobject
                newEvent.gameObject.transform.parent = eventParent.transform;
                // Add to list
                prefabList.Add(newEvent);
            }

            // Replace the original list with the prefab list
            Controller.obj.levelController.currentLevel.Events.Clear();
            Controller.obj.levelController.currentLevel.Events = prefabList;

            Debug.Log(Controller.obj.levelController.currentLevel.Events.Count);
        }
    }
}
