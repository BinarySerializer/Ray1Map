using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBAIsometric : Unity_Object
    {
        public Unity_Object_GBAIsometric(GBAIsometric_Object obj, Unity_ObjectManager objManager)
        {
            Object = obj;
            ObjManager = objManager;
        }

        public GBAIsometric_Object Object { get; }
        public Unity_ObjectManager ObjManager { get; }

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

        public override string DebugText => String.Empty;

        public override ILegacyEditorWrapper LegacyWrapper { get; }

        public override string PrimaryName => $"Object ({XPosition}, {YPosition})";
        public override string SecondaryName => $"Object ({XPosition}, {YPosition})";

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links
        {
            get
            {
                if (Object.LinkIndex != 0xFF)
                    yield return Object.LinkIndex;
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