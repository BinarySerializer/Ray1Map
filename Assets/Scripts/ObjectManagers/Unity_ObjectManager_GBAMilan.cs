using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAMilan : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAMilan(Context context, ModelData[] actorModels) : base(context)
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

        public override string[] LegacyDESNames => ActorModels.Select(x => x.Model.ActorID).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class ModelData
        {
            public ModelData(int index, GBA_Milan_ActorModel model, Unity_ObjGraphics graphics)
            {
                Index = index;
                Model = model;
                Graphics = graphics;
            }

            public int Index { get; }

            public GBA_Milan_ActorModel Model { get; }

            public Unity_ObjGraphics Graphics { get; }
        }
    }
}