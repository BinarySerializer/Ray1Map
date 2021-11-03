using System.Collections.Generic;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map
{
    public class Unity_Object_Dummy : Unity_SpriteObject_3D
    {
        public Unity_Object_Dummy(BinarySerializable serializableData, Unity_ObjectType type, string debugText = null, int[] objLinks = null)
        {
            SerializableData = serializableData;
            Type = type;
            DebugText = debugText;
            ObjLinks = objLinks;
        }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override Vector3 Position { get; set; }
        public override BinarySerializable SerializableData { get; }
        public override string DebugText { get; }
        public override Unity_ObjectType Type { get; }
        public override string PrimaryName => $"DUMMY";

        public int[] ObjLinks { get; }

        public override IEnumerable<int> Links => ObjLinks;
        public override bool CanBeLinked => ObjLinks != null;

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