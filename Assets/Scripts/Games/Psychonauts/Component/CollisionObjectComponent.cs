using UnityEngine;

namespace Ray1Map.Psychonauts
{
    public class CollisionObjectComponent : MonoBehaviour
    {
        public GameObject CollisionObject;

        private void Update()
        {
            if (Settings.ShowCollision != CollisionObject.activeSelf)
                CollisionObject.SetActive(Settings.ShowCollision);
        }
    }
}