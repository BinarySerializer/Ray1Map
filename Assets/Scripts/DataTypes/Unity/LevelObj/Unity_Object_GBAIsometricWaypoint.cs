using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometricWaypoint : Unity_Object_3D
    {
        public Unity_Object_GBAIsometricWaypoint(GBAIsometric_RHR_Waypoint waypoint, Unity_ObjectManager_GBAIsometric objManager)
        {
            Waypoint = waypoint;
            ObjManager = objManager;
        }

        public override ObjectType Type => ObjectType.Waypoint;
        public GBAIsometric_RHR_Waypoint Waypoint { get; }
        public Unity_ObjectManager_GBAIsometric ObjManager { get; }

        public override short XPosition
        {
            get => (short)Waypoint.XPosValue;
            set => Waypoint.XPosValue = value;
        }
        public override short YPosition
        {
            get => (short)Waypoint.YPosValue;
            set => Waypoint.YPosValue = value;
        }
		public override Vector3 Position {
            get => new Vector3(Waypoint.XPosValue, Waypoint.YPosValue, Waypoint.HeightValue);
            set {
                Waypoint.XPosValue = value.x;
                Waypoint.YPosValue = value.y;
                Waypoint.HeightValue = value.z;

            }
        }

		public override bool IsEditor => true;
        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Waypoint;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override string PrimaryName => $"waypoint";
        public override string SecondaryName => null;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;
        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];


        private class LegacyEditorWrapper : ILegacyEditorWrapper {
            public LegacyEditorWrapper(Unity_Object_GBAIsometricWaypoint obj) {
                Obj = obj;
            }

            private Unity_Object_GBAIsometricWaypoint Obj { get; }

            public ushort Type { get; set; }

            public int DES { get; set; }

            public int ETA { get; set; }

            public byte Etat { get; set; }

            public byte SubEtat { get; set; }

            public int EtatLength => 0;
            public int SubEtatLength => 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }
    }
}