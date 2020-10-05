using System;
using System.Collections.Generic;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBARRR : Unity_Object
    {
        public Unity_Object_GBARRR(GBARRR_Actor actor)
        {
            Actor = actor;
        }

        public GBARRR_Actor Actor { get; }

        public override short XPosition
        {
            get => Actor.XPosition;
            set => Actor.XPosition = value;
        }

        public override short YPosition
        {
            get => Actor.YPosition;
            set => Actor.YPosition = value;
        }

        public override string DebugText => String.Empty;

        public override ILegacyEditorWrapper LegacyWrapper { get; }
        public override string PrimaryName => String.Empty;
        public override string SecondaryName => String.Empty;
        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => 0;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => new Sprite[0];
        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];
    }
}