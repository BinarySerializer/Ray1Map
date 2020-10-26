using R1Engine.Serialize;

namespace R1Engine
{
    public class Unity_ObjectManager_GBAIsometric : Unity_ObjectManager
    {
        public Unity_ObjectManager_GBAIsometric(Context context, GBAIsometric_ObjectType[] types) : base(context)
        {
            Types = types;
        }
     
        public GBAIsometric_ObjectType[] Types { get; }
    }
}