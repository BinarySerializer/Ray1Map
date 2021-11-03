using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public sealed class Unity_Object_PSKlonoa_DTP_Collectible : Unity_Object_BasePSKlonoa_DTP
    {
        public Unity_Object_PSKlonoa_DTP_Collectible(Unity_ObjectManager_PSKlonoa_DTP objManager, CollectibleObject obj, Vector3 pos, PSKlonoaHelpers.ObjSpriteInfo spriteInfo) : base(objManager)
        {
            Object = obj;
            Position = pos;
            SpriteSetIndex = ObjManager.SpriteSets.FindItemIndex(x => x.Index == spriteInfo.SpriteSet);
            AnimIndex = spriteInfo.SpriteIndex;

            if (SpriteSetIndex == -1)
                Debug.LogWarning($"Collectible sprite {spriteInfo.SpriteSet} is not loaded in current level");
        }

        public CollectibleObject Object { get; set; }
        public override BinarySerializable SerializableData => Object;

        public override PrimaryObjectType PrimaryType => Object.PrimaryType;
        public override int SecondaryType => Object.SecondaryType;
    }
}