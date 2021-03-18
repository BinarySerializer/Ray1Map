using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public abstract class Unity_Object_BaseGBAVV : Unity_Object_3D
    {
        protected Unity_Object_BaseGBAVV(Unity_ObjectManager_GBAVV objManager)
        {
            ObjManager = objManager;
        }

        public Unity_ObjectManager_GBAVV ObjManager { get; }
        public override Vector3 Position
        {
            get => new Vector3(XPosition, YPosition, 0); 
            set
            {
                XPosition = (short)value.x;
                YPosition = (short)value.y;
            }
        }

        public override string DebugText => null;

        public Unity_ObjectManager_GBAVV.AnimSet AnimSet => ObjManager.AnimSets.ElementAtOrDefault(AnimSetIndices.Item1)?.ElementAtOrDefault(AnimSetIndices.Item2);
        public Unity_ObjectManager_GBAVV.AnimSet.Animation Animation
        {
            get
            {
                var anim = AnimSet?.Animations.ElementAtOrDefault(AnimIndex);

                if (anim?.AnimFrames.Length == 0)
                    return null;
                
                return anim;
            }
        }

        public override ILegacyEditorWrapper LegacyWrapper => new DummyLegacyEditorWrapper(this);

        public override ObjectType Type => AnimSetIndex == -1 ? ObjectType.Trigger : ObjectType.Object;
        public override bool IsEditor => AnimSetIndex == -1;

        public int ScriptIndex { get; set; } = -1;
        public GBAVV_Script Script => ObjManager.Scripts?.ElementAtOrDefault(ScriptIndex);
        public virtual GBAVV_Script DialogScript => null;
        public virtual bool ScriptHasDialog => false;
        public string ScriptName => DialogScript?.DisplayName ?? Script?.DisplayName;
        public string[] GetTranslatedScript
        {
            get
            {
                var animSets = ObjManager.Graphics?.FirstOrDefault()?.AnimSets;
                var list = Script?.TranslatedStringAll(animSets, ObjManager.LocPointerTable);

                DialogScript?.TranslatedStringAll(animSets, ObjManager.LocPointerTable, list);

                return list?.ToArray();
            }
        }

        public void SetAnimation(int graphics, int animSet, byte anim)
        {
            AnimSetIndices = (graphics, animSet);
            AnimIndex = anim;
        }

        private int _animSetIndex;
        private byte _animIndex;

        public int AnimSetIndex
        {
            get => _animSetIndex;
            set
            {
                if (AnimSetIndex == -1)
                    return;

                _animSetIndex = value;
                AnimIndex = 0;
                FreezeFrame = false;
            }
        }

        public (int, int) AnimSetIndices
        {
            get => AnimSetIndex == -1 ? (-1, -1) : ObjManager.AnimSetsLookupTable.ElementAtOrDefault(AnimSetIndex);
            set => AnimSetIndex = ObjManager.AnimSets.ElementAtOrDefault(value.Item1)?.ElementAtOrDefault(value.Item2)?.Index ?? -1;
        }

        public byte AnimIndex
        {
            get => _animIndex;
            set
            {
                _animIndex = value;
                FreezeFrame = false;
            }
        }
        
        public bool FreezeFrame { get; set; }

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => Animation?.AnimHitBox;

        public override Unity_ObjAnimation CurrentAnimation => Animation?.ObjAnimation;
        public override int AnimSpeed => FreezeFrame ? 0 : Animation?.CrashAnim?.GetAnimSpeed ?? 0;
        public override int? GetAnimIndex => AnimIndex;
        protected override int GetSpriteID => AnimSetIndex;
        public override IList<Sprite> Sprites => Animation?.AnimFrames;

        #region UI States

        protected int UIStates_AnimSetIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => AnimSetIndex == UIStates_AnimSetIndex;

        protected class GBAVV_UIState : UIState
        {
            public GBAVV_UIState(string displayName, byte animIndex) : base(displayName, animIndex) { }

            public override void Apply(Unity_Object obj)
            {
                ((Unity_Object_BaseGBAVV)obj).AnimIndex = (byte)AnimIndex;
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                return AnimIndex == ((Unity_Object_BaseGBAVV)obj).AnimIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_AnimSetIndex = AnimSetIndex;

            List<UIState> uiStates = new List<UIState>();

            for (byte i = 0; i < (AnimSet?.Animations?.Length ?? 0); i++)
                uiStates.Add(new GBAVV_UIState($"Animation {i}", animIndex: i));

            UIStates = uiStates.ToArray();
        }
        
        #endregion
    }
}