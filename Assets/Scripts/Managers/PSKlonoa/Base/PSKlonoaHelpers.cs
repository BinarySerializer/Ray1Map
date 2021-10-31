using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BinarySerializer;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using UnityEngine;

namespace R1Engine
{
    public static class PSKlonoaHelpers
    {
        public static ObjSpriteInfo GetSprite_Enemy(EnemyObject obj)
        {
            // TODO: Some enemies have palette swaps. This is done by modifying the x and y palette offsets (by default they are 0 and 500). For example the shielded Moo enemies in Vision 1-2

            // There are 42 object types (0-41). The graphics index is an index to an array of functions for displaying the graphics. The game
            // normally doesn't directly use the graphics index as it sometimes modifies it, but it appears the value initially set
            // in the object data will always match the correct sprite to show, so we can use that.
            var graphicsIndex = obj.GraphicsIndex;

            // Usually the graphics index matches the sprite set index (minus 1), but some are special cases, and since we don't want to show the
            // first sprite we hard-code this. Ideally we would animate them, but that is sadly entirely hard-coded :(
            return graphicsIndex switch
            {
                01 => new ObjSpriteInfo(0, 81), // Moo
                02 => new ObjSpriteInfo(1, 0),
                03 => new ObjSpriteInfo(2, 36), // Pinkie

                05 => new ObjSpriteInfo(4, 0), // Portal
                06 => new ObjSpriteInfo(5, 12),
                07 => new ObjSpriteInfo(6, 36), // Flying Moo
                08 => new ObjSpriteInfo(7, 16),
                09 => new ObjSpriteInfo(8, 4), // Spiker
                10 => new ObjSpriteInfo(9, 68),
                11 => new ObjSpriteInfo(10, 4),
                12 => new ObjSpriteInfo(11, 72),
                13 => new ObjSpriteInfo(12, 54),
                14 => new ObjSpriteInfo(13, 24),
                15 => new ObjSpriteInfo(14, 0), // Moo with shield
                16 => new ObjSpriteInfo(15, 154),
                17 => new ObjSpriteInfo(16, 0), // Moo with spiky shield
                18 => new ObjSpriteInfo(17, 0),
                19 => new ObjSpriteInfo(18, 8),
                20 => new ObjSpriteInfo(19, 28),
                21 => new ObjSpriteInfo(20, 0),

                23 => new ObjSpriteInfo(22, 44),
                24 => new ObjSpriteInfo(23, 76),
                25 => new ObjSpriteInfo(24, 0), // Big spiky ball
                26 => new ObjSpriteInfo(25, 36),

                28 => new ObjSpriteInfo(27, 118),
                29 => new ObjSpriteInfo(28, 165),
                30 => new ObjSpriteInfo(29, 41),
                31 => new ObjSpriteInfo(30, 157),
                32 => new ObjSpriteInfo(31, 16),

                35 => new ObjSpriteInfo(0, 81, scale: 2), // Big Moo
                36 => new ObjSpriteInfo(1, 0, scale: 2),
                37 => new ObjSpriteInfo(0, 81, scale: 2), // Big Moo

                39 => new ObjSpriteInfo(14, 0, scale: 2), // Big Moo with shield

                112 => new ObjSpriteInfo(11, 149),
                137 => new ObjSpriteInfo(11, 149, scale: 2),
                _ => new ObjSpriteInfo(-1, -1)
            };
        }

