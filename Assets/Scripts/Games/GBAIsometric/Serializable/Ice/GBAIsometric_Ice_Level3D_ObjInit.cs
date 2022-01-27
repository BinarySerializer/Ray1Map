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

        private static readonly int[] _spriteSetFixIndices = Enumerable.Range(29, 35).ToArray();

        public static readonly int[][] SpriteSetLevelIndices = 
        {
            // 0 - Fodder
            // 1 - ?
            // 2 - Small enemy
            // 3 - Big enemy
            // ...
            _spriteSetFixIndices.Concat(new int[] { 064, 065, 067, 068, 069, 070 }).ToArray(), // 0
            _spriteSetFixIndices.Concat(new int[] { 071, 072, 067, 068, 069, 070 }).ToArray(), // 1
            _spriteSetFixIndices.Concat(new int[] { 073, 066, 067, 068, 069, 070 }).ToArray(), // 2
            _spriteSetFixIndices.Concat(new int[] { 074, 075, 067, 068, 069, 070 }).ToArray(), // 3
            _spriteSetFixIndices.Concat(new int[] { 073, 076, 077, 078, 079, 080, 081, 082, 083, 084, 085 }).ToArray(), // 4
            _spriteSetFixIndices.Concat(new int[] { 071, 086, 087, 088, 089, 090, 091, 092, 093 }).ToArray(), // 5
            _spriteSetFixIndices.Concat(new int[] { 064, 094, 095, 096, 097, 098, 099, 100 }).ToArray(), // 6
            _spriteSetFixIndices.Concat(new int[] { 064, 101, 102, 103, 104, 105, 092, 092, 106, 107 }).ToArray(), // 7
            _spriteSetFixIndices.Concat(new int[] { 064, 108, 095, 109, 110, 111, 112, 113, 114, 115 }).ToArray(), // 8
            _spriteSetFixIndices.Concat(new int[] { 073, 116, 117, 118, 119, 120, 121, 099 }).ToArray(), // 9
            _spriteSetFixIndices.Concat(new int[] { 073, 131, 132, 133, 134, 135, 136, 099 }).ToArray(), // 10
            _spriteSetFixIndices.Concat(new int[] { 074, 137, 138, 139, 140, 141, 142, 143, 083, 084, 085, 144, 145 }).ToArray(), // 11
            _spriteSetFixIndices.Concat(new int[] { 064, 146, 147, 124, 104, 126, 127, 148, 107, 107 }).ToArray(), // 12
            _spriteSetFixIndices.Concat(new int[] { 073, 122, 123, 124, 125, 126, 127, 128, 129, 130 }).ToArray(), // 13
            _spriteSetFixIndices.Concat(new int[] { 071, 149, 095, 150, 151, 152, 153, 115, 120, 099 }).ToArray(), // 14
            _spriteSetFixIndices.Concat(new int[] { 073, 154, 155 }).ToArray(), // 15
            _spriteSetFixIndices.Concat(new int[] { 074, 154, 155  }).ToArray(), // 16
        };

        public static SpriteInfo GetSprite(int level, int objType)
        {
            SpriteInfo s = GetLevelSprite(objType);

            if (s.SpriteSet != -1)
                s.SpriteSet = SpriteSetLevelIndices[level][s.SpriteSet];
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

                // Frog
                case 11:
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

                // Grendor
                case 115:
                case 116:
                    return new SpriteInfo(37);

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

                // Enemy
                case 140:
                case 141:
                case 142:
                case 143:
                case 144:
                    return new SpriteInfo(37);

                // Enemy
                case 145:
                case 146:
                case 147:
                case 148:
                case 149:
                    return new SpriteInfo(38);

                // Enemy
                case 150:
                case 151:
                case 152:
                case 153:
                case 154:
                    return new SpriteInfo(37);

                // Enemy
                case 155:
                case 156:
                case 157:
                case 158:
                case 159:
                    return new SpriteInfo(38);

                // Enemy
                case 160:
                case 161:
                case 162:
                case 163:
                case 164:

                case 180:
                case 181:
                case 182:
                case 183:
                case 184:
                    return new SpriteInfo(37);

                // Enemy
                case 165:
                case 166:
                case 167:
                case 168:
                case 169:
                    return new SpriteInfo(38);

                // Enemy
                case 185:
                case 186:
                case 187:
                case 188:
                case 189:
                    return new SpriteInfo(38);

                // Enemy
                case 190:
                case 191:
                case 192:
                case 193:
                case 194:
                    return new SpriteInfo(37);

                // Enemy
                case 195:
                case 196:
                case 197:
                case 198:
                case 199:
                    return new SpriteInfo(38);

                // Enemy
                case 200:
                case 201:
                case 202:
                case 203:
                case 204:
                    return new SpriteInfo(37);

                // Enemy
                case 205:
                case 206:
                case 207:
                case 208:
                case 209:
                    return new SpriteInfo(38);

                // Enemy
                case 210:
                case 211:
                case 212:
                case 213:
                case 214:
                    return new SpriteInfo(38);

                // Enemy
                case 215:
                case 216:
                case 217:
                case 218:
                case 219:
                    return new SpriteInfo(37);

                // Enemy
                case 220:
                case 221:
                case 222:
                case 223:
                case 224:
                    return new SpriteInfo(38);

                // Enemy
                case 225:
                case 226:
                case 227:
                case 228:
                case 229:
                    return new SpriteInfo(37);

                // Enemy
                case 230:
                case 231:
                case 232:
                case 233:
                case 234:
                    return new SpriteInfo(38);

                // Enemy
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                    return new SpriteInfo(37);

                // Ice fodder
                case 245:
                    return new SpriteInfo(35);

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