using BinarySerializer;
using BinarySerializer.KlonoaDTP;

namespace R1Engine
{
    public sealed class Unity_Object_PS1Klonoa_Enemy : Unity_Object_BasePS1Klonoa
    {
        public Unity_Object_PS1Klonoa_Enemy(Unity_ObjectManager_PS1Klonoa objManager, EnemyObject obj, float scale, int spriteSet, int sprite) : base(objManager)
        {
            Object = obj;
            Position = PS1Klonoa_Manager.GetPosition(obj.XPos.Value, obj.YPos.Value, obj.ZPos.Value, scale);
            SpriteSetIndex = ObjManager.SpriteSets.FindItemIndex(x => x.Index == spriteSet);
            AnimIndex = sprite;
        }

        public EnemyObject Object { get; set; }
        public override BinarySerializable SerializableData => Object;

        public override PrimaryObjectType PrimaryType => Object.PrimaryType;
        public override int SecondaryType => Object.SecondaryType;
    }
}