using R1Engine.Serialize;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Classic (Mobile)
    /// </summary>
    public class Mobile_R1_Manager : PC_R1_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public override string GetDataPath() => "Media/PCMAP/";

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            // Get the base options
            var baseOptions = base.GetGameActions(settings);

            // Append and return
            return baseOptions.Append<GameAction>(new GameAction("Decrypt Files", true, false)).ToArray<GameAction>();
        }

        /// <summary>
        /// Runs the specified game action
        /// </summary>
        /// <param name="actionIndex">The action index</param>
        /// <param name="inputDir">The input directory</param>
        /// <param name="outputDir">The output directory</param>
        /// <param name="settings">The game settings</param>
        public override void RunAction(int actionIndex, string inputDir, string outputDir, GameSettings settings)
        {
            // Get original count
            var origCount = base.GetGameActions(settings).Length;

            // Use original export
            if (actionIndex < origCount)
            {
                base.RunAction(actionIndex, inputDir, outputDir, settings);
                return;
            }

            // Use new export
            switch (actionIndex - origCount)
            {
                case 0:
                    DecryptFiles(inputDir);
                    break;
            }
        }

        /// <summary>
        /// Decrypts an encrypted file name
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The decrypted name</returns>
        public string DecryptFileName(string fileName)
        {
            string decrypted = "";

            const string key = "UBICOSMOS";

            int keyIndex = 0;

            foreach (int c in fileName)
            {
                int k = key[keyIndex];
                if (c >= 'a' && c <= 'z')
                {
                    int nc = (c - k - 6) % 26;
                    decrypted += (char)(int)(nc + (int)'a');
                    keyIndex = (keyIndex + 1) % key.Length;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    int nc = (c - k + 26) % 26;
                    decrypted += (char)(int)(nc + (int)'A');
                    keyIndex = (keyIndex + 1) % key.Length;
                }
                else
                {
                    decrypted += (char)c;
                }
            }

            return decrypted;
        }

        /// <summary>
        /// Decrypts all encrypted files
        /// </summary>
        /// <param name="basePath">The directory to look for the encrypted files in</param>
        public void DecryptFiles(string basePath)
        {
            // Enumerate every encrypted file
            foreach (string filePath in Directory.EnumerateFiles(basePath, "*.spd*", SearchOption.AllDirectories))
            {
                // Get the file name
                string filename = Path.GetFileName(filePath);

                // Decrypt the file name without the .spd extension
                string decFileName = DecryptFileName(filename.Substring(0, filename.Length - 4));

                // Get the output file path
                string outputFilePath = Path.GetDirectoryName(filePath) + "/" + decFileName;

                using (Stream s = FileSystem.GetFileReadStream(filePath))
                {
                    using (Reader reader = new Reader(s))
                    {
                        // Read the file bytes
                        byte[] bytes = reader.ReadBytes((int)s.Length);
                        
                        if (bytes.Length > 0)
                        {
                            using (Rijndael r = Rijndael.Create())
                            {
                                r.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 7, 6, 5, 4, 3, 2, 1, 10 };
                                r.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    using (CryptoStream cs = new CryptoStream(ms, r.CreateDecryptor(), CryptoStreamMode.Write))
                                        cs.Write(bytes, 0, bytes.Length);

                                    byte[] dec = ms.ToArray();

                                    Util.ByteArrayToFile(outputFilePath, dec);
                                }
                            }
                        }
                        else
                        {
                            Util.ByteArrayToFile(outputFilePath, new byte[0]);
                        }
                    }
                }

                // Delete the original file
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Gets a binary file to add to the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filePath">The file path</param>
        /// <returns>The binary file</returns>
        protected override BinaryFile GetFile(Context context, string filePath) => new LinearSerializedFile(context)
        {
            filePath = filePath
        };

        #endregion
    }
}