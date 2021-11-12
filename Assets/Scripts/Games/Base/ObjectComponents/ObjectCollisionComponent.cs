using System.Collections.Generic;
using Ray1Map;
using UnityEngine;

public class ObjectCollisionComponent : MonoBehaviour
{
    public List<GameObject> normalObjects = new List<GameObject>();
    public List<GameObject> collisionObjects = new List<GameObject>();

    private void Update()
    {
        foreach (var obj in normalObjects)
            obj.SetActive(!Settings.ShowCollision);

        foreach (var obj in collisionObjects)
            obj.SetActive(Settings.ShowCollision);
    }
}