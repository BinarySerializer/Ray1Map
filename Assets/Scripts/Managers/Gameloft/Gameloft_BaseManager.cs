using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using ImageMagick;
using R1Engine.Serialize;
using UnityEngine;

namespace R1Engine
{
    public abstract class Gameloft_BaseManager : IGameManager
    {
        public abstract string[] ResourceFiles { get; }
        public abstract string[] SingleResourceFiles { get; }

        public virtual GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
        {
            new GameInfo_World(0, Enumerable.Range(0, 1).ToArray()),
        });

        public GameAction[] GetGameActions(GameSettings settings) => new GameAction[]
        {
            new GameAction("Export Resources", false, true, (input, output) => ExportResourcesAsync(settings, output, ExportMethod.Resources)),
            new GameAction("Export Sprites", false, true, (input, output) => ExportResourcesAsync(settings, output, ExportMethod.Sprites)),
            new GameAction("Export Layer Groups", false, true, (input, output) => ExportResourcesAsync(settings, output, ExportMethod.LayerGroups)),
            new GameAction("Export Animations", false, true, (input, output) => ExportResourcesAsync(settings, output, ExportMethod.Animations)),
        };

        public enum ExportMethod {
            Resources,
            Sprites,
            LayerGroups,
            Animations
        }

        public async UniTask ExportResourcesAsync(GameSettings settings, string outputDir, ExportMethod exportMethod) {
            using (var context = new Context(settings)) {
                foreach(var filePath in ResourceFiles) {
                    var f = await context.AddLinearSerializedFileAsync(filePath);
                    SerializerObject s = context.Deserializer;
                    s.DoAt(f.StartPointer, () => {
                        try {
                            var resf = s.SerializeObject<Gameloft_ResourceFile>(default, name: f.filePath);
                            ExportResourceFile(resf, s, Path.Combine(outputDir,filePath), exportMethod);
                        } catch (Exception ex) {
                            Debug.LogError(ex);
                        }
                    });
                    await Controller.WaitIfNecessary();
                }
                foreach (var filePath in SingleResourceFiles) {
                    var f = await context.AddLinearSerializedFileAsync(filePath);
                    SerializerObject s = context.Deserializer;
                    s.DoAt(f.StartPointer, () => {
                        try {
                            var bytes = s.SerializeArray<byte>(default, s.CurrentLength, name: f.filePath);
                            ExportResourceFileData(bytes,"data", Path.Combine(outputDir, filePath), exportMethod);
                        } catch (Exception ex) {
                            Debug.LogError(ex);
                        }
                    });
                }
            }
        }

        public enum ResourceType {
            Puppet,
            PNG,
            MIDI,
            WAV,
            Other
        }

