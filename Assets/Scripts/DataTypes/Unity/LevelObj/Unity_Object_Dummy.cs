using System.Collections.Generic;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Unity_Object_Dummy : Unity_Object_3D
    {
        public Unity_Object_Dummy(BinarySerializable serializableData, ObjectType type)
        {
            SerializableData = serializableData;
            Type = type;
        }

        public override short XPosition { get; set; }
        public override short YPosition { get; set; }
        public override Vector3 Position { get; set; }
        public override ILegacyEditorWrapper LegacyWrapper => null;
        public override BinarySerializable SerializableData { get; }
        public override ObjectType Type { get; }
        public override string PrimaryName => $"DUMMY";
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