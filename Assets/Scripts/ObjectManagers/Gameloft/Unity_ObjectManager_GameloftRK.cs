
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine
{
    public class Unity_ObjectManager_GameloftRK : Unity_ObjectManager
    {
        public Unity_ObjectManager_GameloftRK(Context context, PuppetData[] puppets) : base(context)
        {
            Puppets = puppets;
        }

        public PuppetData[] Puppets { get; }

		public override Unity_Object GetMainObject(IList<Unity_Object> objects) => objects.OfType<Unity_Object_GameloftRK>().FindItem(x => x.ObjectName == "Player");

        public override string[] LegacyDESNames => Puppets.Select(x => x.DisplayName).ToArray();
        public override string[] LegacyETANames => LegacyDESNames;

        public class PuppetData
        {
            public PuppetData(int index, int resourceFileID, int resourceID, Gameloft_Puppet puppetData, Gameloft_BaseManager.Unity_Gameloft_ObjGraphics puppet, string name = null)
            {
                Index = index;
                ResourceFileID = resourceFileID;
                ResourceID = resourceID;
                Puppet = puppet;
                GameloftPuppetData = puppetData;
                Name = name;
            }

            public int Index { get; }

            public int ResourceFileID { get; }
            public int ResourceID { get; }

            public Gameloft_BaseManager.Unity_Gameloft_ObjGraphics Puppet { get; }
            public Gameloft_Puppet GameloftPuppetData { get; }

            public string Name { get; }

            public string DisplayName => Name == null ? $"{Index}" : $"{Index}_{Name}";
        }
    }
}