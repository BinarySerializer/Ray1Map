using R1Engine;
using System.Linq;
using UnityEngine;

public class ObjectHighlight : MonoBehaviour {
    public Unity_ObjBehaviour highlightedObject = null;
    public Unity_Tile[] highlightedCollision = null;
    public Unity_Tile[] highlightedTile = null;

    private void HandleCollision() {
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("Object");

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
        if (hits != null && hits.Length > 0) {
            System.Array.Sort(hits, (x, y) => (x.distance.CompareTo(y.distance)));
            if (Settings.ShowObjects) {
                for (int i = 0; i < hits.Length; i++) {
                    // the object identified by hit.transform was clicked
                    Unity_ObjBehaviour ob = hits[i].transform.GetComponentInParent<Unity_ObjBehaviour>();
                    if (ob != null) {
                        highlightedObject = ob;
                        break;
                    }
                }
            }
        }
        Vector2Int mouseTile = Controller.obj.levelController.controllerTilemap.MouseToTileInt(Input.mousePosition);
        highlightedCollision = LevelEditorData.Level?.Maps?.Select(x => x.GetMapTile(mouseTile.x, mouseTile.y)).ToArray();
        highlightedTile = LevelEditorData.Level?.Maps?.Select(x => x.GetMapTile(mouseTile.x, mouseTile.y)).ToArray();
    }

    void Update() {
        highlightedObject = null;
        highlightedCollision = null;
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        if (Controller.LoadState == Controller.State.Finished
            && screenRect.Contains(Input.mousePosition)
            ) HandleCollision();
    }
}
