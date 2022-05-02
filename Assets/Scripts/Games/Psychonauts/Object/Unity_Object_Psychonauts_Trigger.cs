using System.Collections.Generic;
using PsychoPortal;
using PsychoPortal.Unity;
using UnityEngine;

namespace Ray1Map.Psychonauts
{
    public sealed class Unity_Object_Psychonauts_Trigger : Unity_SpriteObject_3D
    {
        public Unity_Object_Psychonauts_Trigger(TriggerOBB trigger, float scale)
        {
            Trigger = trigger;
            Position = trigger.Position.ToInvVector3() * scale;

            Vector3 size = Trigger.Bounds.Max.ToVector3() - Trigger.Bounds.Min.ToVector3();
            DetectionCube = new DetectionCubeData(Trigger.Bounds.Min.ToVector3() + (size / 2), Trigger.Rotation.ToQuaternionRad(), size * scale);
        }

        public TriggerOBB Trigger { get; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override Vector3 Position { get; set; }
        //public override string DebugText { get; }
        public override Unity_ObjectType Type => Unity_ObjectType.Trigger;
        public override string PrimaryName => $"Trigger: {Trigger.TriggerName}" + (Trigger.IsPlane ? " (Plane)" : "");

        public override DetectionCubeData DetectionCube { get; }

        public override Unity_ObjAnimation CurrentAnimation => null;
        public override int AnimSpeed => 0;
        public override int? GetAnimIndex => null;
        protected override int GetSpriteID => 0;
        public override IList<Sprite> Sprites => null;

        private bool _isUIStateArrayUpToDate;

        protected override bool IsUIStateArrayUpToDate => _isUIStateArrayUpToDate;

        protected override void RecalculateUIStates()
        {
            UIStates = new UIState[0];
            _isUIStateArrayUpToDate = true;
        }
    }
}