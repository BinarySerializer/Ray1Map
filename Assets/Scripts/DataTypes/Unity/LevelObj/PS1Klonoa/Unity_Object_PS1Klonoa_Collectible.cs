﻿using BinarySerializer;
using BinarySerializer.KlonoaDTP;
using UnityEngine;

namespace R1Engine
{
    public sealed class Unity_Object_PS1Klonoa_Collectible : Unity_Object_BasePS1Klonoa
    {
        public Unity_Object_PS1Klonoa_Collectible(Unity_ObjectManager_PS1Klonoa objManager, CollectibleObject obj, Vector3 pos, int spriteSet, int sprite) : base(objManager)
        {
            Object = obj;
            Position = pos;
            SpriteSetIndex = ObjManager.SpriteSets.FindItemIndex(x => x.Index == spriteSet);
            AnimIndex = sprite;
        }

        public CollectibleObject Object { get; set; }
        public override BinarySerializable SerializableData => Object;

        public override PrimaryObjectType PrimaryType => Object.PrimaryType;
        public override int SecondaryType => Object.SecondaryType;
    }
}