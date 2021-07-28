using BinarySerializer;
using BinarySerializer.KlonoaDTP;
using UnityEngine;

namespace R1Engine
{
    public sealed class Unity_Object_PS1Klonoa_Enemy : Unity_Object_BasePS1Klonoa
    {
        public Unity_Object_PS1Klonoa_Enemy(Unity_ObjectManager_PS1Klonoa objManager, EnemyObject obj, float scale) : base(objManager)
        {
            Object = obj;
            SpriteSetIndex = objManager.SpriteSets.FindItemIndex(x => x.Index == obj.GraphicsIndex - 1);

            if (SpriteSetIndex == -1)
                Debug.LogWarning($"Enemy graphics was not set");

            Position = PS1Klonoa_Manager.GetPosition(obj.XPos.Value, obj.YPos.Value, obj.ZPos.Value, scale);
        }

        public EnemyObject Object { get; set; }
        public override BinarySerializable SerializableData => Object;

        public override PrimaryObjectType PrimaryType => Object.PrimaryType;
        public override int SecondaryType => Object.SecondaryType;
    }
}