using System.Collections.Generic;

namespace R1Engine
{
    public class Unity_Sector
    {
        public Unity_Sector(List<int> objects = null)
        {
            Objects = objects ?? new List<int>();
        }

        public List<int> Objects { get; }
    }
}