using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAVVNitroKartWaypoint : Unity_Object_3D {
        public Unity_Object_GBAVVNitroKartWaypoint(GBAVV_NitroKart_TrackWaypoint obj, int? objectGroupIndex, int trackDataIndex) {
            Object = obj;
            ObjectGroupIndex = objectGroupIndex;
            TrackDataIndex = trackDataIndex;
            UIStates = new UIState[0];
        }

        public GBAVV_NitroKart_TrackWaypoint Object { get; set; }

        public override short XPosition {
            get => Object.XPos;
            set => Object.XPos = value;
        }

        public override short YPosition {
            get => Object.YPos;
            set => Object.YPos = value;
        }

        public override Vector3 Position {
            get => new Vector3(Object.XPos, Object.YPos, Height);
            set {
                Object.XPos = (short)value.x;
                Object.YPos = (short)value.y;
                Height = value.z;
            }
        }

        public int? LinkedWayPointIndex { get; set; }

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links {
            get {
                if(LinkedWayPointIndex.HasValue)
                    yield return LinkedWayPointIndex.Value;
            }
        }

		public float Height { get; set; }

		public override float Scale => 0.5f;

		public override string DebugText => null;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => new DummyLegacyEditorWrapper(this);

        public override string PrimaryName => $"Waypoint_{TrackDataIndex + 1}";
        public override string SecondaryName => null;

        public override ObjectType Type => ObjectType.Waypoint;
        public override bool IsEditor => true;

        public override int? ObjectGroupIndex { get; }
        public int TrackDataIndex { get; }

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => null;
        protected override int GetSpriteID => -1;
        public override IList<Sprite> Sprites => null;

        #region UI States

        protected override bool IsUIStateArrayUpToDate => true;

        protected override void RecalculateUIStates() { }
        
        #endregion
    }
}