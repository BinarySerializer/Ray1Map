using System.Collections.Generic;
using UnityEngine;
using Light = PsychoPortal.Light;

namespace Ray1Map.Psychonauts
{
    public sealed class Unity_Object_Psychonauts_Light : Unity_SpriteObject_3D
    {
        public Unity_Object_Psychonauts_Light(Light light, float scale)
        {
            Light = light;
            Position = light.Position.ToInvVector3() * scale;
        }

        public Light Light { get; }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override Vector3 Position { get; set; }
        //public override string DebugText { get; }
        public override Unity_ObjectType Type => Unity_ObjectType.Waypoint;
        public override string PrimaryName => $"Light: {Light.Name} ({Light.Type})";

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