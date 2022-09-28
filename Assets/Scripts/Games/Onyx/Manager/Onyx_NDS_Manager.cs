using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.Nintendo.GBA;
using Cysharp.Threading.Tasks;
using ImageMagick;
using Ray1Map;
using UnityEngine;
using ROMBase = BinarySerializer.Nintendo.NDS.ROMBase;

namespace BinarySerializer.Ubisoft.Onyx.NDS
{
    public class Onyx_NDS_Manager : BaseGameManager
    {
        private const string ROMFilePath = "ROM.nds";

        public override GameInfo_Volume[] GetLevels(GameSettings settings)
        {
            return new GameInfo_Volume[]
            {
                new GameInfo_Volume(null, new GameInfo_World[]
                {
                    new GameInfo_World(0, new int[0])
                })
            };
        }

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return new GameAction[]
            {
                // TODO: Add raw NDS file export
                new GameAction("Export & convert", false, true, (input, output) => ExportAndConvertAsync(settings, output))
            };
        }

        public async UniTask ExportAndConvertAsync(GameSettings settings, string outputDir)
        {
            using Context context = new Ray1MapContext(settings);
            await LoadFilesAsync(context);

            ROMBase rom = FileFactory.Read<ROMBase>(context, ROMFilePath);
            OnyxFileResolver resolver = new(rom.Header);
            context.StoreObject(OnyxFileResolver.Key, resolver);

            var s = context.Deserializer;
            foreach (string filePath in resolver.NDS_EnumerateFilePaths())
            {
                OnyxFile file = resolver.NDS_DeserializeFile(s, filePath);

                switch (file)
                {
                    case BitmapFile bmp:
                        RGB555Color[] pal = bmp.PaletteReference.Value.ColorData;
                        byte[] imgData = bmp.ImgData;

                        Texture2D bmpTex = TextureHelpers.CreateTexture2D(bmp.Width, bmp.Height);

                        bmpTex.FillRegion(
                            imgData: imgData, 
                            imgDataOffset: 0, 
                            pal: Util.ConvertGBAPalette(pal), 
                            encoding: Util.TileEncoding.Linear_8bpp, 
                            regionX: 0, 
                            regionY: 0, 
                            regionWidth: bmp.Width, 
                            regionHeight: bmp.Height,
                            flipTextureY: true);

                        Util.ByteArrayToFile(Path.Combine(outputDir, $"{filePath}.png"), bmpTex.EncodeToPNG());

                        break;

                    // TODO: Remove this? Or maybe instead get correct palette + 4/8-bit from puppet and export there.
                    case SpriteTileSetFile spriteTileSet:
                        Texture2D spriteTileSetTex_4 = Util.ToTileSetTexture(
                            imgData: spriteTileSet.ImgData, 
                            pal: Util.ConvertGBAPalette(PaletteHelpers.CreateDummyPalette(16)),
                            encoding: Util.TileEncoding.Linear_4bpp, 
                            tileWidth: Constants.TileSize, 
                            wrap: 8,
                            flipY: true);

                        Util.ByteArrayToFile(Path.Combine(outputDir, $"{filePath}_4.png"), spriteTileSetTex_4.EncodeToPNG());

                        Texture2D spriteTileSetTex_8 = Util.ToTileSetTexture(
                            imgData: spriteTileSet.ImgData, 
                            pal: Util.ConvertGBAPalette(PaletteHelpers.CreateDummyPalette(256)),
                            encoding: Util.TileEncoding.Linear_8bpp, 
                            tileWidth: Constants.TileSize,
                            wrap: 8,
                            flipY: true);

                        Util.ByteArrayToFile(Path.Combine(outputDir, $"{filePath}_8.png"), spriteTileSetTex_8.EncodeToPNG());

                        break;

                    case PuppetFile puppet:
                        await ExportAnimations(puppet, Path.Combine(outputDir, filePath));
                        break;

                    default:
                        
                        break;
                }
            }
        }