        public void ExportResourceFile(Gameloft_ResourceFile resf, SerializerObject s, string outputDir, ExportMethod exportMethod) {
            for (int i = 0; i < resf.ResourcesCount; i++) {
                var res = resf.SerializeResource<Gameloft_DummyResource>(s, default, i, name: $"Resource_{i}");
                if(res == null) continue;
                var restype = ResourceType.Other;
                if (res.Data.Length >= 5) {
                    using (Reader reader = new Reader(new MemoryStream(res.Data), isLittleEndian: false)) {
                        if (reader.ReadUInt32() == 0x89504E47) {
                            restype = ResourceType.PNG;
                        }
                        reader.BaseStream.Position = 0;
                        if (reader.ReadUInt32() == 0x4D546864) {
                            restype = ResourceType.MIDI;
                        }
                        reader.BaseStream.Position = 0;
                        if (reader.ReadUInt16() == 0xDF03) {
                            restype = ResourceType.Puppet;
                        }
                        reader.BaseStream.Position = 0;
                        if (reader.ReadUInt32() == 0x52494646) {
                            restype = ResourceType.WAV;
                        }
                        reader.BaseStream.Position = 0;
                    }
                }
                string extension = "dat";
                switch (restype) {
                    case ResourceType.Puppet: extension = "puppet"; break;
                    case ResourceType.MIDI: extension = "mid"; break;
                    case ResourceType.PNG: extension = "png"; break;
                    case ResourceType.WAV: extension = "wav"; break;
                }
                switch(exportMethod) {
                    case ExportMethod.Resources:
                        Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}.{extension}"), res.Data);
                        break;
                    case ExportMethod.Sprites:
                    case ExportMethod.LayerGroups:
                        if (restype == ResourceType.Puppet) {
                            //Debug.Log($"Reading {resf.Offset.file.filePath} - {i}");
                            var puppet = resf.SerializeResource<Gameloft_Puppet>(s, default, i, name: $"Puppet_{resf.Offset.file.filePath}_{i}");
                            ExportPuppet(puppet, Path.Combine(outputDir, $"{i}"), exportMethod);
                        }
                        break;
                    case ExportMethod.Animations:
                        if (restype == ResourceType.Puppet) {
                            //Debug.Log($"Reading {resf.Offset.file.filePath} - {i}");
                            var puppet = resf.SerializeResource<Gameloft_Puppet>(s, default, i, name: $"Puppet_{resf.Offset.file.filePath}_{i}");
                            ExportAnimations(puppet, Path.Combine(outputDir, $"{i}"));
                        }
                        break;
                }
            }
        }

        public void ExportResourceFileData(byte[] data, string filename, string outputDir, ExportMethod exportMethod) {
            if (data == null) return;
            var restype = ResourceType.Other;
            if (data.Length >= 5) {
                using (Reader reader = new Reader(new MemoryStream(data), isLittleEndian: false)) {
                    if (reader.ReadUInt32() == 0x89504E47) {
                        restype = ResourceType.PNG;
                    }
                    reader.BaseStream.Position = 0;
                    if (reader.ReadUInt32() == 0x4D546864) {
                        restype = ResourceType.MIDI;
                    }
                    reader.BaseStream.Position = 0;
                    if (reader.ReadUInt16() == 0xDF03) {
                        restype = ResourceType.Puppet;
                    }
                    reader.BaseStream.Position = 0;
                    if (reader.ReadUInt32() == 0x52494646) {
                        restype = ResourceType.WAV;
                    }
                    reader.BaseStream.Position = 0;
                }
            }
            string extension = "dat";
            switch (restype) {
                case ResourceType.Puppet: extension = "puppet"; break;
                case ResourceType.MIDI: extension = "mid"; break;
                case ResourceType.PNG: extension = "png"; break;
                case ResourceType.WAV: extension = "wav"; break;
            }
            switch (exportMethod) {
                case ExportMethod.Resources:
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{filename}.{extension}"), data);
                    break;
            }
        }


        public Texture2D[][] GetPuppetImages(Gameloft_Puppet puppet, bool flipY = true) {
            Texture2D[][] texs = new Texture2D[puppet.ImagesCount][];
            for (int i = 0; i < puppet.ImagesCount; i++) {
                var id = puppet.ImageDescriptors[i];
                var pal = puppet.Palettes[id.Palette];
                byte[] imageData = puppet.Images[i].Convert(puppet.ImageFormat, id.Width, id.Height, pal.PaletteLength);
                texs[i] = new Texture2D[pal.PaletteCount];
                for (int p = 0; p < pal.PaletteCount; p++) {
                    Texture2D tex = TextureHelpers.CreateTexture2D(id.Width, id.Height);
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            tex.SetPixel(x, flipY ? tex.height - 1 - y : y, pal.Palettes[p][imageData[y * tex.width + x]].GetColor());
                        }
                    }
                    tex.Apply();
                    texs[i][p] = tex;
                }
            }
            return texs;
        }

        public void ExportPuppet(Gameloft_Puppet puppet, string outputDir, ExportMethod exportMethod) {
            Texture2D[][] texs = GetPuppetImages(puppet);
            Texture2D[][] layerGroupTexs = new Texture2D[puppet.LayerGroupsCount][];
            if (exportMethod == ExportMethod.LayerGroups || exportMethod == ExportMethod.Animations) {
                for (int i = 0; i < puppet.LayerGroupsGraphics.Length; i++) {
                    var lg = puppet.LayerGroupsGraphics[i];
                    var dim = puppet.LayerGroupsCollision[i];
                    if (lg.Length == 0) continue;
                    var minX = Enumerable.Range(lg.StartIndex, lg.Length).Select(li => puppet.Layers[li])
                        .Min(layer => layer.XPosition);
                    var minY = Enumerable.Range(lg.StartIndex, lg.Length).Select(li => puppet.Layers[li])
                        .Min(layer => layer.YPosition);
                    var width = Enumerable.Range(lg.StartIndex, lg.Length).Select(li => puppet.Layers[li])
                        .Max(layer => layer.XPosition + puppet.ImageDescriptors[layer.ImageIndex].Width) - minX;
                    var height = Enumerable.Range(lg.StartIndex, lg.Length).Select(li => puppet.Layers[li])
                        .Max(layer => layer.YPosition + puppet.ImageDescriptors[layer.ImageIndex].Height) - minY;
                    var numPalettes = Enumerable.Range(lg.StartIndex, lg.Length).Select(li => puppet.Layers[li])
                        .Max(layer => texs[layer.ImageIndex].Length);
                    layerGroupTexs[i] = new Texture2D[numPalettes];
                    for (int p = 0; p < numPalettes; p++) {
                        Texture2D tex = TextureHelpers.CreateTexture2D(width, height, clear: true);
                        for (int li = lg.StartIndex; li < lg.StartIndex + lg.Length; li++) {
                            var layer = puppet.Layers[li];
                            var id = puppet.ImageDescriptors[layer.ImageIndex];
                            bool flipX = layer.Flags.HasFlag(Gameloft_Puppet.AnimationLayer.Flag.HorizontalFlip);
                            bool flipY = layer.Flags.HasFlag(Gameloft_Puppet.AnimationLayer.Flag.VerticalFlip);
                            var imageTex = texs[layer.ImageIndex][p % texs[layer.ImageIndex].Length];
                            var texPixels = imageTex.GetPixels();
                            for (int y = 0; y < imageTex.height; y++) {
                                for (int x = 0; x < imageTex.width; x++) {
                                    int curX = layer.XPosition + (flipX ? (id.Width - 1 - x) : x) - minX;
                                    int curY = layer.YPosition + (flipY ? (id.Height - 1 - y) : y) - minY;
                                    var pixel = texPixels[(imageTex.height - 1 - y) * imageTex.width + x];
                                    if (pixel.a > 0) {
                                        tex.SetPixel(curX, tex.height - 1 - curY, pixel);
                                    }
                                }
                            }
                        }
                        tex.Apply();
                        layerGroupTexs[i][p] = tex;
                    }
                }
            }
            switch (exportMethod) {
                case ExportMethod.Sprites:
                    for (int i = 0; i < texs.Length; i++) {
                        for (int p = 0; p < texs[i].Length; p++) {
                            Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}_{p}.png"), texs[i][p].EncodeToPNG());
                        }
                    }
                    break;
                case ExportMethod.LayerGroups:
                    for (int i = 0; i < layerGroupTexs.Length; i++) {
                        for (int p = 0; p < layerGroupTexs[i]?.Length; p++) {
                            Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}_{p}.png"), layerGroupTexs[i][p].EncodeToPNG());
                        }
                    }
                    break;
                case ExportMethod.Animations:
                    for (int i = 0; i < puppet.Animations.Length; i++) {
                        var anim = puppet.Animations[i];

                        var minX = Enumerable.Range(anim.FrameIndex, anim.Length).Select(f => puppet.Frames[f])
                            .Min(f => f.XPosition + 
                                Enumerable.Range(puppet.LayerGroupsGraphics[f.LayerGroupIndex].StartIndex, puppet.LayerGroupsGraphics[f.LayerGroupIndex].Length).Select(li => puppet.Layers[li])
                                .Min(layer => layer.XPosition));
                        var minY = Enumerable.Range(anim.FrameIndex, anim.Length).Select(f => puppet.Frames[f])
                            .Min(f => f.YPosition +
                                Enumerable.Range(puppet.LayerGroupsGraphics[f.LayerGroupIndex].StartIndex, puppet.LayerGroupsGraphics[f.LayerGroupIndex].Length).Select(li => puppet.Layers[li])
                                .Min(layer => layer.YPosition));
                        var width = Enumerable.Range(anim.FrameIndex, anim.Length).Select(f => puppet.Frames[f])
                            .Max(f => f.XPosition +
                                Enumerable.Range(puppet.LayerGroupsGraphics[f.LayerGroupIndex].StartIndex, puppet.LayerGroupsGraphics[f.LayerGroupIndex].Length).Select(li => puppet.Layers[li])
                                .Max(layer => layer.XPosition + puppet.ImageDescriptors[layer.ImageIndex].Width)) - minX;
                        var height = Enumerable.Range(anim.FrameIndex, anim.Length).Select(f => puppet.Frames[f])
                            .Max(f => f.YPosition +
                                Enumerable.Range(puppet.LayerGroupsGraphics[f.LayerGroupIndex].StartIndex, puppet.LayerGroupsGraphics[f.LayerGroupIndex].Length).Select(li => puppet.Layers[li])
                                .Max(layer => layer.YPosition + puppet.ImageDescriptors[layer.ImageIndex].Height)) - minY;

                        var numPalettes = Enumerable.Range(anim.FrameIndex, anim.Length).Select(f => puppet.Frames[f])
                            .Max(f => layerGroupTexs[f.LayerGroupIndex]?.Length ?? 0);

                        for (int p = 0; p < numPalettes; p++) {
                            Texture2D[] frameTexs = new Texture2D[anim.Length];
                            for (int f = 0; f < anim.Length; f++) {
                                var frame = puppet.Frames[anim.FrameIndex + f];
                                Texture2D tex = TextureHelpers.CreateTexture2D(width, height, clear: true);
                                var frameX = frame.XPosition - minX;
                                var frameY = frame.YPosition - minY;
                                bool flipX = frame.Flags.HasFlag(Gameloft_Puppet.AnimationFrame.Flag.HorizontalFlip);
                                bool flipY = frame.Flags.HasFlag(Gameloft_Puppet.AnimationFrame.Flag.VerticalFlip);
                                if (layerGroupTexs[frame.LayerGroupIndex].Length == 0) continue;
                                var imageTex = layerGroupTexs[frame.LayerGroupIndex][p % layerGroupTexs[frame.LayerGroupIndex].Length];
                                var texPixels = imageTex.GetPixels();
                                for (int y = 0; y < imageTex.height; y++) {
                                    for (int x = 0; x < imageTex.width; x++) {
                                        int curX = frameX + (flipX ? (imageTex.width - 1 - x) : x);
                                        int curY = frameY + (flipY ? (imageTex.height - 1 - y) : y);
                                        var pixel = texPixels[(imageTex.height - 1 - y) * imageTex.width + x];
                                        if (pixel.a > 0) {
                                            tex.SetPixel(curX, tex.height - 1 - curY, pixel);
                                        }
                                    }
                                }
                                tex.Apply();
                                frameTexs[f] = tex;
                                Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}_{f}.png"), frameTexs[f].EncodeToPNG());
                            }
                        }
                    }
                    break;
            }
        }


        public class Unity_Gameloft_ObjGraphics {
            public Sprite[][] Sprites;
            public Unity_ObjAnimation[] Animations;
        }

        public virtual Unity_Gameloft_ObjGraphics GetCommonDesign(Gameloft_Puppet puppet) {
            // Create the design
            var des = new Unity_Gameloft_ObjGraphics {
            };

            if (puppet == null) {
                des.Sprites = new Sprite[0][];
                des.Animations = new Unity_ObjAnimation[0];
                return des;
            }

            Texture2D[][] texs = new Texture2D[puppet.ImagesCount][];
            des.Sprites = new Sprite[puppet.Palettes.Max(p => p.PaletteCount)][];
            for(int p = 0; p < des.Sprites.Length; p++) {
                des.Sprites[p] = new Sprite[puppet.ImagesCount];
            }
            for (int i = 0; i < puppet.ImagesCount; i++) {
                var id = puppet.ImageDescriptors[i];
                var pal = puppet.Palettes[id.Palette];
                byte[] imageData = puppet.Images[i].Convert(puppet.ImageFormat, id.Width, id.Height, pal.PaletteLength);
                texs[i] = new Texture2D[pal.PaletteCount];
                for (int p = 0; p < pal.PaletteCount; p++) {
                    Texture2D tex = TextureHelpers.CreateTexture2D(id.Width, id.Height);
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            tex.SetPixel(x, tex.height - 1 - y, pal.Palettes[p][imageData[y * tex.width + x]].GetColor());
                        }
                    }
                    tex.Apply();
                    texs[i][p] = tex;
                    des.Sprites[p][i] = tex.CreateSprite();
                }
                for (int p = 0; p < des.Sprites.Length; p++) {
                    if(des.Sprites[p][i] == null) des.Sprites[p][i] = des.Sprites[0][i];
                }
            }


            Unity_ObjAnimationPart[] GetPartsForFrame(
                Gameloft_Puppet.Animation a,
                Gameloft_Puppet.AnimationFrame f,
                Gameloft_Puppet.AnimationLayerGroupGraphics g) {
                Unity_ObjAnimationPart[] parts = new Unity_ObjAnimationPart[g.Length];
                var frameFlipX = f.Flags.HasFlag(Gameloft_Puppet.AnimationFrame.Flag.HorizontalFlip);
                var frameFlipY = f.Flags.HasFlag(Gameloft_Puppet.AnimationFrame.Flag.VerticalFlip);
                for (int i = 0; i < g.Length; i++) {
                    var l = puppet.Layers[g.StartIndex + i];
                    var flipX = l.Flags.HasFlag(Gameloft_Puppet.AnimationLayer.Flag.HorizontalFlip);
                    var flipY = l.Flags.HasFlag(Gameloft_Puppet.AnimationLayer.Flag.VerticalFlip);
                    parts[i] = new Unity_ObjAnimationPart {
                        ImageIndex = l.ImageIndex,
                        IsFlippedHorizontally = frameFlipX ^ flipX,
                        IsFlippedVertically = frameFlipY ^ flipY,
                        XPosition = f.XPosition + (frameFlipX ? -1 : 1) * l.XPosition + (frameFlipX ? -puppet.ImageDescriptors[l.ImageIndex].Width : 0),
                        YPosition = f.YPosition + (frameFlipY ? -1 : 1) * l.YPosition + (frameFlipY ? -puppet.ImageDescriptors[l.ImageIndex].Height : 0),
                    };
                }
                return parts;
            }

            Unity_ObjAnimationCollisionPart[] GetCollisionForFrame(
                Gameloft_Puppet.Animation a,
                Gameloft_Puppet.AnimationFrame f,
                Gameloft_Puppet.AnimationLayerGroupCollision c) {
                if(c.Width == 0 || c.Height == 0) return new Unity_ObjAnimationCollisionPart[0];
                var frameFlipX = f.Flags.HasFlag(Gameloft_Puppet.AnimationFrame.Flag.HorizontalFlip);
                var frameFlipY = f.Flags.HasFlag(Gameloft_Puppet.AnimationFrame.Flag.VerticalFlip);

                return new Unity_ObjAnimationCollisionPart[] {
                    new Unity_ObjAnimationCollisionPart() {
                        XPosition = (frameFlipX ? -1 : 1) * c.XPosition - (frameFlipX ? c.Width : 0),
                        YPosition = (frameFlipY ? -1 : 1) * c.YPosition - (frameFlipY ? c.Height : 0),
                        Width = c.Width,
                        Height = c.Height,
                        Type = Unity_ObjAnimationCollisionPart.CollisionType.VulnerabilityBox
                    }
                };
            }

            // Add animations
            des.Animations = new Unity_ObjAnimation[puppet.Animations.Length];
            for(int j = 0; j < puppet.Animations.Length; j++) {
                var a = puppet.Animations[j];
                var unityAnim = new Unity_ObjAnimation {
                    AnimSpeed = 4,
                };

                var frames = new List<Unity_ObjAnimationFrame>();

                for (int i = 0; i < a.Length; i++) {
                    var f = puppet.Frames[a.FrameIndex + i];
                    if(f.LayerGroupIndex >= puppet.LayerGroupsCount) continue;
                    var g = puppet.LayerGroupsGraphics[f.LayerGroupIndex];
                    var c = puppet.LayerGroupsCollision[f.LayerGroupIndex];
                    frames.Add(new Unity_ObjAnimationFrame(
                        GetPartsForFrame(a, f,g),
                        GetCollisionForFrame(a,f,c)
                        ));
                }

                unityAnim.Frames = frames.ToArray();
                unityAnim.AnimSpeeds = Enumerable.Range(a.FrameIndex, a.Length).Select(f => (int)puppet.Frames[f].Duration).ToArray();
                des.Animations[j] = unityAnim;
            }

            return des;
        }


        protected void ExportAnimations(Gameloft_Puppet spr, string outputDir) {
            MagickImage[][] sprites = null;

            try {
                var commonDesign = GetCommonDesign(spr);

                // Convert Texture2D to MagickImage
                sprites = commonDesign.Sprites.Select(x => x.Select(i => i?.ToMagickImage()).ToArray()).ToArray();


                // Export every animation
                for (int p = 0; p < sprites.Length; p++) {
                    var animIndex = 0;
                    foreach (var anim in commonDesign.Animations) {
                        var frameIndex = 0;
                        /*var animDir = Path.Combine(outputDir, $"{animIndex}-{anim.AnimSpeed}");
                        Directory.CreateDirectory(animDir);*/
                        Directory.CreateDirectory(outputDir);
                        if (anim.Frames == null || anim.Frames.Length == 0) continue;

                        Vector2Int min = new Vector2Int();
                        Vector2Int max = new Vector2Int();
                        foreach (var frame in anim.Frames) {
                            foreach (var layer in frame.SpriteLayers) {
                                var sprite = commonDesign.Sprites[p][layer.ImageIndex] ?? commonDesign.Sprites[0][layer.ImageIndex];
                                Vector2Int size = new Vector2Int((int)sprite.textureRect.width, (int)sprite.textureRect.height);
                                Vector2Int pos = new Vector2Int(layer.XPosition, layer.YPosition);
                                int x = pos.x;
                                int y = pos.y;
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

                        using (MagickImageCollection collection = new MagickImageCollection()) {
                            //int index = 0;

                            foreach (var frame in anim.Frames) {
                                var frameImg = new MagickImage(new byte[frameImgSize.x * frameImgSize.y * 4], new PixelReadSettings(frameImgSize.x, frameImgSize.y, StorageType.Char, PixelMapping.ABGR));
                                frameImg.FilterType = FilterType.Point;
                                frameImg.Interpolate = PixelInterpolateMethod.Nearest;
                                int layerIndex = 0;
                                foreach (var layer in frame.SpriteLayers) {
                                    if (layer.ImageIndex >= sprites[0].Length) {
                                        Debug.LogWarning($"Out of bounds sprite index {layer.ImageIndex}/{sprites[0].Length}");
                                        continue;
                                    }

                                    MagickImage img = (MagickImage)(sprites[p][layer.ImageIndex] ?? sprites[0][layer.ImageIndex]).Clone();
                                    Vector2 size = new Vector2(img.Width, img.Height);
                                    img.FilterType = FilterType.Point;
                                    img.Interpolate = PixelInterpolateMethod.Nearest;
                                    img.BackgroundColor = MagickColors.Transparent;
                                    if (layer.IsFlippedHorizontally)
                                        img.Flop();

                                    if (layer.IsFlippedVertically)
                                        img.Flip();
                                    Vector2 pos = new Vector2(layer.XPosition, layer.YPosition);
                                    frameImg.Composite(img,
                                        Mathf.RoundToInt(pos.x) - min.x,
                                        Mathf.RoundToInt(pos.y) - min.y, CompositeOperator.Over);
                                    layerIndex++;
                                }

                                //frameImg.Write(Path.Combine(animDir, $"{frameIndex}.png"), MagickFormat.Png);

                                // For gif
                                collection.Add(frameImg);
                                collection[frameIndex].AnimationDelay = anim.AnimSpeeds[frameIndex];
                                collection[frameIndex].AnimationTicksPerSecond = 10;
                                collection[frameIndex].Trim();
                                collection[frameIndex].GifDisposeMethod = GifDisposeMethod.Background;

                                frameIndex++;
                            }

                            // Save gif
                            collection.Write(Path.Combine(outputDir, $"{animIndex}_{p}-{anim.AnimSpeed}.gif"));
                        }

                        animIndex++;
                    }
                }
            } catch (Exception ex) {
                Debug.LogError($"Message: {ex.Message}{Environment.NewLine}StackTrace: {ex.StackTrace}");
            } finally {
                if (sprites != null)
                    foreach (var s in sprites)
                        foreach(var i in s)
                            i?.Dispose();
            }
        }

        public async UniTask SaveLevelAsync(Context context, Unity_Level level) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
        }

        public virtual async UniTask LoadFilesAsync(Context context) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
        }

		public virtual async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
            await UniTask.CompletedTask;
            throw new NotImplementedException();
		}
	}
}