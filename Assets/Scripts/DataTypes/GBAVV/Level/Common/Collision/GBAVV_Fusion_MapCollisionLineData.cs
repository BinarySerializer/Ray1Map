using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class GBAVV_Fusion_MapCollisionLineData : BinarySerializable
    {
        public bool IsSingleValue { get; set; } // Set before serializing

        public byte[] Data { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Data = s.SerializeArray<byte>(Data, IsSingleValue ? 4 : 20, name: nameof(Data));
        }

        public CollisionTypes GetCollisionType()
        {
            if (Context?.GetR1Settings().EngineVersion == EngineVersion.GBAVV_CrashFusion)
            {
                if (Data.ElementAtOrDefault(0) == 1)
                    return CollisionTypes.Water;
                else if (Data.ElementAtOrDefault(16) == 7)
                    return CollisionTypes.Death;
                else if (Data.ElementAtOrDefault(16) == 10)
                    return CollisionTypes.Lava;
                else if (Data.ElementAtOrDefault(13) == 1)
                    return CollisionTypes.Slippery;
            }
            else if (Context?.GetR1Settings().EngineVersion == EngineVersion.GBAVV_SpyroFusion)
            {
                if (Data.ElementAtOrDefault(9) == 1)
                    return CollisionTypes.Lava;
                else if (Data.ElementAtOrDefault(10) == 1)
                    return CollisionTypes.Slippery;
                else if (Data.ElementAtOrDefault(8) == 1)
                    return CollisionTypes.Passthrough;
            }

            return CollisionTypes.Solid;
        }

        public Color GetColor()
        {
            switch (GetCollisionType())
            {
                default:
                case CollisionTypes.Solid:
                    return new Color(84 / 255f, 55 / 255f, 25 / 255f);
                case CollisionTypes.Death:
                    return new Color(212 / 255f, 142 / 255f, 13 / 255f);
                case CollisionTypes.Lava:
                    return new Color(212 / 255f, 93 / 255f, 13 / 255f);
                case CollisionTypes.Slippery:
                    return new Color(10 / 255f, 100 / 255f, 168 / 255f);
                case CollisionTypes.Water:
                    return new Color(17 / 255f, 143 / 255f, 207 / 255f);
                case CollisionTypes.Passthrough:
                    return new Color(133 / 255f, 94 / 255f, 56 / 255f);
            }
        }

        public enum CollisionTypes
        {
            Solid,
            Death,
            Lava,
            Slippery,
            Water,
            Passthrough,
        }

        /*

        Crash:
        
        00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00 - Normal
        00 00 00 00                                                     - Normal

        00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00  07 00 00 00 - Spikes

        01 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00 - Water
        01 00 00 00                                                     - Water

        00 00 00 00  00 00 00 00  00 00 00 00  00 01 00 00  00 00 00 00 - Slippery
        00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00  0A 00 00 00 - Lava

        Spyro:

        00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00  00 00 00 00 - Normal
        00 00 00 00                                                     - Normal
                                                            
        00 00 00 00  00 00 00 00  00 01 00 00  00 00 00 00  00 00 00 00 - Lava
        00 00 00 00  00 00 00 00  00 00 01 00  00 00 00 00  00 00 00 00 - Slippery
        00 00 00 00  00 00 00 00  01 00 00 00  00 00 00 00  00 00 00 00 - Passthrough

        */
    }
}