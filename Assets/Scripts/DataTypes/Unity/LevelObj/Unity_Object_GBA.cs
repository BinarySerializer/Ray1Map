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

        public GBA_ActorState State => GraphicsData?.States.ElementAtOrDefault(Actor.StateIndex);
        public Unity_ObjectManager_GBA.GraphicsData GraphicsData => ObjManager.GraphicsDatas.ElementAtOrDefault(GraphicsDataIndex);
        
        public int GraphicsDataIndex
        {
            get => Actor.GraphicData == null ? -1 : ObjManager.GraphicsDatas.FindItemIndex(x => x.Index == Actor.GraphicsDataIndex);
            set 
            {
                if (Actor.GraphicData == null)
                    return;

                OverrideAnimIndex = null;
                Actor.GraphicsDataIndex = (byte)ObjManager.GraphicsDatas[value].Index;
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
                        text.AppendLine($"{nameof(Actor.GraphicsDataIndex)}: {Actor.GraphicsDataIndex}");
                    }
                    text.AppendLine($"{nameof(Actor.ExtraData)}: {BitConverter.ToString(Actor.ExtraData)}");
                }
                else
                {
                    if (Actor.Type != GBA_Actor.ActorType.BoxTrigger)
                    {
                        text.AppendLine($"{nameof(Actor.Byte_04)}: {Actor.Byte_04}");
                        text.AppendLine($"{nameof(Actor.ActorID)}: {Actor.ActorID}");

                        if (ObjManager.Context.Settings.EngineVersion < EngineVersion.GBA_SplinterCellPandoraTomorrow || Actor.Type == GBA_Actor.ActorType.Normal || Actor.Type == GBA_Actor.ActorType.Always)
                        {
                            text.AppendLine($"{nameof(Actor.GraphicsDataIndex)}: {Actor.GraphicsDataIndex}");
                            text.AppendLine($"{nameof(Actor.StateIndex)}: {Actor.StateIndex}");
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
                            if (Actor.Type == GBA_Actor.ActorType.Trigger || Actor.Type == GBA_Actor.ActorType.Unk)
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
                        text.AppendLine($"{nameof(Actor.BoxActorID)}: {Actor.BoxActorID}");
                        text.AppendLine($"{nameof(Actor.LinkedActorsCount)}: {Actor.LinkedActorsCount}");
                        
                        if (ObjManager.Context.Settings.EngineVersion >= EngineVersion.GBA_PrinceOfPersia)
                            text.AppendLine($"{nameof(Actor.UnkData1)}: {BitConverter.ToString(Actor.UnkData1)}");

                        text.AppendLine($"{nameof(Actor.BoxActorBlockOffsetIndex)}: {Actor.BoxActorBlockOffsetIndex}");

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

                        for (int i = 0; i < Actor.BoxActorBlock.Data.Length; i++)
                        {
                            text.AppendLine($"{nameof(Actor.BoxActorBlock.Data)}[{i}].{nameof(GBA_BoxTriggerActorData.UShort_00)}: {Actor.BoxActorBlock.Data[i].UShort_00}");
                            text.AppendLine($"{nameof(Actor.BoxActorBlock.Data)}[{i}].{nameof(GBA_BoxTriggerActorData.LinkedActor)}: {Actor.BoxActorBlock.Data[i].LinkedActor}");
                            text.AppendLine($"{nameof(Actor.BoxActorBlock.Data)}[{i}].{nameof(GBA_BoxTriggerActorData.Byte_03)}: {Actor.BoxActorBlock.Data[i].Byte_03}");
                            text.AppendLine($"{nameof(Actor.BoxActorBlock.Data)}[{i}].{nameof(GBA_BoxTriggerActorData.UShort_04)}: {Actor.BoxActorBlock.Data[i].UShort_04}");
                        }
                    }
                }

                return text.ToString();
            }
        }

        [Obsolete]
        public override ILegacyEditorWrapper LegacyWrapper => new LegacyEditorWrapper(this);

        public override bool IsAlways => Actor.Type == GBA_Actor.ActorType.Always || Actor.Type == GBA_Actor.ActorType.Main;
        public override bool IsEditor => Actor.Type == GBA_Actor.ActorType.BoxTrigger || Actor.Type == GBA_Actor.ActorType.Trigger;
        public override string PrimaryName => $"ID_{Actor.ActorID}";
        public override string SecondaryName => ObjManager.Context.Settings.Game == Game.GBA_Rayman3 ? $"{(GBA_R3_ActorID)Actor.ActorID}" : null;

        public override bool FlipHorizontally => State?.Flags.HasFlag(GBA_ActorState.ActorStateFlags.HorizontalFlip) ?? false;
        public override bool FlipVertically => State?.Flags.HasFlag(GBA_ActorState.ActorStateFlags.VerticalFlip) ?? false;

        public override Unity_ObjAnimationCollisionPart[] ObjCollision => Actor.Type == GBA_Actor.ActorType.BoxTrigger ? new Unity_ObjAnimationCollisionPart[]
        {
            new Unity_ObjAnimationCollisionPart()
            {
                XPosition = Actor.BoxMinX - XPosition,
                YPosition = Actor.BoxMinY - YPosition,
                Width = Actor.BoxMaxX - Actor.BoxMinX,
                Height = Actor.BoxMaxY - Actor.BoxMinY,
                Type = Actor.BoxActorID == GBA_Actor.BoxActorType.Player ? Unity_ObjAnimationCollisionPart.CollisionType.TriggerBox : Unity_ObjAnimationCollisionPart.CollisionType.HitTriggerBox
            }
        } : new Unity_ObjAnimationCollisionPart[0];

        public override Unity_ObjAnimation CurrentAnimation => GraphicsData?.Graphics.Animations.ElementAtOrDefault(AnimationIndex);
        public override byte AnimSpeed => CurrentAnimation?.AnimSpeed.Value ?? 0;

        public override byte GetAnimIndex => OverrideAnimIndex ?? State?.AnimationIndex ?? Actor.StateIndex;
        public override IList<Sprite> Sprites => GraphicsData?.Graphics.Sprites;

		public override string[] UIStateNames {
            get {
                var states = GraphicsData?.States;
                var anims = GraphicsData?.Graphics.Animations;
                HashSet<int> usedAnims = new HashSet<int>();
                List<string> stateNames = new List<string>();
                if (states != null) {
                    for (int i = 0; i < states.Length; i++) {
                        stateNames.Add("State " + i);
                        usedAnims.Add(states[i].AnimationIndex);
                    }
                }
                if (anims != null) {
                    for (int i = 0; i < anims.Count; i++) {
                        if (usedAnims.Contains(i)) continue;
                        stateNames.Add("(Unused) Animation " + i);
                    }
                }
                return stateNames.ToArray();
            }
        }

		public override int CurrentUIState {
            get {
                if (OverrideAnimIndex.HasValue) {
                    var states = GraphicsData?.States;
                    var anims = GraphicsData?.Graphics.Animations;
                    HashSet<int> usedAnims = new HashSet<int>();
                    int currentState = states?.Length ?? 0;
                    if (states != null) {
                        for (int i = 0; i < states.Length; i++) {
                            usedAnims.Add(states[i].AnimationIndex);
                        }
                    }
                    if (anims != null) {
                        for (int i = 0; i < anims.Count; i++) {
                            if (usedAnims.Contains(i)) continue;
                            if (i == OverrideAnimIndex) return currentState;
                            currentState++;
                        }
                    }
                    return currentState;
                } else {
                    var states = GraphicsData?.States;
                    if (Actor.StateIndex >= 0 && Actor.StateIndex < states.Length) return Actor.StateIndex;
                    return 0;
                }
            }
            set {
                if (value != CurrentUIState) {
                    var states = GraphicsData?.States;
                    if (value < states.Length) {
                        Actor.StateIndex = (byte)value;
                        OverrideAnimIndex = null;
                    } else if(GraphicsData?.Graphics.Animations.Count > 0) {
                        int currentState = states?.Length ?? 0;
                        HashSet<int> usedAnims = new HashSet<int>();
                        if (states != null) {
                            for (int i = 0; i < states.Length; i++) {
                                usedAnims.Add(states[i].AnimationIndex);
                            }
                        }
                        var anims = GraphicsData?.Graphics.Animations;
                        if (anims != null) {
                            for (int i = 0; i < anims.Count; i++) {
                                if (usedAnims.Contains(i)) continue;
                                if (currentState == value) {
                                    OverrideAnimIndex = (byte)i;
                                    return;
                                }
                                currentState++;
                            }
                        }
                    }
                }
            }
        }

		[Obsolete]
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
                get => Obj.Actor.StateIndex;
                set => Obj.Actor.StateIndex = value;
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
    }
}