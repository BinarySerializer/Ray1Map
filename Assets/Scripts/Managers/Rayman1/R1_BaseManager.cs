using BinarySerializer;

namespace R1Engine
{
    public abstract class R1_BaseManager : BaseGameManager
    {
        public override void AddContextSettings(Context context)
        {
            // Add Ray1 settings
            context.AddSettings(context.GetR1Settings().GetRay1Settings());
        }
    }
}