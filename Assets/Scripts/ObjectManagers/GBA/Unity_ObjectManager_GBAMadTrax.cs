using BinarySerializer;

using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAMadTrax : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAMadTrax(Context context, GraphicsData[] graphicsDatas) : base(context)
        {
            GraphicsDatas = graphicsDatas;
        }

        public GraphicsData[] GraphicsDatas { get; }

        public class GraphicsData
        {
            public GraphicsData(Texture2D tex, Pointer pointer)
            {
                Pointer = pointer;
                Sprites = new Sprite[]
                {
                    tex.CreateSprite()
                };
                Animation = new Unity_ObjAnimation()
                {
                    Frames = new Unity_ObjAnimationFrame[]
                    {
                        new Unity_ObjAnimationFrame(new Unity_ObjAnimationPart[]
                        {
                            new Unity_ObjAnimationPart()
                        })
                    }
                };
            }

            public Pointer Pointer { get; }
            public Sprite[] Sprites { get; }
            public Unity_ObjAnimation Animation { get; }
        }
    }
}