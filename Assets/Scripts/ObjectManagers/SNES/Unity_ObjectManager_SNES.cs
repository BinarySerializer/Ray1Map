using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_SNES : Unity_ObjectManager
    {
        public Unity_ObjectManager_SNES(Context context, GraphicsGroup[] graphicsGroups) : base(context)
        {
            GraphicsGroups = graphicsGroups;
        }

        public GraphicsGroup[] GraphicsGroups { get; }

        public class GraphicsGroup
        {
            public GraphicsGroup(State[] states, SNES_Proto_ImageDescriptor[] imageDescriptors, Sprite[] sprites, bool isRecreated, string name)
            {
                States = states;
                ImageDescriptors = imageDescriptors;
                Sprites = sprites;
                IsRecreated = isRecreated;
                Name = name;
            }

            public State[] States { get; }
            public SNES_Proto_ImageDescriptor[] ImageDescriptors { get; }
            public Sprite[] Sprites { get; }
            public bool IsRecreated { get; }
            public string Name { get; }

            public class State
            {
                public State(SNES_Proto_State state, Unity_ObjAnimation animation)
                {
                    SNES_State = state;
                    Animation = animation;
                }

                public SNES_Proto_State SNES_State { get; }
                public Unity_ObjAnimation Animation { get; }
            }
        }

        public override string[] LegacyDESNames => GraphicsGroups.Select((x,i) => x.Name).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}