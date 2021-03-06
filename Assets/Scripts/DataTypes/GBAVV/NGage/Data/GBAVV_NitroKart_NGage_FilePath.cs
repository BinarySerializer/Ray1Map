using System;
using System.IO;
using R1Engine.Serialize;

namespace R1Engine
{
    public class GBAVV_NitroKart_NGage_FilePath : R1Serializable
    {
        public GBAVV_NitroKart_NGage_FilePath() { }
        public GBAVV_NitroKart_NGage_FilePath(Context context, string filePath)
        {
            Context = context;
            FilePath = filePath;
        }

        // Set before serializing
        public string BasePath { get; set; }
        public long? StringLength { get; set; }

        public string FilePath { get; set; }

        // Helpers
        public string GetFullPath => BasePath != null ? Path.Combine(BasePath, FilePath) : FilePath;
        public T DoAtFile<T>(Func<T> func) => DoAtFile(null, func);
        public T DoAtFile<T>(string fileExtension, Func<T> func)
        {
            var manager = (GBAVV_NitroKart_NGage_Manager)Context.Settings.GetGameManager;

            var path = GetFullPath;

            if (fileExtension != null)
                path += fileExtension;

            return manager.DoAtBlock(Context, path, func);
        }

        public override void SerializeImpl(SerializerObject s)
        {
            FilePath = s.SerializeString(FilePath, StringLength, name: nameof(FilePath));
        }
    }
}