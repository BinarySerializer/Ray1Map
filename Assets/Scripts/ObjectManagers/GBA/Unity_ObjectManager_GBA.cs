
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class Unity_ObjectManager_GBA : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBA(Context context, ModelData[] actorModels) : base(context)
        {
            ActorModels = actorModels;
            if (ActorModels != null) {
                for (int i = 0; i < ActorModels.Length; i++) {
                    GraphicsDataLookup[ActorModels[i]?.Index ?? 0] = i;
                }
            }
        }

        public ModelData[] ActorModels { get; }
        public Dictionary<int, int> GraphicsDataLookup { get; } = new Dictionary<int, int>();

        public override Unity_SpriteObject GetMainObject(IList<Unity_SpriteObject> objects) => objects.OfType<Unity_Object_GBA>().FindItem(x => x.Actor.ActorID == 0);

        public override string[] LegacyDESNames => ActorModels.Select(x => x.DisplayName).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class ModelData
        {
            public ModelData(int index, GBA_Action[] actions, Unity_ObjGraphics[] puppets, string name = null)
            {
                Index = index;
                Actions = actions;
                Puppets = puppets;
                Name = name;
            }

            public int Index { get; }

            public GBA_Action[] Actions { get; }

            public Unity_ObjGraphics[] Puppets { get; }

            public string Name { get; }

            public string DisplayName => Name == null ? $"{Index}" : $"{Index}_{Name}";
        }
    }
}