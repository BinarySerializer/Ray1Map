using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class Unity_ObjectManager_GBC : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBC(Context context, ActorModel[] actorModels) : base(context)
        {
            ActorModels = actorModels;

            for (int i = 0; i < (ActorModels?.Length ?? 0); i++)
                ActorModelsLookup[ActorModels[i]?.Index ?? 0] = i;
        }

        public ActorModel[] ActorModels { get; }
        public Dictionary<int, int> ActorModelsLookup { get; } = new Dictionary<int, int>();

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.Cast<Unity_Object_GBC>().FindItem(x => x.Actor.ActorID == 0);

        public override string[] LegacyDESNames => ActorModels.Select(x => x.Index.ToString()).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class ActorModel
        {
            public ActorModel(int index, GBC_Action[] actions, Unity_ObjGraphics graphics)
            {
                Index = index;
                Actions = actions.OrderBy(x => x.ActionID).ToArray();
                Graphics = graphics;

                for (int i = 0; i < (Actions?.Length ?? 0); i++)
                    ActionsLookup[Actions[i]?.ActionID ?? 0] = i;
            }

            public int Index { get; }

            public Dictionary<int, int> ActionsLookup { get; } = new Dictionary<int, int>();
            public GBC_Action[] Actions { get; }

            public Unity_ObjGraphics Graphics { get; }
        }
    }
}