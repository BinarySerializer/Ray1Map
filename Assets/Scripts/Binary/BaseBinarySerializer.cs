using System.IO;

namespace R1Engine
{
    /// <summary>
    /// A base binary serializer used for serializing/deserializing
    /// </summary>
    public class BaseBinarySerializer
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseStream">The base stream</param>
        /// <param name="filePath">The path of the file being serialized</param>
        /// <param name="gameSettings">The game settings</param>
        public BaseBinarySerializer(Stream baseStream, string filePath, GameSettings gameSettings)
        {
            BaseStream = baseStream;
            FilePath = filePath;
            GameSettings = gameSettings;
        }

        /// <summary>
        /// The base stream
        /// </summary>
        public Stream BaseStream { get; }

        /// <summary>
        /// The path of the file being serialized
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// The file name of the file being serialized, in lower-case
        /// </summary>
        public string FileName => Path.GetFileName(FilePath)?.ToLower();
        
        /// <summary>
        /// The file extensions of the file being serialized, in lower-case
        /// </summary>
        public string FileExtension => Path.GetExtension(FilePath)?.ToLower();

        /// <summary>
        /// The game settings
        /// </summary>
        public GameSettings GameSettings { get; }
    }
}