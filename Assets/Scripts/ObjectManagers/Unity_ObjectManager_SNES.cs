using R1Engine.Serialize;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    public class Unity_ObjectManager_SNES : Unity_ObjectManager
    {
        public Unity_ObjectManager_SNES(Context context, State[] states, Sprite[] sprites) : base(context)
        {
            States = states;
            Sprites = sprites;
        }

        public State[] States { get; }
        public Sprite[] Sprites { get; }

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

        public override string[] LegacyDESNames => States.Select((x,i) => i.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;
    }
}