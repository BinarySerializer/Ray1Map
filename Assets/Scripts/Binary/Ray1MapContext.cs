using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;
using UnityEngine;
using ISystemLog = BinarySerializer.ISystemLog;

namespace Ray1Map
{
    public class Ray1MapContext : Context
    {
        public Ray1MapContext(string basePath, GameSettings settings) : base(
            basePath: basePath, // Pass in the base path
            settings: new R1SerializerSettings(), // Pass in the settings
            serializerLog: new R1SerializerLog(), // Use R1 serializer log for logging to a file
            fileManager: new R1FileManager(), // Use R1 file manager for use with FileSystem
            systemLog: new UnitySystemLog()) // Use Unity logger
        {
            // Add the game settings
            AddSettings(settings);

            settings.GetGameManager.AddContextSettings(this);
            settings.GetGameManager.AddContextPointers(this);
        }
        public Ray1MapContext(GameSettings settings) : this(settings.GameDirectory, settings) { }

        public GameSettings GameSettings => GetRequiredSettings<GameSettings>();

        public class R1SerializerSettings : ISerializerSettings
        {
            /// <summary>
            /// The default string encoding to use when none is specified
            /// </summary>
            public Encoding DefaultStringEncoding => Ray1Map.Settings.StringEncoding;

            /// <summary>
            /// Indicates if a backup file should be created when writing to a file
            /// </summary>
            public bool CreateBackupOnWrite => Ray1Map.Settings.BackupFiles;

            /// <summary>
            /// Indicates if pointers should be saved in the Memory Map for relocation
            /// </summary>
            public bool SavePointersForRelocation => false;

            /// <summary>
            /// Indicates if caching read objects should be ignored
            /// </summary>
            public bool IgnoreCacheOnRead => false;

            // TODO: Set to true when debugging? It's set to false now to match previous behavior, but ideally it should be
            //       true when debugging to avoid missing data. Season of Ice specifically gives a lot of warnings - fix?
            public bool LogAlignIfNotNull => false;
            //public bool LogAlignIfNotNull => FileSystem.mode != FileSystem.Mode.Web;

            /// <summary>
            /// The pointer size to use when logging a <see cref="Pointer"/>. Set to <see langword="null"/> to dynamically determine the appropriate size.
            /// </summary>
            public PointerSize? LoggingPointerSize => PointerSize.Pointer32;

			public Endian DefaultEndianness => Endian.Little;
		}

        public class R1FileManager : IFileManager
        {
			public PathSeparatorChar SeparatorCharacter => PathSeparatorChar.ForwardSlash;

			public bool DirectoryExists(string path) => FileSystem.DirectoryExists(path);

            public bool FileExists(string path) => FileSystem.FileExists(path);

            public Stream GetFileReadStream(string path) => FileSystem.GetFileReadStream(path);

            public Stream GetFileWriteStream(string path, bool recreateOnWrite = true) => FileSystem.GetFileWriteStream(path, recreateOnWrite);

            public async Task FillCacheForReadAsync(long length, Reader reader)
            {
                if (reader.BaseStream.InnerStream is PartialHttpStream httpStream)
                    await httpStream.FillCacheForRead(length);
            }
        }
        
        public class UnitySystemLog : ISystemLog
        {
            public void Log(BinarySerializer.LogLevel logLevel, object log, params object[] args) {
                switch (logLevel) {
                    case BinarySerializer.LogLevel.Error:
                        Debug.LogError(String.Format(log?.ToString() ?? String.Empty, args));
                        break;
                    case BinarySerializer.LogLevel.Warning:
                        Debug.LogWarning(String.Format(log?.ToString() ?? String.Empty, args));
                        break;
                    case BinarySerializer.LogLevel.Info:
                        Debug.Log(String.Format(log?.ToString() ?? String.Empty, args));
                        break;
                }
            }
        }

        public class R1SerializerLog : ISerializerLog
        {
            public bool IsEnabled => Ray1Map.Settings.Log;

            private StreamWriter _logWriter;

            protected StreamWriter LogWriter => _logWriter ??= GetFile();

            public string OverrideLogPath { get; set; }
            public string LogFile => OverrideLogPath ?? Ray1Map.Settings.LogFile;
            public int BufferSize => 0x8000000; // 1 GB

            public StreamWriter GetFile()
            {
                return new StreamWriter(File.Open(LogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8, BufferSize);
            }

            public void Log(object obj)
            {
                if (IsEnabled)
                    LogWriter.WriteLine(obj != null ? obj.ToString() : "");
            }

            public void Dispose()
            {
                _logWriter?.Dispose();
                _logWriter = null;
            }
        }
    }
}