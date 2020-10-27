using R1Engine.Serialize;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAIsometric : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAIsometric(Context context, GBAIsometric_ObjectType[] types, int waypointsStartIndex) : base(context)
        {
            Types = types;
            WaypointsStartIndex = waypointsStartIndex;
        }
     
        public GBAIsometric_ObjectType[] Types { get; }
        public int WaypointsStartIndex { get; }
    }
}