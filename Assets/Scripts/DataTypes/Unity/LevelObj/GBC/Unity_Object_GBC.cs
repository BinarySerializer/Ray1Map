using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_GBC : Unity_Object
    {
        public Unity_Object_GBC(GBC_Actor actor, Unity_ObjectManager objManager)
        {
            // Set properties
            Actor = actor;
            ObjManager = objManager;
        }

        public GBC_Actor Actor { get; }
        public Unity_ObjectManager ObjManager { get; }

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

        public override string DebugText => String.Empty;

        public override R1Serializable SerializableData => Actor;
        public override ILegacyEditorWrapper LegacyWrapper => null;

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links => Actor.Links.
            Where(link => link.ActorIndex > 0 && link.Byte_02 == 0).
            Select(link => link.ActorIndex - 1);

        public override string PrimaryName
        {
            get
            {
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

        public bool IsTrigger => Actor.ActorID == 0xFF;

        public override bool IsEditor => IsTrigger;
        public override ObjectType Type => IsTrigger ? ObjectType.Trigger : ObjectType.Object;

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => CurrentAnimation?.AnimSpeed.Value ?? 0;

        public override int? GetAnimIndex => -1;
        protected override int GetSpriteID => -1;
        public override IList<Sprite> Sprites => null;

        protected override bool IsUIStateArrayUpToDate => false;
        protected override void RecalculateUIStates() => UIStates = new UIState[0];
    }
}