        public static ObjSpriteInfo GetSprite_Collectible(CollectibleObject obj)
        {
            switch (obj.SecondaryType)
            {
                // Switch
                case 1:
                    return new ObjSpriteInfo(68, 10);

                // Dream Stone
                case 2:
                    return obj.Ushort_14 == 0 ? new ObjSpriteInfo(68, 0) : new ObjSpriteInfo(68, 5);

                // Heart, life
                case 3:
                case 4:
                    return obj.Short_0E switch
                    {
                        3 => new ObjSpriteInfo(68, 30),
                        4 => new ObjSpriteInfo(68, 22),
                        15 => new ObjSpriteInfo(68, 57),
                        _ => new ObjSpriteInfo(-1, -1)
                    };

                // Bubble
                case 5:
                case 6:
                case 16:
                case 17:
                    return obj.Short_0E switch
                    {
                        5 => new ObjSpriteInfo(68, 42), // Checkpoint
                        9 => new ObjSpriteInfo(68, 43), // Item
                        13 => new ObjSpriteInfo(68, 44), // x2
                        _ => new ObjSpriteInfo(-1, -1)
                    };

                // Nagapoko Egg
                case 8:
                case 9:
                    return new ObjSpriteInfo(68, 76);

                // Bouncy spring
                case 10:
                    return new ObjSpriteInfo(21, 2);

                // Colored orb (Vision 5-1)
                case 15:
                    return new ObjSpriteInfo(68, 81 + (6 * (obj.Ushort_14 - 2)));

                default:
                    return new ObjSpriteInfo(-1, -1);
            }
        }

        public static Vector3 GetPosition(float x, float y, float z, float scale) => new Vector3(x / scale, -z / scale, -y / scale);

        public static Vector3 GetPosition(MovementPathBlock[] path, int position, Vector3 relativePos, float scale)
        {
            var blockIndex = 0;
            int blockPosOffset;

            if (position < 0)
            {
                blockIndex = 0;
                blockPosOffset = position;
            }
            else
            {
                var iVar6 = 0;

                do
                {
                    var iVar2 = path[blockIndex].BlockLength;

                    if (iVar2 == 0x7ffe)
                    {
                        blockIndex = 0;
                    }
                    else
                    {
                        if (iVar2 == 0x7fff)
                        {
                            iVar6 -= path[blockIndex - 1].BlockLength;
                            break;
                        }

                        iVar6 += iVar2;
                        blockIndex++;
                    }
                } while (iVar6 <= position);

                iVar6 -= position;

                blockIndex--;

                if (iVar6 < 0)
                    blockPosOffset = -iVar6;
                else
                    blockPosOffset = path[blockIndex].BlockLength - iVar6;
            }

            var block = path[blockIndex];

            float xPos = block.XPos + block.DirectionX * blockPosOffset + relativePos.x;
            float yPos = block.YPos + block.DirectionY * blockPosOffset + relativePos.y;
            float zPos = block.ZPos + block.DirectionZ * blockPosOffset + relativePos.z;

            return GetPosition(xPos, yPos, zPos, scale);
        }

        public static Vector3 GetPositionVector(KlonoaVector16 pos, Vector3? posOffset, float scale)
        {
            if (posOffset == null)
                return new Vector3(pos.X / scale, -pos.Y / scale, pos.Z / scale);
            else
                return new Vector3((pos.X + posOffset.Value.x) / scale, -(pos.Y + posOffset.Value.y) / scale, (pos.Z + posOffset.Value.z) / scale);
        }

        public static Bounds GetDimensions(PS1_TMD tmd, float scale)
        {
            var verts = tmd.Objects.SelectMany(x => x.Vertices).ToArray();
            var min = new Vector3(verts.Min(v => v.X), verts.Min(v => v.Y), verts.Min(v => v.Z)) / scale;
            var max = new Vector3(verts.Max(v => v.X), verts.Max(v => v.Y), verts.Max(v => v.Z)) / scale;
            var center = Vector3.Lerp(min, max, 0.5f);

            return new Bounds(center, max - min);
        }

        public static float GetRotationInDegrees(float value)
        {
            if (value > 0x800)
                value -= 0x1000;

            return value * (360f / 0x1000);
        }

        public static Quaternion GetQuaternion(ObjRotation rot)
        {
            return GetQuaternion(rot.RotationX, rot.RotationY, rot.RotationZ);
        }

        public static Quaternion GetQuaternion(float rotX, float rotY, float rotZ)
        {
            return
                Quaternion.Euler(-GetRotationInDegrees(rotX), 0, 0) *
                Quaternion.Euler(0, GetRotationInDegrees(rotY), 0) *
                Quaternion.Euler(0, 0, -GetRotationInDegrees(rotZ));
        }

