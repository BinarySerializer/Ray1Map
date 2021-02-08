using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GameloftRRR : Unity_Object
    {
        public Unity_Object_GameloftRRR(Unity_ObjectManager objManager, Gameloft_Objects.Object obj, int objGroupIndex, int objIndex)
        {
            ObjManager = objManager;
            Object = obj;
        }

        public Unity_ObjectManager ObjManager { get; }
        public Gameloft_Objects.Object Object { get; set; }

        public bool IsLinked_4 { get; set; }
        public bool IsLinked_6 { get; set; }

        public override short XPosition
        {
            get => Object.Shorts[1];
            set => Object.Shorts[1] = value;
        }

        public override short YPosition
        {
            get => Object.Shorts[2];
            set => Object.Shorts[2] = value;
        }

        public override string DebugText => $"Params: {string.Join(", ",Object.Shorts.Skip(3).Select((s, i) => $"{i}: {s}"))}";


        public override R1Serializable SerializableData => Object;
        public override ILegacyEditorWrapper LegacyWrapper => null;//new LegacyEditorWrapper(this);

        public override string PrimaryName => $"Type_{(int)Object.Shorts[0]}";
        public override string SecondaryName
        {
            get
            {
                return null;
            }
        }

        public override bool FlipHorizontally => false;
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