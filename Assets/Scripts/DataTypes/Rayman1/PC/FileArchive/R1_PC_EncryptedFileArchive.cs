using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// The raw decoded file data
        /// </summary>
        public byte[][] DecodedFiles { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Read the header
            base.SerializeImpl(s);

            // Serialize the file table
            if (Entries == null)
            {
                var tempList = new List<R1_PC_EncryptedFileArchiveEntry>();

                while (!tempList.Any() || tempList.Last().FileSize != 0)
                    tempList.Add(s.SerializeObject<R1_PC_EncryptedFileArchiveEntry>(null, name: $"{nameof(Entries)} [{tempList.Count}]"));

                Entries = tempList.ToArray();
            }
            else
            {
                s.SerializeObjectArray<R1_PC_EncryptedFileArchiveEntry>(Entries, Entries.Length, name: nameof(Entries));
            }

            if (DecodedFiles == null)
                DecodedFiles = new byte[Entries.Length - 1][];

            // Serialize the file contents
            for (var i = 0; i < Entries.Length - 1; i++)
            {
                // Get the entry
                var entry = Entries[i];
                
                // TODO: Handle checksum
                // Serialize the entry
                s.DoAt(Offset + entry.FileOffset, () =>
                {
                    s.DoXOR(entry.XORKey, () => DecodedFiles[i] = s.SerializeArray<byte>(DecodedFiles[i], entry.FileSize, name: $"{nameof(DecodedFiles)} [{i}]"));
                });
            }
        }
    }
}