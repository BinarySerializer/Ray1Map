using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GameloftRRR : Unity_Object
    {
        public Unity_Object_GameloftRRR(Unity_ObjectManager objManager, Gameloft_RRR_Objects.Object obj)
        {
            ObjManager = objManager;
            Object = obj;
        }

        public Unity_ObjectManager ObjManager { get; }
        public Gameloft_RRR_Objects.Object Object { get; set; }

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

        public override string DebugText =>
            $"AnimIndex: {Object.AnimationIndex}{Environment.NewLine}" +
            $"Unknown: {Object.Unknown}{Environment.NewLine}" +
            $"Flags: {Object.Flags}{Environment.NewLine}" +
            $"Params: {string.Join(", ",Object.Shorts.Select(s => $"{s}"))}";


        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => null;//new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{Object.Type}";
        public override string SecondaryName
        {
            get
            {
                return null;
            }
        }

        public override bool FlipHorizontally => (Object.Flags & 1) == 1;
        public override bool FlipVertically => false;

        public override bool CanBeLinkedToGroup => true;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => null;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;
        public int AnimIndex { get; set; }


        #region UI States

        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => 0 == UIStates_AnimSetIndex;

        protected class GameloftRRR_UIState : UIState
        {
            public GameloftRRR_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_GameloftRRR)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_GameloftRRR)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = 0;

            List<UIState> uiStates = new List<UIState>();

            /*for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAVV_UIState($"Animation {i}", animIndex: i));*/
            uiStates.Add(new GameloftRRR_UIState("Yeah", 0));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}