        // TODO: Clean up and fix this hacky code. Mostly copied from GBA with some changes.
        protected async UniTask ExportAnimations(PuppetFile spr, string outputDir)
        {
            MagickImage[] sprites = null;

            try
            {
                var commonDesign = GetCommonDesign(spr);

                // Convert Texture2D to MagickImage
                sprites = commonDesign.Sprites.Select(x => x.ToMagickImage()).ToArray();

                var animIndex = 0;

                // Export every animation
                foreach (var anim in commonDesign.Animations)
                {
                    await Controller.WaitIfNecessary();
                    var frameIndex = 0;
                    var animDir = Path.Combine(outputDir, $"{animIndex}-{anim.AnimSpeed}");
                    Directory.CreateDirectory(animDir);
                    if (anim.Frames == null || anim.Frames.Length == 0) continue;


                    /*var shiftX = anim.Frames.Min(f => f.Layers.Select(x => Mathf.Min(0, x.XPosition)).DefaultIfEmpty().Min()) * -1;
                    var shiftY = anim.Frames.Min(f => f.Layers.Select(x => Mathf.Min(0, x.YPosition)).DefaultIfEmpty().Min()) * -1;

                    var maxX = anim.Frames.Max(f => f.Layers.Select(x => x.XPosition).DefaultIfEmpty().Max()) + 8 + shiftX;
                    var maxY = anim.Frames.Max(f => f.Layers.Select(x => x.YPosition).DefaultIfEmpty().Max()) + 8 + shiftY;*/

                    Vector2Int min = new Vector2Int();
                    Vector2Int max = new Vector2Int();
                    foreach (var frame in anim.Frames)
                    {
                        foreach (var layer in frame.SpriteLayers)
                        {
                            Vector2 size = new Vector2Int(8, 8);
                            Vector2 pos = new Vector2(layer.XPosition, layer.YPosition);
                            if ((layer.Scale.HasValue && layer.Scale.Value != Vector2.one) || (layer.Rotation.HasValue && layer.Rotation.Value != 0f))
                            {
                                Vector2 transformOrigin = new Vector2(layer.TransformOriginX, layer.TransformOriginY);
                                Vector2 relativePos = pos - transformOrigin; // Center relative to transform origin

                                // Scale first
                                Vector2 scale = Vector2.one;
                                if (layer.Scale.HasValue && layer.Scale.Value != Vector2.one)
                                {
                                    scale = layer.Scale.Value;
                                    float scaleX = layer.Scale.Value.x;
                                    float scaleY = layer.Scale.Value.y;
                                    if (scaleX == 0f || scaleY == 0f) continue;
                                    if (scaleX > 0f)
                                    {
                                        scaleX = Mathf.Ceil(size.x * scaleX) / (size.x);
                                    }
                                    else
                                    {
                                        scaleX = -Mathf.Ceil(size.x * -scaleX) / (size.x);
                                    }
                                    if (scaleY > 0f)
                                    {
                                        scaleY = Mathf.Ceil(size.y * scaleY) / (size.y);
                                    }
                                    else
                                    {
                                        scaleY = -Mathf.Ceil(size.y * -scaleY) / (size.y);
                                    }
                                    scale = new Vector2(scaleX, scaleY);

                                    relativePos = Vector2.Scale(relativePos, layer.Scale.Value);
                                    size = Vector2.Scale(size, scale);
                                }
                                // Then rotate
                                float rotation = 0f;
                                if (layer.Rotation.HasValue && layer.Rotation.Value != 0)
                                {
                                    rotation = -layer.Rotation.Value;
                                    relativePos = Quaternion.Euler(0f, 0f, rotation) * relativePos;
                                    //size = Quaternion.Euler(0f, 0f, rotation) * size;
                                    // Calculate new bounding box
                                    var newY = Mathf.Abs(size.x * Mathf.Sin(Mathf.Deg2Rad * rotation)) + Mathf.Abs(size.y * Mathf.Cos(Mathf.Deg2Rad * rotation));
                                    var newX = Mathf.Abs(size.x * Mathf.Cos(Mathf.Deg2Rad * rotation)) + Mathf.Abs(size.y * Mathf.Sin(Mathf.Deg2Rad * rotation));
                                    size = new Vector2(newX, newY);
                                }
                                pos = transformOrigin + relativePos;
                            }
                            int x = Mathf.FloorToInt(pos.x);
                            int y = Mathf.FloorToInt(pos.y);
                            if (x < min.x) min.x = x;
                            if (y < min.y) min.y = y;
                            int maxX = Mathf.CeilToInt(pos.x + size.x);
                            int maxY = Mathf.CeilToInt(pos.y + size.y);
                            if (maxX > max.x) max.x = maxX;
                            if (maxY > max.y) max.y = maxY;
                        }
                    }
                    Vector2Int frameImgSize = max - min;
                    if (frameImgSize.x == 0 || frameImgSize.y == 0) continue;

                    foreach (var frame in anim.Frames)
                    {

                        using (var frameImg = new MagickImage(new byte[frameImgSize.x * frameImgSize.y * 4], new PixelReadSettings(frameImgSize.x, frameImgSize.y, StorageType.Char, PixelMapping.ABGR)))
                        {
                            frameImg.FilterType = FilterType.Point;
                            frameImg.Interpolate = PixelInterpolateMethod.Nearest;
                            int layerIndex = 0;
                            foreach (var layer in frame.SpriteLayers)
                            {
                                if (layer.ImageIndex >= sprites.Length)
                                {
                                    Debug.LogWarning($"Out of bounds sprite index {layer.ImageIndex}/{sprites.Length}");
                                    continue;
                                }

                                MagickImage img = (MagickImage)sprites[layer.ImageIndex].Clone();
                                Vector2 size = new Vector2(img.Width, img.Height);
                                img.FilterType = FilterType.Point;
                                img.Interpolate = PixelInterpolateMethod.Nearest;
                                img.BackgroundColor = MagickColors.Transparent;
                                if (layer.IsFlippedHorizontally)
                                    img.Flop();

                                if (layer.IsFlippedVertically)
                                    img.Flip();
                                Vector2 pos = new Vector2(layer.XPosition, layer.YPosition);
                                if ((layer.Scale.HasValue && layer.Scale.Value != Vector2.one) || (layer.Rotation.HasValue && layer.Rotation.Value != 0f))
                                {
                                    pos += size / 2f;
                                    Vector2 transformOrigin = new Vector2(layer.TransformOriginX, layer.TransformOriginY);
                                    Vector2 relativePos = pos - transformOrigin; // Center relative to transform origin
                                    Vector2Int canvas = Vector2Int.one * 128;
                                    img.Extent(canvas.x, canvas.y, Gravity.Center); // 2x max size

                                    // Scale first
                                    Vector2 scale = Vector2.one;
                                    if (layer.Scale.HasValue && layer.Scale.Value != Vector2.one)
                                    {
                                        scale = layer.Scale.Value;
                                        float scaleX = layer.Scale.Value.x;
                                        float scaleY = layer.Scale.Value.y;
                                        if (scaleX == 0f || scaleY == 0f) continue;
                                        if (scaleX > 0f)
                                        {
                                            scaleX = Mathf.Ceil(size.x * scaleX) / (size.x);
                                        }
                                        else
                                        {
                                            scaleX = -Mathf.Ceil(size.x * -scaleX) / (size.x);
                                        }
                                        if (scaleY > 0f)
                                        {
                                            scaleY = Mathf.Ceil(size.y * scaleY) / (size.y);
                                        }
                                        else
                                        {
                                            scaleY = -Mathf.Ceil(size.y * -scaleY) / (size.y);
                                        }
                                        scale = new Vector2(scaleX, scaleY);

                                        relativePos = Vector2.Scale(relativePos, layer.Scale.Value);
                                        size = Vector2.Scale(size, layer.Scale.Value);
                                    }
                                    // Then rotate
                                    float rotation = 0f;
                                    if (layer.Rotation.HasValue && layer.Rotation.Value != 0)
                                    {
                                        rotation = -layer.Rotation.Value;
                                        relativePos = Quaternion.Euler(0f, 0f, rotation) * relativePos;
                                        size = Quaternion.Euler(0f, 0f, rotation) * size;
                                        // Calculate new bounding box
                                        /*var a = Mathf.Abs(x * Mathf.Sin(o)) + Mathf.Abs(y * Mathf.Cos(o));
                                        var b = Mathf.Abs(x * Mathf.Cos(o)) + Mathf.Abs(y * Mathf.Sin(o));*/


                                    }
                                    if (scale.x < 0f)
                                    {
                                        img.Flop();
                                        scale.x = -scale.x;
                                    }
                                    if (scale.y < 0f)
                                    {
                                        img.Flip();
                                        scale.y = -scale.y;
                                    }
                                    img.Distort(DistortMethod.ScaleRotateTranslate, new double[] { (canvas.x / 2), (canvas.y / 2), scale.x, scale.y, rotation });
                                    //img.Write(Path.Combine(animDir, $"{frameIndex}___{layerIndex}.png"), MagickFormat.Png);
                                    frameImg.Composite(img,
                                        Mathf.RoundToInt(transformOrigin.x + relativePos.x) - (canvas.x / 2) - min.x,
                                        Mathf.RoundToInt(transformOrigin.y + relativePos.y) - (canvas.y / 2) - min.y, CompositeOperator.Over);
                                }
                                else
                                {
                                    frameImg.Composite(img,
                                        Mathf.RoundToInt(pos.x) - min.x,
                                        Mathf.RoundToInt(pos.y) - min.y, CompositeOperator.Over);
                                }
                                layerIndex++;
                            }

                            frameImg.Write(Path.Combine(animDir, $"{frameIndex}.png"), MagickFormat.Png);
                        }

                        frameIndex++;
                    }

                    animIndex++;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
            }
            finally
            {
                if (sprites != null && sprites.Length > 0)
                    foreach (var s in sprites)
                        s?.Dispose();
            }
        }
        public virtual Unity_ObjGraphics GetCommonDesign(PuppetFile puppet)
        {
            bool is8bit = (puppet.Flags & PuppetFlags.Is8Bit) != 0;

            // Create the design
            Unity_ObjGraphics des = new()
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Unity_ObjAnimation>(),
            };

            int tilesCount = puppet.TileSetReference.Value.ImgData.Length / (is8bit ? 0x40 : 0x20);
            
            int numPalettes = puppet.PaletteReferences.Length;

            // Add sprites for each palette
            Color[][] pal_split = puppet.PaletteReferences.Select(x => Util.ConvertGBAPalette(x.Value.ColorData)).ToArray();
            for (int palIndex = 0; palIndex < numPalettes; palIndex++)
            {
                Texture2D tileSetTex = Util.ToTileSetTexture(
                    imgData: puppet.TileSetReference.Value.ImgData, 
                    pal: pal_split[palIndex], 
                    encoding: is8bit ? Util.TileEncoding.Linear_8bpp : Util.TileEncoding.Linear_4bpp, 
                    tileWidth: Constants.TileSize, 
                    flipY: false, 
                    flipTileY: true);

                // Extract every sprite
                for (int i = 0; i < tilesCount; i++)
                {
                    int x = i % 32;
                    int y = i / 32;
                    des.Sprites.Add(tileSetTex.CreateSprite(rect: new Rect(x * Constants.TileSize, y * Constants.TileSize, Constants.TileSize, Constants.TileSize)));
                }
            }

            Unity_ObjAnimationPart[] GetPartsForLayer(PuppetFile s, Animation a, int frame, AnimationChannel l)
            {
                if (l.ChannelType != AnimationChannelType.Sprite)
                    return Array.Empty<Unity_ObjAnimationPart>();

                Constants.Size spriteSize = Constants.GetSpriteShape(l.SpriteShape, l.SpriteSize);

                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[spriteSize.TilesWidth * spriteSize.TilesHeight];

                int imgIndex = l.TileIndex / (is8bit ? 2 : 1);

                // TODO: Support rotation and scaling. Works same as on GBA, but the affine index isn't stored in the channel.
                //float rot = l.GetRotation(a, s, frame);
                //Vector2 scl = l.GetScale(a, s, frame);
                for (int y = 0; y < spriteSize.TilesHeight; y++)
                {
                    for (int x = 0; x < spriteSize.TilesWidth; x++)
                    {
                        parts[y * spriteSize.TilesWidth + x] = new Unity_ObjAnimationPart
                        {
                            ImageIndex = tilesCount * l.PalIndex + (imgIndex + y * spriteSize.TilesWidth + x),
                            IsFlippedHorizontally = l.HorizontalFlip,
                            IsFlippedVertically = l.VerticalFlip,
                            XPosition = (l.XPosition + (l.HorizontalFlip ? (spriteSize.TilesWidth - 1 - x) : x) * Constants.TileSize),
                            YPosition = (l.YPosition + (l.VerticalFlip ? (spriteSize.TilesHeight - 1 - y) : y) * Constants.TileSize),
                            //Rotation = rot,
                            //Scale = scl,
                            TransformOriginX = (l.XPosition + spriteSize.TilesWidth * Constants.TileSize / 2f),
                            TransformOriginY = (l.YPosition + spriteSize.TilesHeight * Constants.TileSize / 2f)
                        };
                    }
                }
                return parts;
            }

            // Add animations
            foreach (Animation a in puppet.Animations)
            {
                Unity_ObjAnimation unityAnim = new()
                {
                    AnimSpeed = a.AnimSpeed,
                };

                var frames = new List<Unity_ObjAnimationFrame>();

                for (int i = 0; i < a.FramesCount; i++)
                {
                    frames.Add(new Unity_ObjAnimationFrame(a.Frames[i].Channels.SelectMany(l => GetPartsForLayer(puppet, a, i, l)).Reverse().ToArray()));
                }

                unityAnim.Frames = frames.ToArray();
                des.Animations.Add(unityAnim);
            }

            return des;
        }

        public override async UniTask<Unity_Level> LoadAsync(Context context)
        {
            ROMBase rom = FileFactory.Read<ROMBase>(context, ROMFilePath);
            OnyxFileResolver resolver = new(rom.Header);
            context.StoreObject(OnyxFileResolver.Key, resolver);

            var s = context.Deserializer;
            foreach (string filePath in resolver.NDS_EnumerateFilePaths())
                resolver.NDS_DeserializeFile(s, filePath);

            throw new NotImplementedException();
        }

        public override async UniTask LoadFilesAsync(Context context)
        {
            await context.AddLinearFileAsync(ROMFilePath);
        }
    }
}