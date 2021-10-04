using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_ObjBehaviour : MonoBehaviour
    {
        #region Public Properties

        public Unity_Object ObjData { get; set; }
        public int Index { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool IsSelected { get; set; }
        public bool ShowOffsets => (IsSelected || Settings.ShowObjOffsets) && EnableBoxCollider;
        public bool ShowCollision => (IsSelected || Settings.ShowObjCollision) && IsVisible;

        public abstract bool ShowGizmo { get; }
        public abstract bool EnableBoxCollider { get; }
        public abstract bool HasObjCollision { get; }
        public abstract bool IsVisible { get; }

        #endregion
    }
}