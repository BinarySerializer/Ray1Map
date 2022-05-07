using BinarySerializer;
using PsychoPortal;
using PsychoPortal.Unity;

namespace Ray1Map.Psychonauts
{
    public class Ray1MapLoader : Loader
    {
        public Ray1MapLoader(PsychonautsSettings settings, string directory) 
            : this(settings, directory, null, null, null)
        { }
        public Ray1MapLoader(PsychonautsSettings settings, string directory, IBinarySerializerLogger logger) 
            : this(settings, directory, null, null, logger)
        { }
        public Ray1MapLoader(PsychonautsSettings settings, string directory, Context context, Unity_Level level, IBinarySerializerLogger logger) 
            : base(new Ray1MapFileSystem(), settings, directory)
        {
            Context = context;
            Level = level;
            Logger = logger;
        }

        public Context Context { get; }
        public Unity_Level Level { get; }
        public IBinarySerializerLogger Logger { get; }

        // TODO: Make async versions of the load methods which prepares the files for web. Packages will need to be treated as big files.

        public override void Dispose()
        {
            // TODO: Find a better solution to this which supports lazy post-load animation loading
            //base.Dispose();
            //Logger?.Dispose();
        }
    }
}