using System.Collections.Generic;
using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public sealed class Unity_Object_PSKlonoa_DTP_Enemy : Unity_Object_BasePSKlonoa_DTP
    {
        public Unity_Object_PSKlonoa_DTP_Enemy(Unity_ObjectManager_PSKlonoa_DTP objManager, EnemyObject obj, float scale, PSKlonoa_DTP_BaseManager.ObjSpriteInfo spriteInfo) : base(objManager)
        {
            Object = obj;
            Position = KlonoaHelpers.GetPosition(obj.Position.X.Value, obj.Position.Y.Value, obj.Position.Z.Value, scale);
            SpriteSetIndex = GetSpriteSetIndex(Unity_ObjectManager_PSKlonoa_DTP.SpritesType.CommonSprites, spriteInfo.SpriteSet);
            AnimIndex = spriteInfo.SpriteIndex;
            Scale = spriteInfo.Scale;
            WaypointLinks = new List<int>();
            DetectionRadius = Object.Data?.DespawnDistance / scale;

            if (SpriteSetIndex == -1)
                Debug.LogWarning($"Enemy sprite {spriteInfo.SpriteSet} is not loaded in current level");
        }

        public EnemyObject Object { get; set; }
        public override BinarySerializable SerializableData => Object;

        public override PrimaryObjectType PrimaryType => Object.PrimaryType;
        public override int SecondaryType => Object.SecondaryType;

        public override float Scale { get; }

        public override bool CanBeLinked => true;
        public override IEnumerable<int> Links => WaypointLinks;

        public List<int> WaypointLinks { get; }

        public override float? DetectionRadius { get; }
    }
}