using System.IO;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;
using UnityEngine;
using ILogger = BinarySerializer.ILogger;

namespace R1Engine
{
    public class R1Context : Context
    {
        public R1Context(string basePath, GameSettings settings) : base(
            basePath: basePath, // Pass in the base path
            defaultEncoding: R1Engine.Settings.StringEncoding, // Use default string encoding
            serializerLog: new R1SerializerLog(), // Use R1 serializer log for logging to a file
            fileManager: new R1FileManager(), // Use R1 file manager for use with FileSystem
            logger: new UnityLogger()) // Use Unity logger
        {
            Settings = settings;
        }
        public R1Context(GameSettings settings) : this(settings.GameDirectory, settings) { }

        public GameSettings Settings { get; }
        public override bool CreateBackupOnWrite => R1Engine.Settings.BackupFiles;

        public class R1FileManager : IFileManager
        {
            public bool DirectoryExists(string path) => FileSystem.DirectoryExists(path);

            public bool FileExists(string path) => FileSystem.FileExists(path);

            public Stream GetFileReadStream(string path) => FileSystem.GetFileReadStream(path);

            public Stream GetFileWriteStream(string path, bool recreateOnWrite = true) => FileSystem.GetFileWriteStream(path, recreateOnWrite);

            public string NormalizePath(string path, bool isDirectory) => Util.NormalizePath(path, isDirectory);

            public async Task FillCacheForReadAsync(int length, Reader reader)
            {
                if (reader.BaseStream is PartialHttpStream httpStream)
                    await httpStream.FillCacheForRead(length);
            }
        }
        
        public class UnityLogger : ILogger
        {
            public void Log(object log)
            {
                Debug.Log(log);
            }

            public void LogWarning(object log)
            {
                Debug.LogWarning(log);
            }

            public void LogError(object log)
            {
                Debug.LogError(log);
            }
        }

        public class R1SerializerLog : ISerializerLog
        {
            public R1SerializerLog()
            {
                CurrentLog = new StringBuilder();
            }

            public bool IsEnabled => R1Engine.Settings.Log;
            protected StringBuilder CurrentLog { get; }

            public string OverrideLogPath { get; set; }
            public string LogFile => OverrideLogPath ?? R1Engine.Settings.LogFile;

            public void Log(object obj)
            {
                if (IsEnabled)
                    CurrentLog.AppendLine(obj != null ? obj.ToString() : "");
            }

            public void WriteLog()
            {
                if (IsEnabled && LogFile.Trim() != "")
                {
                    using (StreamWriter writer = new StreamWriter(LogFile))
                    {
                        writer.WriteLine(CurrentLog.ToString());
                    }
                }
            }
        }
    }
}