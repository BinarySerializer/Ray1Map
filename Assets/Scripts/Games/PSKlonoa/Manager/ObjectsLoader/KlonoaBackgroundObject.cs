using System.Collections.Generic;
using System.Linq;
using BinarySerializer.Klonoa.DTP;
using BinarySerializer.PS1;
using UnityEngine;

namespace Ray1Map.PSKlonoa
{
    public class KlonoaBackgroundObject : KlonoaObject
    {
        public KlonoaBackgroundObject(KlonoaObjectsLoader objLoader, BackgroundGameObject obj) : base(objLoader)
        {
            Obj = obj;
        }

        public BackgroundGameObject Obj { get; }

        public override void LoadAnimations()
        {
            switch (Obj.Type)
            {
                case BackgroundGameObject.BackgroundGameObjectType.PaletteScroll:
                    BackgroundGameObjectData_PaletteScroll scroll = Obj.Data_PaletteScroll;

                    var frames = new byte[scroll.Length][];
                    var pal = VRAM.GetPixels8(0, 0, scroll.XPosition * 2, scroll.YPosition, 32);

                    frames[0] = pal;

                    for (int i = 1; i < frames.Length; i++)
                    {
                        // Clone the array to avoid modifying the previous frames
                        pal = (byte[])pal.Clone();

                        var firstColor_0 = pal[0];
                        var firstColor_1 = pal[1];

                        var index = scroll.StartIndex;
                        var endIndex = index + scroll.Length;

                        do
                        {
                            pal[index * 2] = pal[(index + 1) * 2];
                            pal[index * 2 + 1] = pal[(index + 1) * 2 + 1];

                            index += 1;
                        } while (index < endIndex);

                        pal[(endIndex - 1) * 2] = firstColor_0;
                        pal[(endIndex - 1) * 2 + 1] = firstColor_1;

                        frames[i] = pal;
                    }

                    var region = new RectInt(scroll.XPosition * 2, scroll.YPosition, 32, 1);
                    ObjLoader.BGPaletteAnimations.Add(new PS1VRAMAnimation(region, frames, scroll.Speed, false));
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.SetLightState:
                    // TODO: Implement
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.PaletteSwap:
                    // TODO: Implement
                    break;
            }
        }

        public override void LoadObject()
        {
            switch (Obj.Type)
            {
                case BackgroundGameObject.BackgroundGameObjectType.BackgroundLayer_19:
                case BackgroundGameObject.BackgroundGameObjectType.BackgroundLayer_22:
                    BackgroundPack_ArchiveFile bg = ObjLoader.Loader.BackgroundPack;

                    int celIndex = Obj.CELIndex;
                    int bgIndex = Obj.BGDIndex;

                    TIM tim = bg.TIMFiles.Files[celIndex];
                    CEL cel = bg.CELFiles.Files[celIndex];
                    BGD map = bg.BGDFiles.Files[bgIndex];

                    bool is8bit = tim.ColorFormat == TIM.TIM_ColorFormat.BPP_8;
                    int palLength = (is8bit ? 256 : 16) * 2;

                    var anims = new HashSet<PS1VRAMAnimation>();

                    if (ObjLoader.BGPaletteAnimations.Any())
                    {
                        foreach (var clut in map.Map.Select(x => cel.Cells[x]).Select(x => x.ClutX | x.ClutY << 6).Distinct())
                        {
                            var region = new RectInt((clut & 0x3F) * 16 * 2, clut >> 6, palLength, 1);

                            foreach (var anim in ObjLoader.Anim_GetBGAnimationsFromRegion(region))
                                anims.Add(anim);

                            if (anims.Count == ObjLoader.BGPaletteAnimations.Count)
                                break;
                        }
                    }

                    if (!anims.Any())
                    {
                        ObjLoader.BackgroundLayers.Add(new KlonoaBackgroundLayer(Obj, new Texture2D[]
                        {
                            GetLayerTexture()
                        }, 0));
                    }
                    else
                    {
                        int width = map.MapWidth * map.CellWidth;
                        int height = map.MapHeight * map.CellHeight;

                        var animatedTex = new PS1VRAMAnimatedTexture(width, height, true, tex =>
                        {
                            GetLayerTexture(tex);
                        }, anims.ToArray());

                        ObjLoader.Anim_Manager.AddAnimatedTexture(animatedTex);

                        ObjLoader.BackgroundLayers.Add(new KlonoaBackgroundLayer(Obj, animatedTex.Textures, animatedTex.Speed));
                    }
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.Clear_Gradient:
                case BackgroundGameObject.BackgroundGameObjectType.Clear:
                    ObjLoader.BackgroundClears.Add(Obj.Data_Clear);
                    break;
            }
        }

        public Texture2D GetLayerTexture(Texture2D tex = null)
        {
            BackgroundPack_ArchiveFile bg = ObjLoader.Loader.BackgroundPack;

            int celIndex = Obj.CELIndex;
            int bgIndex = Obj.BGDIndex;

            TIM tim = bg.TIMFiles.Files[celIndex];
            CEL cel = bg.CELFiles.Files[celIndex];
            BGD map = bg.BGDFiles.Files[bgIndex];

            return VRAM.FillMapTexture(tim, cel, map, tex);
        }
    }
}