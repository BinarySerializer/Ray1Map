using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace R1Engine
{
    public class GBA_BatmanVengeance_Manager : GBA_Manager
    {
        public override IEnumerable<int>[] WorldLevels => new IEnumerable<int>[]
        {
            Enumerable.Range(0, 37)
        };

        public override int[] MenuLevels => new int[0];
        public override int DLCLevelCount => 0;
        public override int[] AdditionalSprites4bpp => new int[0];
        public override int[] AdditionalSprites8bpp => new int[0];

        public override UniTask ExtractVignetteAsync(GameSettings settings, string outputDir) => throw new System.NotImplementedException();


        public override Unity_ObjGraphics GetCommonDesign(GBA_ActorModel model, GBA_Data data) => GetCommonDesign(model?.Puppet_BatmanVengeance, data);
        public Unity_ObjGraphics GetCommonDesign(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) {
            // Create the design
            var des = new Unity_ObjGraphics {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };

            if (puppet == null) 
                return des;

            var tileSet = puppet.TileSet;
            var pal = GetSpritePalette(puppet, data);
            const int tileWidth = 8;
            const int tileSize = (tileWidth * tileWidth) / 2;
            var numPalettes = pal.Length / 16;

            // Add sprites for each palette
            if (tileSet.Is8Bit)
            {
                var pal_8 = Util.ConvertGBAPalette((RGBA5551Color[])pal);
                var tileSetTex = Util.ToTileSetTexture(tileSet.TileSet, pal_8, Util.TileEncoding.Linear_8bpp, CellSize, false);

                // Extract every sprite
                for (int y = 0; y < tileSetTex.height; y += CellSize)
                {
                    for (int x = 0; x < tileSetTex.width; x += CellSize)
                    {
                        des.Sprites.Add(tileSetTex.CreateSprite(rect: new Rect(x, y, CellSize, CellSize)));
                    }
                }
            }
            else
            {
                var pal_8 = Util.ConvertAndSplitGBAPalette((RGBA5551Color[])pal);

                for (int palIndex = 0; palIndex < numPalettes; palIndex++)
                {
                    for (int i = 0; i < tileSet.TileSetLength; i++)
                    {
                        var tex = TextureHelpers.CreateTexture2D(CellSize, CellSize);

                        for (int y = 0; y < tileWidth; y++)
                        {
                            for (int x = 0; x < tileWidth; x++)
                            {
                                int index = (i * tileSize) + ((y * tileWidth + x) / 2);

                                var b = tileSet.TileSet[index];
                                var v = BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                                Color c = pal_8[palIndex][v];

                                if (v != 0)
                                    c = new Color(c.r, c.g, c.b, 1f);

                                tex.SetPixel(x, (tileWidth - 1 - y), c);
                            }
                        }

                        tex.Apply();
                        des.Sprites.Add(tex.CreateSprite());
                    }
                }
            }

            Unity_ObjAnimationPart[] GetPartsForTilemap(GBA_BatmanVengeance_Puppet s, GBA_BatmanVengeance_Animation a, int frame, GBA_BatmanVengeance_AnimationCommand c) {
                var height = s.TilemapHeight;
                var width = s.TilemapWidth;
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[width * height];
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var ind = y * width + x;
                        parts[ind] = new Unity_ObjAnimationPart {
                            ImageIndex = tileSet.TileSetLength * (tileSet.Is8Bit ? 0 : c.TileMap[ind].PaletteIndex) + (c.TileMap[ind].TileIndex),
                            XPosition = x * CellSize - (width * CellSize / 2),
                            YPosition = y * CellSize - (height * CellSize / 2),
                            IsFlippedHorizontally = c.TileMap[ind].IsFlippedHorizontally,
                            IsFlippedVertically = c.TileMap[ind].IsFlippedVertically,
                        };
                    }
                }
                return parts;
            }

            Unity_ObjAnimationPart[] GetPartsForLayer(GBA_BatmanVengeance_Puppet s, GBA_BatmanVengeance_Animation a, int frame, GBA_BatmanVengeance_AnimationChannel l) 
            {
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[l.XSize * l.YSize];

                var imageIndex = l.ImageIndex / (tileSet.Is8Bit ? 2 : 1);

                if (imageIndex > puppet.TileSet.TileSetLength)
                    Controller.print("Image index too high: " + puppet.Offset + " - " + l.Offset + $"Index: {imageIndex} - Max: {puppet.TileSet.TileSetLength - 1}");

                if (l.PaletteIndex > pal.Length / 16)
                    Controller.print("Palette index too high: " + puppet.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (pal.Length / 16));

                for (int y = 0; y < l.YSize; y++) {
                    for (int x = 0; x < l.XSize; x++) {
                        parts[y * l.XSize + x] = new Unity_ObjAnimationPart {
                            ImageIndex = tileSet.TileSetLength * (tileSet.Is8Bit ? 0 : l.PaletteIndex) + (imageIndex + y * l.XSize + x),
                            IsFlippedHorizontally = l.IsFlippedHorizontally,
                            IsFlippedVertically = l.IsFlippedVertically,
                            XPosition = (l.XPosition + (l.IsFlippedHorizontally ? (l.XSize - 1 - x) : x) * CellSize),
                            YPosition = (l.YPosition + (l.IsFlippedVertically ? (l.YSize - 1 - y) : y) * CellSize),
                            TransformOriginX = (l.XPosition + l.XSize * CellSize / 2f),
                            TransformOriginY = (l.YPosition + l.YSize * CellSize / 2f)
                        };
                    }
                }
                return parts;
            }

            // Add first animation for now
            foreach (var a in puppet.Animations) {
                var unityAnim = new Unity_ObjAnimation();
                var frames = new List<Unity_ObjAnimationFrame>();
                for (int i = 0; i < a.FrameCount; i++) {
                    var parts = new List<Unity_ObjAnimationPart[]>();
                    var collisionParts = new List<Unity_ObjAnimationCollisionPart>();
                    foreach (var c in a.Frames[i].Commands) {
                        switch (c.Command) {
                            case GBA_BatmanVengeance_AnimationCommand.InstructionCommand.SpriteNew:
                                parts.Add(c.Layers.SelectMany(l => GetPartsForLayer(puppet, a, i, l)).Reverse().ToArray());
                                break;
                            case GBA_BatmanVengeance_AnimationCommand.InstructionCommand.SpriteTilemap:
                                parts.Add(GetPartsForTilemap(puppet, a, i, c));
                                break;
                            case GBA_BatmanVengeance_AnimationCommand.InstructionCommand.Hitbox:
                                collisionParts.Add(new Unity_ObjAnimationCollisionPart
                                {
                                    XPosition = c.HitboxXPos - c.HitboxHalfWidth,
                                    YPosition = c.HitboxYPos - c.HitboxHalfHeight,
                                    Width = c.HitboxHalfWidth * 2,
                                    Height = c.HitboxHalfHeight * 2,
                                    Type = Unity_ObjAnimationCollisionPart.CollisionType.AttackBox
                                });
                                break;
                        }
                    }
                    if (parts.Count == 0 && frames.Count > 0) {
                        var lastFrame = frames.Last();
                        frames.Add(new Unity_ObjAnimationFrame(lastFrame.SpriteLayers, lastFrame.CollisionLayers));
                    } else {
                        frames.Add(new Unity_ObjAnimationFrame(parts.SelectMany(p => p).ToArray(), collisionParts.ToArray()));
                    }
                }
                unityAnim.Frames = frames.ToArray();
                unityAnim.AnimSpeeds = a.Frames.Select(x => (x.Commands.FirstOrDefault(c => c.IsTerminator)?.Time ?? 0) + 1).ToArray();
                des.Animations.Add(unityAnim);
            }

            return des;
        }

        protected virtual BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => puppet.Palette.Palette;
    }
}