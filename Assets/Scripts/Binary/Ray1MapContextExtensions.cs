using BinarySerializer;

namespace Ray1Map
{
    public static class Ray1MapContextExtensions
    {
        public static GameSettings GetR1Settings(this SerializerObject s) => s.Context.GetR1Settings();
        public static GameSettings GetR1Settings(this Context c) => (c as Ray1MapContext)?.GameSettings;
    }
}