        public static void FillTextureFromVRAM(
            Texture2D tex,
            PS1_VRAM vram,
            int width, int height,
            PS1_TIM.TIM_ColorFormat colorFormat,
            int texX, int texY,
            int clutX, int clutY,
            int texturePageOriginX = 0, int texturePageOriginY = 0,
            int texturePageOffsetX = 0, int texturePageOffsetY = 0,
            int texturePageX = 0, int texturePageY = 0,
            bool flipX = false, bool flipY = false,
            bool useDummyPal = false)
        {
            var dummyPal = useDummyPal ? Util.CreateDummyPalette(colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8 ? 256 : 16) : null;

            texturePageOriginX *= 2;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte paletteIndex;

                    if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_8)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + texturePageOffsetX + x,
                            texturePageOriginY + texturePageOffsetY + y);
                    }
                    else if (colorFormat == PS1_TIM.TIM_ColorFormat.BPP_4)
                    {
                        paletteIndex = vram.GetPixel8(texturePageX, texturePageY,
                            texturePageOriginX + (texturePageOffsetX + x) / 2,
                            texturePageOriginY + texturePageOffsetY + y);

                        if (x % 2 == 0)
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 0);
                        else
                            paletteIndex = (byte)BitHelpers.ExtractBits(paletteIndex, 4, 4);
                    }
                    else
                    {
                        throw new Exception($"Non-supported color format");
                    }

                    // Get the color from the palette
                    var c = useDummyPal ? dummyPal[paletteIndex] : vram.GetColor1555(0, 0, clutX + paletteIndex, clutY);

                    // http://hitmen.c02.at/files/docs/psx/psx.pdf page 31
                    if (c.Red == 0 && c.Green == 0 && c.Blue == 0 && c.Alpha == 0)
                        continue;

                    c.Alpha = 1;

                    var texOffsetX = flipX ? width - x - 1 : x;
                    var texOffsetY = flipY ? height - y - 1 : y;

                    // Set the pixel
                    tex.SetPixel(texX + texOffsetX, tex.height - (texY + texOffsetY) - 1, c.GetColor());
                }
            }
        }

        public static void GenerateCutsceneTextTranslation(Loader loader, Dictionary<string, char> d, int cutscene, int instruction, string text)
        {
            var c = loader.LevelPack.CutscenePack.Cutscenes[cutscene];
            var i = (CutsceneInstructionData_DrawText)c.Cutscene_Normal.Instructions[instruction].Data;

            var textIndex = 0;

            foreach (var cmd in i.TextCommands)
            {
                if (cmd.Type != CutsceneInstructionData_DrawText.TextCommand.CommandType.DrawChar)
                    continue;

                var charImgData = c.Font.CharactersImgData[cmd.Command];

                using SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                var hash = Convert.ToBase64String(sha1.ComputeHash(charImgData));

                if (d.ContainsKey(hash))
                {
                    if (d[hash] != text[textIndex])
                        Debug.LogWarning($"Character {text[textIndex]} has multiple font textures!");
                }
                else
                {
                    d[hash] = text[textIndex];
                }

                textIndex++;
            }

            var str = new StringBuilder();

            foreach (var v in d.OrderBy(x => x.Value))
            {
                var value = v.Value.ToString();

                if (value == "\"" || value == "\'")
                    value = $"\\{value}";

                str.AppendLine($"[\"{v.Key}\"] = '{value}',");
            }

            str.ToString().CopyToClipboard();
        }

        public class ObjSpriteInfo
        {
            public ObjSpriteInfo(int spriteSet, int spriteIndex, int scale = 1, int palOffsetX = 0, int palOffsetY = 500)
            {
                SpriteSet = spriteSet;
                SpriteIndex = spriteIndex;
                Scale = scale;
                PalOffsetX = palOffsetX;
                PalOffsetY = palOffsetY;
            }

            public int SpriteSet { get; }
            public int SpriteIndex { get; }
            public int Scale { get; }
            public int PalOffsetX { get; }
            public int PalOffsetY { get; }
        }
    }
}