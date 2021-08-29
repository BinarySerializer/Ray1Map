using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace R1Engine
{
    public sealed class Unity_Object_PSKlonoa_DTP_Enemy : Unity_Object_BasePSKlonoa_DTP
    {
        public Unity_Object_PSKlonoa_DTP_Enemy(Unity_ObjectManager_PSKlonoa_DTP objManager, EnemyObject obj, float scale, PSKlonoa_DTP_BaseManager.ObjSpriteInfo spriteInfo) : base(objManager)
        {
            Object = obj;
            Position = PSKlonoa_DTP_BaseManager.GetPosition(obj.Position.X.Value, obj.Position.Y.Value, obj.Position.Z.Value, scale);
            SpriteSetIndex = ObjManager.SpriteSets.FindItemIndex(x => x.Index == spriteInfo.SpriteSet);
            AnimIndex = spriteInfo.SpriteIndex;
            Scale = spriteInfo.Scale;

            if (SpriteSetIndex == -1)
                Debug.LogWarning($"Enemy sprite {spriteInfo.SpriteSet} is not loaded in current level");
        }

        public EnemyObject Object { get; set; }
        public override BinarySerializable SerializableData => Object;

        public override PrimaryObjectType PrimaryType => Object.PrimaryType;
        public override int SecondaryType => Object.SecondaryType;

        public override float Scale { get; }
    }
}