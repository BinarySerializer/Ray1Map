using System;
using System.Collections.Generic;
using R1Engine.Serialize;

namespace R1Engine
{
    public class Unity_ObjectManager
    {
        public Unity_ObjectManager(Context context)
        {
            Context = context;
        }

        public Context Context { get; }

        public virtual string[] GetAvailableObjects => new string[0];
        public virtual Unity_Object CreateObject(int index) => null;

        public virtual void InitLinkGroups(IList<Unity_Object> objects) { }
        public virtual void SaveLinkGroups(IList<Unity_Object> objects) { }

        public virtual void InitEvents(Unity_Level level) { }
        public virtual Unity_Object GetMainObject(IList<Unity_Object> objects) => null;

        [Obsolete]
        public virtual string[] LegacyDESNames => new string[0];
        [Obsolete]
        public virtual string[] LegacyETANames => new string[0];
    }
}