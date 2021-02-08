using R1Engine.Serialize;
using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public class Unity_ObjectManager_GameloftRRR : Unity_ObjectManager
    {
        public Unity_ObjectManager_GameloftRRR(Context context, PuppetData[] puppets) : base(context)
        {
            Puppets = puppets;
        }

        public PuppetData[] Puppets { get; }

        public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.OfType<Unity_Object_GameloftRRR>().FindItem(x => x.Object.Type == 0);

        public override string[] LegacyDESNames => Puppets.Select(x => x.DisplayName).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class PuppetData
        {
            public PuppetData(int index, Gameloft_RRR_PuppetResourceList.ResourceReference puppetReference, Unity_ObjGraphics puppet, string name = null)
            {
                Index = index;
                PuppetReference = puppetReference;
                Puppet = puppet;
                Name = name;
            }

            public int Index { get; }

            public Gameloft_RRR_PuppetResourceList.ResourceReference PuppetReference { get; }

            public Unity_ObjGraphics Puppet { get; }

            public string Name { get; }

            public string DisplayName => Name == null ? $"{Index}" : $"{Index}_{Name}";
        }
    }
}