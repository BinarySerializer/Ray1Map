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
                /*if (l.TransformMode == GBA_AnimationLayer.AffineObjectMode.Hide
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Window
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Regular
                    || l.Mosaic) return new Unity_ObjAnimationPart[0];
                if (l.Color == GBA_AnimationLayer.ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Unity_ObjAnimationPart[0];
                }*/
                var height = s.TilemapHeight;
                var width = s.TilemapWidth;
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[width * height];
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        var flipX = false;
                        var flipY = false;
                        var ind = y * width + x;
                        parts[ind] = new Unity_ObjAnimationPart {
                            ImageIndex = tileSet.TileSetLength * (tileSet.Is8Bit ? 0 : c.TileMap[ind].PaletteIndex) + (c.TileMap[ind].TileIndex),
                            IsFlippedHorizontally = flipX,
                            IsFlippedVertically = flipY,
                            XPosition = x * CellSize,
                            YPosition = y * CellSize,
                        };
                    }
                }
                return parts;
            }

            Unity_ObjAnimationPart[] GetPartsForLayer(GBA_BatmanVengeance_Puppet s, GBA_BatmanVengeance_Animation a, int frame, GBA_BatmanVengeance_AnimationChannel l) {
                /*if (l.TransformMode == GBA_AnimationLayer.AffineObjectMode.Hide
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Window
                    || l.RenderMode == GBA_AnimationLayer.GfxMode.Regular
                    || l.Mosaic) return new Unity_ObjAnimationPart[0];
                if (l.Color == GBA_AnimationLayer.ColorMode.Color8bpp) {
                    Debug.LogWarning("Animation Layer @ " + l.Offset + " has 8bpp color mode, which is currently not supported.");
                    return new Unity_ObjAnimationPart[0];
                }*/
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[l.XSize * l.YSize];
                if (l.ImageIndex > puppet.TileSet.TileSetLength) {
                    Controller.print("Image index too high: " + puppet.Offset + " - " + l.Offset + $"Index: {l.ImageIndex} - Max: {puppet.TileSet.TileSetLength - 1}");
                }
                if (l.PaletteIndex > pal.Length / 16) {
                    Controller.print("Palette index too high: " + puppet.Offset + " - " + l.Offset + " - " + l.PaletteIndex + " - " + (pal.Length / 16));
                }
                float rot = 0;// l.GetRotation(a, s, frame);
                Vector2? scl = null;// l.GetScale(a, s, frame);
                for (int y = 0; y < l.YSize; y++) {
                    for (int x = 0; x < l.XSize; x++) {
                        parts[y * l.XSize + x] = new Unity_ObjAnimationPart {
                            ImageIndex = tileSet.TileSetLength * (tileSet.Is8Bit ? 0 : l.PaletteIndex) + (l.ImageIndex + y * l.XSize + x),
                            IsFlippedHorizontally = l.IsFlippedHorizontally,
                            IsFlippedVertically = l.IsFlippedVertically,
                            XPosition = (l.XPosition + (l.IsFlippedHorizontally ? (l.XSize - 1 - x) : x) * CellSize),
                            YPosition = (l.YPosition + (l.IsFlippedVertically ? (l.YSize - 1 - y) : y) * CellSize),
                            Rotation = rot,
                            Scale = scl,
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
                    // TODO: Change to use commands system
                    List<Unity_ObjAnimationPart[]> parts = new List<Unity_ObjAnimationPart[]>();
                    foreach (var c in a.Frames[i].Commands) {
                        switch (c.Command) {
                            case GBA_BatmanVengeance_AnimationCommand.InstructionCommand.SpriteNew:
                                parts.Add(c.Layers.SelectMany(l => GetPartsForLayer(puppet, a, i, l)).Reverse().ToArray());
                                break;
                            case GBA_BatmanVengeance_AnimationCommand.InstructionCommand.SpriteTilemap:
                                parts.Add(GetPartsForTilemap(puppet, a, i, c));
                                break;
                        }
                    }
                    frames.Add(new Unity_ObjAnimationFrame(parts.SelectMany(p => p).ToArray()));
                }
                unityAnim.Frames = frames.ToArray();
                unityAnim.AnimSpeed = 1;
                unityAnim.AnimSpeeds = a.Frames.Select(x =>
                {
                    return (x.Commands.FirstOrDefault(c => c.Command == GBA_BatmanVengeance_AnimationCommand.InstructionCommand.Terminator0 || c.Command == GBA_BatmanVengeance_AnimationCommand.InstructionCommand.Terminator20)?.Padding[0] ?? 0) + 1;
                }).ToArray();
                des.Animations.Add(unityAnim);
            }

            return des;
        }

        protected virtual BaseColor[] GetSpritePalette(GBA_BatmanVengeance_Puppet puppet, GBA_Data data) => puppet.Palette.Palette;
    }
}