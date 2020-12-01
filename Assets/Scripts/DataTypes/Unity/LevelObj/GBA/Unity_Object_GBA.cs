using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBA : Unity_Object
    {
        public Unity_Object_GBA(GBA_Actor actor, Unity_ObjectManager_GBA objManager)
        {
            // Set properties
            Actor = actor;
            ObjManager = objManager;
            InitialXPos = actor.XPos;
            InitialYPos = actor.YPos;
        }

        public GBA_Actor Actor { get; }

        protected short InitialXPos { get; }
        protected short InitialYPos { get; }

        public Unity_ObjectManager_GBA ObjManager { get; }

        public GBA_Action State => GraphicsData?.States.ElementAtOrDefault(Actor.ActionIndex);
        public Unity_ObjectManager_GBA.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(GraphicsDataIndex);

        public override ObjectType Type => Actor.Type == GBA_Actor.ActorType.Waypoint || Actor.Type == GBA_Actor.ActorType.Captor ? ObjectType.Trigger : ObjectType.Object;

        public int GraphicsDataIndex
        {
            get => Actor.ActorModel == null ? -1 : ObjManager.GraphicsDataLookup.TryGetItem(Actor.ModelIndex, -1);
            set 
            {
                if (Actor.ActorModel == null)
                    return;

                if (value != GraphicsDataIndex) {
                    Actor.ActionIndex = 0;
                    OverrideAnimIndex = null;
                    Actor.ModelIndex = (byte)ObjManager.GraphicsDatas[value].Index;
                }
            }
        }


        public override short XPosition
        {
            get => Actor.XPos;
            set
            {
                var change = XPosition - value;
                Actor.XPos = value;

                Actor.BoxMinX = (short)(Actor.BoxMinX - change);
                Actor.BoxMaxX = (short)(Actor.BoxMaxX - change);
            }
        }

        public override short YPosition
        {
            get => Actor.YPos;
            set
            {
                var change = YPosition - value;
                Actor.YPos = value;
                Actor.BoxMinY = (short)(Actor.BoxMinY - change);
                Actor.BoxMaxY = (short)(Actor.BoxMaxY - change);
            }
        }

        public override string DebugText
        {
            get
            {
                var text = new StringBuilder();

                text.AppendLine($"{nameof(Actor.Type)}: {Actor.Type}");

                if (Actor.Type == GBA_Actor.ActorType.Unk)
                {
                    text.AppendLine($"{nameof(Actor.Index)}: {Actor.Index}");
                    text.AppendLine($"{nameof(Actor.Unk_01)}: {Actor.Unk_01}");
                    text.AppendLine($"{nameof(Actor.ActorSize)}: {Actor.ActorSize}");
                    if (Actor.ActorSize >= 2)
                    {
                        text.AppendLine($"{nameof(Actor.Byte_04)}: {Actor.Byte_04}");
                        text.AppendLine($"{nameof(Actor.ModelIndex)}: {Actor.ModelIndex}");
                    }
                    text.AppendLine($"{nameof(Actor.ExtraData)}: {BitConverter.ToString(Actor.ExtraData)}");
                }
                else
                {
                    if (Actor.Type != GBA_Actor.ActorType.Captor)
                    {
                        text.AppendLine($"{nameof(Actor.Byte_04)}: {Actor.Byte_04}");
                        text.AppendLine($"{nameof(Actor.ActorID)}: {Actor.ActorID}");

                        if (ObjManager.Context.Settings.EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow || Actor.Type == GBA_Actor.ActorType.Actor || Actor.Type == GBA_Actor.ActorType.AlwaysActor)
                        {
                            text.AppendLine($"{nameof(Actor.ModelIndex)}: {Actor.ModelIndex}");
                            text.AppendLine($"{nameof(Actor.ActionIndex)}: {Actor.ActionIndex}");
                        }

                        if (ObjManager.Context.Settings.EngineVersion > EngineVersion.GBA_BatmanVengeance && ObjManager.Context.Settings.EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow)
                        {
                            text.AppendLine($"{nameof(Actor.Link_0)}: {Actor.Link_0}");
                            text.AppendLine($"{nameof(Actor.Link_1)}: {Actor.Link_1}");
                            text.AppendLine($"{nameof(Actor.Link_2)}: {Actor.Link_2}");
                            text.AppendLine($"{nameof(Actor.Link_3)}: {Actor.Link_3}");
                        }

                        if (ObjManager.Context.Settings.EngineVersion >= EngineVersion.GBA_SplinterCell
                            && ObjManager.Context.Settings.EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow)
                        {
                            text.AppendLine($"{nameof(Actor.Short_0C)}: {Actor.Short_0C}");
                            text.AppendLine($"{nameof(Actor.Short_0E)}: {Actor.Short_0E}");
                            text.AppendLine($"{nameof(Actor.ExtraData)}: {BitConverter.ToString(Actor.ExtraData)}");
                        }
                        else if (ObjManager.Context.Settings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow)
                        {
                            if (Actor.Type == GBA_Actor.ActorType.Waypoint || Actor.Type == GBA_Actor.ActorType.Unk)
                            {
                                text.AppendLine($"{nameof(Actor.ActorSize)}: {Actor.ActorSize}");
                                text.AppendLine($"{nameof(Actor.ExtraData)}: {BitConverter.ToString(Actor.ExtraData)}");
                            }
                            else
                            {
                                text.AppendLine($"{nameof(Actor.Short_0C)}: {Actor.Short_0C}");
                                text.AppendLine($"{nameof(Actor.ActorSize)}: {Actor.ActorSize}");
                                text.AppendLine($"{nameof(Actor.ExtraData)}: {BitConverter.ToString(Actor.ExtraData)}");
                            }
                        }
                    }
                    else
                    {
                        text.AppendLine($"{nameof(Actor.Byte_04)}: {Actor.Byte_04}");
                        text.AppendLine($"{nameof(Actor.CaptorID)}: {Actor.CaptorID}");
                        text.AppendLine($"{nameof(Actor.LinkedActorsCount)}: {Actor.LinkedActorsCount}");
                        
                        if (ObjManager.Context.Settings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia)
                            text.AppendLine($"{nameof(Actor.UnkData1)}: {BitConverter.ToString(Actor.UnkData1)}");

                        text.AppendLine($"{nameof(Actor.CaptorDataOffsetIndex)}: {Actor.CaptorDataOffsetIndex}");

                        if (ObjManager.Context.Settings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow)
                        {
                            text.AppendLine($"{nameof(Actor.UnkData2)}: {BitConverter.ToString(Actor.UnkData2)}");
                            text.AppendLine($"{nameof(Actor.ActorSize)}: {Actor.ActorSize}");
                        }
                        else
                        {
                            text.AppendLine($"{nameof(Actor.UnkData2)}: {BitConverter.ToString(Actor.UnkData2)}");
                        }
                        text.AppendLine($"{nameof(Actor.BoxMinY)}: {Actor.BoxMinY}");
                        text.AppendLine($"{nameof(Actor.BoxMinX)}: {Actor.BoxMinX}");
                        text.AppendLine($"{nameof(Actor.BoxMaxY)}: {Actor.BoxMaxY}");
                        text.AppendLine($"{nameof(Actor.BoxMaxX)}: {Actor.BoxMaxX}");

                        if (ObjManager.Context.Settings.EngineVersion >= EngineVersion.GBA_SplinterCellPandoraTomorrow)
                            text.AppendLine($"{nameof(Actor.ExtraData)}: {BitConverter.ToString(Actor.ExtraData)}");

                        for (int i = 0; i < Actor.CaptorData.DataEntries.Length; i++)
                        {
                            text.AppendLine($"{nameof(Actor.CaptorData.DataEntries)}[{i}].{nameof(GBA_CaptorDataEntry.UShort_00)}: {Actor.CaptorData.DataEntries[i].UShort_00}");
                            text.AppendLine($"{nameof(Actor.CaptorData.DataEntries)}[{i}].{nameof(GBA_CaptorDataEntry.LinkedActor)}: {Actor.CaptorData.DataEntries[i].LinkedActor}");
                            text.AppendLine($"{nameof(Actor.CaptorData.DataEntries)}[{i}].{nameof(GBA_CaptorDataEntry.Byte_03)}: {Actor.CaptorData.DataEntries[i].Byte_03}");
                            text.AppendLine($"{nameof(Actor.CaptorData.DataEntries)}[{i}].{nameof(GBA_CaptorDataEntry.UShort_04)}: {Actor.CaptorData.DataEntries[i].UShort_04}");
                        }
                    }
                }

                return text.ToString();
            }
        }

        public override IEnumerable<int> Links
        {
            get
            {
                if (Actor.Link_0 != 0xFF)
                    yield return Actor.Link_0;

                if (Actor.Link_1 != 0xFF)
                    yield return Actor.Link_1;

                if (Actor.Link_2 != 0xFF)
                    yield return Actor.Link_2;

                if (Actor.Link_3 != 0xFF)
                    yield return Actor.Link_3;

                if (Actor.CaptorData != null)
                    foreach (var d in Actor.CaptorData.DataEntries)
                    {
                        if (d.Type == 0)
                        {
                            yield return d.LinkedActor;
                        }
                    }
            }
        }

        public override R1Serializable SerializableData => Actor;

        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override bool IsAlways => Actor.Type == GBA_Actor.ActorType.AlwaysActor || Actor.Type == GBA_Actor.ActorType.MainActor;
        public override bool IsEditor => Actor.Type == GBA_Actor.ActorType.Captor || Actor.Type == GBA_Actor.ActorType.Waypoint;
        public override bool CanBeLinked => true;

        public override string PrimaryName
        {
            get
            {

                if (Actor.Type == GBA_Actor.ActorType.Unk)
                    return null;
                if (Actor.Type == GBA_Actor.ActorType.Captor) {
                    return $"BOX_{(int)Actor.CaptorID}";
                }

                return $"ID_{Actor.ActorID}";
            }
        }
        public override string SecondaryName
        {
            get
            {
                if (Actor.Type == GBA_Actor.ActorType.Captor)
                    return $"Trigger ({Actor.CaptorID})";

                if (ObjManager.Context.Settings.Game == Game.GBA_Rayman3)
                    return $"{(GBA_R3_ActorID) Actor.ActorID}";

                return null;
            }
        }

        public override bool FlipHorizontally => State?.Flags.HasFlag(GBA_Action.ActorStateFlags.HorizontalFlip) ?? false;
        public override bool FlipVertically => State?.Flags.HasFlag(GBA_Action.ActorStateFlags.VerticalFlip) ?? false;

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => Actor.Type == GBA_Actor.ActorType.Captor ? new Unity_ObjAnimationCollisionPart[]
        {
            new Unity_ObjAnimationCollisionPart()
            {
                XPosition = Actor.BoxMinX - XPosition,
                YPosition = Actor.BoxMinY - YPosition,
                Width = Actor.BoxMaxX - Actor.BoxMinX,
                Height = Actor.BoxMaxY - Actor.BoxMinY,
                Type = Actor.CaptorID == GBA_Actor.CaptorType.Player ? Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox : Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox
            }
        } : new Unity_ObjAnimationCollisionPart[0];

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Graphics.Animations.ElementAtOrDefault(AnimationIndex ?? -1);
        public override int AnimSpeed => CurrentAnimation?.AnimSpeed.Value ?? 0;

        public override int? GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex ?? Actor.ActionIndex;
        protected override int GetSpriteID => GraphicsDataIndex;
        public override IList<Sprite> Sprites => GraphicsData?.Graphics.Sprites;

        private class LegacyEditorWrapper : ILegacyEditorWrapper
        {
            public LegacyEditorWrapper(Unity_Object_GBA obj)
            {
                Obj = obj;
            }

            private Unity_Object_GBA Obj { get; }

            public ushort Type
            {
                get => Obj.Actor.ActorID;
                set => Obj.Actor.ActorID = (byte)value;
            }

            public int DES
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public int ETA
            {
                get => Obj.GraphicsDataIndex;
                set => Obj.GraphicsDataIndex = value;
            }

            public byte Etat { get; set; }

            public byte SubEtat
            {
                get => Obj.Actor.ActionIndex;
                set => Obj.Actor.ActionIndex = value;
            }

            public int EtatLength => 0;
            public int SubEtatLength => Obj.GraphicsData?.States?.Length > 0 ? Obj.GraphicsData?.States?.Length ?? 0 : Obj.GraphicsData?.Graphics.Animations.Count ?? 0;

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
        protected override bool IsUIStateArrayUpToDate => GraphicsDataIndex == UIStates_GraphicsDataIndex;

        protected class GBA_UIState : UIState {
            public GBA_UIState(string displayName, byte stateIndex) : base(displayName) {
                StateIndex = stateIndex;
            }
            public GBA_UIState(string displayName, int animIndex) : base(displayName, animIndex) {}

            public byte StateIndex { get; }

            public override void Apply(Unity_Object obj) {
                if (IsState) {
                    (obj as Unity_Object_GBA).Actor.ActionIndex = StateIndex;
                    obj.OverrideAnimIndex = null;
                } else {
                    obj.OverrideAnimIndex = AnimIndex;
                }
            }

            public override bool IsCurrentState(Unity_Object obj) {

                if (obj.OverrideAnimIndex.HasValue)
                    return !IsState && AnimIndex == obj.OverrideAnimIndex;
                else
                    return IsState && StateIndex == (obj as Unity_Object_GBA).Actor.ActionIndex;
            }
        }

        protected override void RecalculateUIStates() {
            UIStates_GraphicsDataIndex = GraphicsDataIndex;
            var states = GraphicsData?.States;
            var anims = GraphicsData?.Graphics.Animations;
            HashSet<int> usedAnims = new HashSet<int>();
            List<UIState> uiStates = new List<UIState>();
            if (states != null) {
                for (byte i = 0; i < states.Length; i++) {
                    uiStates.Add(new GBA_UIState("State " + i, stateIndex: i));
                    usedAnims.Add(states[i].AnimationIndex);
                }
            }
            if (anims != null) {
                for (int i = 0; i < anims.Count; i++) {
                    if (usedAnims.Contains(i)) continue;
                    uiStates.Add(new GBA_UIState("Animation " + i, animIndex: i));
                }
            }

            UIStates = uiStates.ToArray();
        }
		#endregion
	}
}