using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBC : Unity_Object
    {
        public Unity_Object_GBC(GBC_GameObject actor, Unity_ObjectManager_GBC objManager)
        {
            // Set properties
            Actor = actor;
            ObjManager = objManager;

            // Uncomment for screenshot enumeration to show Rayman
            //if ((objManager.Context.Settings.Game == Game.GBC_R1 || objManager.Context.Settings.Game == Game.GBC_R2) && Actor.ActorID == 0)
            //    ActionIndex = 0;
        }

        public GBC_GameObject Actor { get; }
        public Unity_ObjectManager_GBC ObjManager { get; }

        public GBC_Action Action => ActorModel?.Actions.ElementAtOrDefault(ActionIndex);
        public Unity_ObjectManager_GBC.ActorModel ActorModel => ObjManager.ActorModels.ElementAtOrDefault(ActorModelIndex);

        public int ActorModelIndex
        {
            get => Actor.ActorModel == null && IsGBC || Actor.GBA_ActorModel == null && !IsGBC ? -1 : ObjManager.ActorModelsLookup.TryGetItem(Actor.Index_ActorModel, -1);
            set
            {
                if (Actor.ActorModel == null && IsGBC || Actor.GBA_ActorModel == null && !IsGBC)
                    return;

                if (value != ActorModelIndex)
                {
                    ActionIndex = 0;
                    OverrideAnimIndex = null;
                    Actor.Index_ActorModel = (byte)ObjManager.ActorModels[value].Index;
                }
            }
        }

        public int ActionIndex
        {
            get => ActorModel?.ActionsLookup.TryGetItem(Actor.ActionID, 0) ?? 0;
            set => Actor.ActionID = ActorModel?.Actions[value].ActionID ?? 0;
        }

        public override short XPosition
        {
            get => Actor.XPos;
            set => Actor.XPos = value;
        }

        public override short YPosition
        {
            get => Actor.YPos;
            set => Actor.YPos = value;
        }

        public override string DebugText => $"ActionIndex: {ActionIndex}{Environment.NewLine}" +
                                            $"ActionID: {Actor.ActionID}{Environment.NewLine}";

        public override R1Serializable SerializableData => Actor;
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links => Actor.Links
            .Where(link => link.Type == GBC_GameObjectLink.MessageType.GameObject && link.GameObjectID.HasValue && link.GameObjectID.Value != Actor.XlateID)
            .Select(link => link.GameObjectID.Value - 1);

		public override IEnumerable<LinkType> LinkTypes => Actor.Links
            .Where(link => link.Type == GBC_GameObjectLink.MessageType.GameObject && link.GameObjectID.HasValue && link.GameObjectID.Value != Actor.XlateID)
            .Select(GetLinkType);

        private LinkType GetLinkType(GBC_GameObjectLink link) {
            switch (link.MessageID) {
                case 1: return LinkType.WakeUp;
                case 2: return LinkType.Sleep;
                case 3: return LinkType.Destroy;
                case 4: return LinkType.Reset;
                case 5: return LinkType.ResetWakeUp;
                default: return LinkType.Unknown;
            }
        }

        public override string PrimaryName
        {
            get
            {
                if (Actor.IsCaptor)
                    return $"Captor";

                if (ObjManager.Context.Settings.Game == Game.GBC_R1)
                {
                    var actorName = ((GBC_R1_ActorID)Actor.ActorID).ToString();

                    if (!actorName.Contains("NULL") && Enum.IsDefined(typeof(GBC_R1_ActorID), (GBC_R1_ActorID)Actor.ActorID))
                        return actorName;
                }

                return $"ID_{Actor.ActorID}";
            }
        }

        public override string SecondaryName => null;

        public bool IsTrigger => Actor.IsCaptor;

        private Unity_ObjAnimationCollisionPart[] objCollision;
        public override Unity_ObjAnimationCollisionPart[] ObjCollision {
            get {
                if (objCollision == null) {
                    objCollision = Actor.IsCaptor ? new Unity_ObjAnimationCollisionPart[] {
                        new Unity_ObjAnimationCollisionPart
                        {
                            XPosition = -(Actor.BoxWidth / 2),
                            YPosition = -(Actor.BoxHeight / 2),
                            Width = Actor.BoxWidth,
                            Height = Actor.BoxHeight,
                            Type = Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox
                        }
                    } : new Unity_ObjAnimationCollisionPart[0];
                }
                return objCollision;
            }
        }

        public override bool IsEditor => IsTrigger;
        public override ObjectType Type => IsTrigger ? ObjectType.Trigger : ObjectType.Object;
		public override bool IsAlways => !Actor.IsCaptor && Actor.ActionID == 0;

		public override Unity_ObjAnimation CurrentAnimation => ActorModel?.Graphics?.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => CurrentAnimation?.AnimSpeeds?.ElementAtOrDefault(AnimationFrame) ?? CurrentAnimation?.AnimSpeed ?? 0;

        public override int? GetAnimIndex => OverrideAnimIndex - (IsGBC ? 1 : 0) ?? Action?.AnimIndex - (IsGBC ? 1 : 0) ?? ActionIndex;
        protected override int GetSpriteID => ActorModelIndex;
        public override IList<Sprite> Sprites => ActorModel?.Graphics?.Sprites;

        public bool IsGBC => ObjManager.Context.Settings.MajorEngineVersion == MajorEngineVersion.GBC;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBC obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBC Obj { get; }

            public ushort Type
            {
                get => Obj.Actor.IsCaptor ? (ushort)0 : (ushort)Obj.Actor.ActorID;
                set
                {
                    if (!Obj.Actor.IsCaptor)
                        Obj.Actor.ActorID = (byte) value;
                }
            }

            public int DES
            {
                get => Obj.ActorModelIndex;
                set => Obj.ActorModelIndex = value;
            }

            public int ETA
            {
                get => Obj.ActorModelIndex;
                set => Obj.ActorModelIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => (byte)Obj.ActionIndex;
                set => Obj.ActionIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.ActorModel?.Actions?.Length > 0 ? Obj.ActorModel?.Actions?.Length ?? 0 : Obj.ActorModel?.Graphics?.Animations.Count ?? 0;

            public byte OffsetBX { get; set; }

            public byte OffsetBY { get; set; }

            public byte OffsetHY { get; set; }

            public byte FollowSprite { get; set; }

            public uint HitPoints { get; set; }

            public byte HitSprite { get; set; }

            public bool FollowEnabled { get; set; }
        }

        #region UI States
        protected int UIStates_GraphicsDataIndex { get; set; } = -2;
        protected override bool IsUIStateArrayUpToDate => ActorModelIndex == UIStates_GraphicsDataIndex;

        protected class GBC_UIState : UIState
        {
            public GBC_UIState(string displayName, byte stateIndex) : base(displayName)
            {
                StateIndex = stateIndex;
            }
            public GBC_UIState(string displayName, int animIndex) : base(displayName, animIndex) { }

            public byte StateIndex { get; }

            public override void Apply(Unity_Object obj)
            {
                if (IsState)
                {
                    (obj as Unity_Object_GBC).ActionIndex = StateIndex;
                    obj.OverrideAnimIndex = null;
                }
                else
                {
                    obj.OverrideAnimIndex = AnimIndex;
                }
            }

            public override bool IsCurrentState(Unity_Object obj)
            {
                if (obj.OverrideAnimIndex.HasValue)
                    return !IsState && AnimIndex == obj.OverrideAnimIndex;
                else
                    return IsState && StateIndex == (obj as Unity_Object_GBC).ActionIndex;
            }
        }

        protected override void RecalculateUIStates()
        {
            UIStates_GraphicsDataIndex = ActorModelIndex;
            var actions = ActorModel?.Actions;
            var anims = ActorModel?.Graphics?.Animations;
            HashSet<int> usedAnims = new HashSet<int>();
            List<UIState> uiStates = new List<UIState>();
            if (actions != null)
            {
                for (byte i = 0; i < actions.Length; i++)
                {
                    uiStates.Add(new GBC_UIState($"Action {actions[i].ActionID}", stateIndex: i));
                    usedAnims.Add(actions[i].AnimIndex - (IsGBC ? 1 : 0));
                }
            }
            if (anims != null)
            {
                for (int i = 0; i < anims.Count; i++)
                {
                    if (usedAnims.Contains(i)) continue;
                    uiStates.Add(new GBC_UIState("Animation " + i, animIndex: i + (IsGBC ? 1 : 0)));
                }
            }

            UIStates = uiStates.ToArray();
        }
        #endregion
    }
}