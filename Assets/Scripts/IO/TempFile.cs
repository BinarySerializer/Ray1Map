using System;
using System.IO;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// A temporary file
    /// </summary>
    public sealed class TempFile : IDisposable
    {
        /// <summary>
        /// Creates a new temporary file
        /// </summary>
        public TempFile()
        {
            // Get the temp path and create the file
            TempPath = Path.GetTempFileName();

            // Get the file info
            var info = new FileInfo(TempPath);

            // Set the attribute to temporary
            info.Attributes |= FileAttributes.Temporary;

            Debug.Log($"A new temp file has been created under {TempPath}");
        }

        /// <summary>
        /// The path of the temporary file
        /// </summary>
        public string TempPath { get; }

        public Stream OpenRead() => File.OpenRead(TempPath);

        /// <summary>
        /// Removes the temporary file
        /// </summary>
        public void Dispose()
        {
            try
            {
                // Delete the temp file
                File.Delete(TempPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Couldn't delete temp file at {TempPath} with error {ex}");
            }
        }
    }
}