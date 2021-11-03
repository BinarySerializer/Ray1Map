using BinarySerializer;
using System.Collections.Generic;
using System.Linq;

namespace Ray1Map.Gameloft
{
    public class Unity_ObjectManager_GameloftRRR : Unity_ObjectManager
    {
        public Unity_ObjectManager_GameloftRRR(Context context, PuppetData[] puppets, Gameloft_RRR_Objects.Object[] objects) : base(context)
        {
            Puppets = puppets;
            ObjectIDDictionary = new Dictionary<short, int>();
            for(int i = 0; i < objects.Length; i++) {
                if (objects[i].ObjectID != 0) {
                    ObjectIDDictionary[objects[i].ObjectID] = i;
                }
            }
        }

        public PuppetData[] Puppets { get; }
        public Dictionary<short, int> ObjectIDDictionary { get; set; }

        public override Unity_SpriteObject GetMainObject(IList<Unity_SpriteObject> objects) => objects.OfType<Unity_Object_GameloftRRR>().FindItem(x => x.PuppetIndex == 0);

        public override string[] LegacyDESNames => Puppets.Select(x => x.DisplayName).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class PuppetData
        {
            public PuppetData(int index, Gameloft_RRR_PuppetResourceList.ResourceReference puppetReference, Gameloft_BaseManager.Unity_Gameloft_ObjGraphics puppet, string name = null)
            {
                Index = index;
                PuppetReference = puppetReference;
                Puppet = puppet;
                Name = name;
            }

            public int Index { get; }

            public Gameloft_RRR_PuppetResourceList.ResourceReference PuppetReference { get; }

            public Gameloft_BaseManager.Unity_Gameloft_ObjGraphics Puppet { get; }

            public string Name { get; }

            public string DisplayName => Name == null ? $"{Index}" : $"{Index}_{Name}";
        }
    }
}