using System.Linq;
using UnityEngine;

namespace Ray1Map.GBAIsometric
{
    public static class GBAIsometric_Ice_Level3D_ObjInit
    {
        private static readonly int[] _portalFlipTypes = new[]
        {
            0x28, 0x2a, 0x2c, 0x2f, 0x31, 0x34, 0x39, 0x3c, 0x3d, 
            0x41, 0x43, 0x44, 0x46, 0x47, 0x4a, 0x4f, 0x50, 0x51,
        };

        private static readonly int[] _animSetFixIndices = Enumerable.Range(29, 35).ToArray();

        public static readonly int[][] AnimSetLevelIndices = 
        {
            _animSetFixIndices.Concat(new int[] { 64, 65, 67, 68, 69, 70 }).ToArray(), // 0
            _animSetFixIndices.Concat(new int[] { 71, 72, 67, 68, 69, 70 }).ToArray(), // 1
            _animSetFixIndices.Concat(new int[] { 73, 66, 67, 68, 69, 70 }).ToArray(), // 2
            // TODO: Fill out the rest
        };

        public static SpriteInfo GetSprite(int level, int objType)
        {
            SpriteInfo s = GetLevelSprite(objType);

            if (s.SpriteSet != -1)
                s.SpriteSet = AnimSetLevelIndices[level][s.SpriteSet];
            else
                Debug.LogWarning($"No sprite defined for type {objType}");

            return s;
        }

        private static SpriteInfo GetLevelSprite(int objType)
        {
            // TODO: Fill out the rest
            switch (objType)
            {
                // Spyro
                case 1:
                    return new SpriteInfo(0);

                // Sheep
                case 10:
                    return new SpriteInfo(35);

                // HUD gem
                case 15:
                    return new SpriteInfo(16);

                // Rabbit
                case 16:
                    return new SpriteInfo(35);

                // HUD fairy
                case 21:
                    return new SpriteInfo(24, 0);

                // Portal
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                case 48:
                case 49:
                case 50:
                case 51:
                case 52:
                case 53:
                case 54:
                case 55:
                case 56:
                case 57:
                case 58:
                case 59:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 66:
                case 67:
                case 68:
                case 69:
                case 70:
                case 71:
                case 72:
                case 73:
                case 74:
                case 75:
                case 76:
                case 77:
                case 78:
                case 79:
                case 80:
                case 81:
                    return new SpriteInfo(22, -1, _portalFlipTypes.Contains(objType));

                // Life
                case 90:
                    return new SpriteInfo(31); // TODO: Display child object for cork with index 32

                // Gem
                case 117:
                case 118:
                case 119:
                case 120:
                    return new SpriteInfo((objType - 117) * 2 + 8);

                // Gem container (fire)?
                case 126:
                case 127:
                case 128:
                case 129:
                    return new SpriteInfo(19, 0);

                // Gem container (fire)
                case 130:
                case 131:
                case 132:
                case 133:
                case 134:
                    return new SpriteInfo(19, 0);

                // Gem container (charge)
                case 135:
                case 136:
                case 137:
                case 138:
                case 139:
                    return new SpriteInfo(18, 0);

                default:
                    return new SpriteInfo();
            }
        }

        public class SpriteInfo
        {
            public SpriteInfo(int spriteSet = -1, int sprite = -1, bool flipX = false)
            {
                SpriteSet = spriteSet;
                Sprite = sprite;
                FlipX = flipX;
            }

            public int SpriteSet { get; set; }
            public int Sprite { get; set; }
            public bool FlipX { get; set; }
        }
    }
}