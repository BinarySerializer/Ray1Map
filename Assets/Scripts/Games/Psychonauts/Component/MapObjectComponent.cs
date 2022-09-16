using UnityEngine;

namespace Ray1Map.Psychonauts
{
    public class MapObjectComponent : MonoBehaviour
    {
        public GameObject MapObject;

        private void Update()
        {
            if (Settings.ShowCollision == MapObject.activeSelf)
                MapObject.SetActive(!Settings.ShowCollision);
        }
    }
}