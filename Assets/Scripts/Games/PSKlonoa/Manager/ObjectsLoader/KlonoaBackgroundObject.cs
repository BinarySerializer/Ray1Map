using BinarySerializer.Klonoa.DTP;
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
                    (Texture2D[] frames, var speed) = ObjLoader.Manager.GetBackgroundFrames(ObjLoader.Loader, ObjLoader, ObjLoader.Loader.BackgroundPack, Obj);

                    ObjLoader.BackgroundLayers.Add(new KlonoaBackgroundLayer(Obj, frames, speed));
                    break;

                case BackgroundGameObject.BackgroundGameObjectType.Clear_Gradient:
                case BackgroundGameObject.BackgroundGameObjectType.Clear:
                    ObjLoader.BackgroundClears.Add(Obj.Data_Clear);
                    break;
            }
        }
    }
}