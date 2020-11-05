using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometric : Unity_Object
    {
        public Unity_Object_GBAIsometric(GBAIsometric_Object obj, Unity_ObjectManager_GBAIsometric objManager)
        {
            Object = obj;
            ObjManager = objManager;
        }

        public GBAIsometric_Object Object { get; }
        public Unity_ObjectManager_GBAIsometric ObjManager { get; }

        public override short XPosition
        {
            get => Object.XPosition;
            set => Object.XPosition = value;
        }
        public override short YPosition
        {
            get => Object.YPosition;
            set => Object.YPosition = value;
        }

        public override string DebugText => $"AnimSet: {AnimGroupName}{Environment.NewLine}";

        public string AnimGroupName => ObjManager.Types?.ElementAtOrDefault(Object.ObjectType)?.DataPointer?.Value?.AnimSetPointer?.Value?.Name;

        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper { get; }

        public override string PrimaryName => $"{AnimGroupName?.Replace("AnimSet", String.Empty) ?? $"Type_{Object.ObjectType}"}";
        public override string SecondaryName => null;

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links
        {
            get
            {
                // Normal link
                if (Object.LinkIndex != 0xFF)
                    yield return Object.LinkIndex;

                // Waypoint links
                for (int i = 0; i < Object.WaypointCount; i++)
                    yield return ObjManager.WaypointsStartIndex + Object.WaypointIndex + i;
            }
        }

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;
        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];
    }
}