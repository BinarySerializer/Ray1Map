using BinarySerializer;
using PsychoPortal;
using PsychoPortal.Unity;
using UnityEngine;

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
            base.Dispose();
            Logger?.Dispose();

            Debug.Log("Disposed loader");
        }
    }
}