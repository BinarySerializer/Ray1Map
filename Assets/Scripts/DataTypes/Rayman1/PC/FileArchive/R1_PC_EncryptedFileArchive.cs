using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using R1Engine.Serialize;

namespace R1Engine
{
    /// <summary>
    /// Encrypted file archive data for PC
    /// </summary>
    public class R1_PC_EncryptedFileArchive : R1_PCBaseFile
    {
        /// <summary>
        /// The file entries
        /// </summary>
        public R1_PC_EncryptedFileArchiveEntry[] Entries { get; set; }

        public T ReadFile<T>(Context context, string fileName, Action<T> onPreSerialize = null)
            where T : R1Serializable, new() => ReadFile<T>(context, Entries.FindItemIndex(x => x.FileName == fileName), onPreSerialize);

        public T ReadFile<T>(Context context, int index, Action<T> onPreSerialize = null)
            where T : R1Serializable, new()
        {
            // Make sure the index is not out of bounds
            if (index < 0 || index >= Entries.Length)
                return null;

            var s = context.Deserializer;
            var entry = Entries[index];
            T output = null;

            // Deserialize the file
            s.DoAt(Offset + entry.FileOffset, () =>
            {
                s.DoXOR(entry.XORKey, () => output = s.SerializeObject<T>(default, onPreSerialize, name: entry.FileName ?? index.ToString()));
            });

            return output;
        }

        public byte[] ReadFileBytes(Context context, int index)
        {
            // Make sure the index is not out of bounds
            if (index < 0 || index >= Entries.Length)
                return null;

            var s = context.Deserializer;
            var entry = Entries[index];
            byte[] output = null;

            // Deserialize the file
            s.DoAt(Offset + entry.FileOffset, () =>
            {
                s.DoXOR(entry.XORKey, () => output = s.SerializeArray<byte>(default, entry.FileSize, name: entry.FileName ?? index.ToString()));
            });

            return output;
        }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Read the header
            base.SerializeImpl(s);

            // For Rayman 1 the header is hard-coded in the game executable
            if (s.GameSettings.EngineVersion == EngineVersion.R1_PC || s.GameSettings.EngineVersion == EngineVersion.R1_PocketPC)
            {
                if (s is BinarySerializer)
                    throw new Exception("Can't serialize Rayman 1 archive headers");

                var headerBytes = R1_PC_ArchiveHeaders.GetHeader(s.GameSettings, Path.GetFileName(Offset.file.filePath));
                var headerLength = headerBytes.Length / 12;

                using (var headerStream = new MemoryStream(headerBytes))
                {
                    var key = $"{Offset}_Header";
                    s.Context.AddStreamFile(key, headerStream);

                    Entries = s.DoAt(s.Context.GetFile(key).StartPointer, () => s.SerializeObjectArray<R1_PC_EncryptedFileArchiveEntry>(Entries, headerLength, name: nameof(Entries)));
                }
            }
            else
            {
                // Serialize the file table
                if (Entries == null)
                {
                    var tempList = new List<R1_PC_EncryptedFileArchiveEntry>();

                    while (!tempList.Any() || tempList.Last().FileName != "ENDFILE")
                        tempList.Add(s.SerializeObject<R1_PC_EncryptedFileArchiveEntry>(null, name: $"{nameof(Entries)} [{tempList.Count}]"));

                    Entries = tempList.Take(tempList.Count - 1).ToArray();
                }
                else
                {
                    s.SerializeObjectArray<R1_PC_EncryptedFileArchiveEntry>(Entries, Entries.Length, name: nameof(Entries));
                }
            }
        }
    }
}