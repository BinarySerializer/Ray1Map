using BinarySerializer;
using PsychoPortal;
using PsychoPortal.Unity;

namespace Ray1Map.Psychonauts
{
    public class Ray1MapLoader : Loader
    {
        public Ray1MapLoader(PsychonautsSettings settings, string directory, Context context, Unity_Level level) : base(settings, directory)
        {
            Context = context;
            Level = level;
        }

        public Context Context { get; }
        public Unity_Level Level { get; }
    }
}