using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using BinarySerializer;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman Classic (Mobile)
    /// </summary>
    public class R1_Mobile_Manager : R1_PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the base path for the game data
        /// </summary>
        /// <returns>The data path</returns>
        public override string GetDataPath() => "Media/PCMAP/";

        // We don't use this since it's a leftover file - the game uses the .csv files
        public override string GetLanguageFilePath() => "Media/RAY.LNG";

        public override string GetVignetteFilePath(GameSettings settings) => $"Media/VIGNET.DAT";

        public string GetLanguageFilePath(string langCode) => $"Media/LOCALIZATION_STR_{langCode.ToUpper()}.CSV";
        public string GetExtLanguageFilePath(string langCode) => $"MediaCosmos/Localization/ext_localization_{langCode.ToLower()}.csv";

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets the available game actions
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The game actions</returns>
        public override GameAction[] GetGameActions(GameSettings settings)
        {
            // Append action
            return base.GetGameActions(settings)
                .Append(new GameAction("Decrypt Files", true, false, (input, output) => DecryptFiles(input)))
				.Append(new GameAction("Encrypt Files", true, true, (input, output) => EncryptFiles(input, output)))
				.ToArray();
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
		/// Encrypts a file name
		/// </summary>
		/// <param name="fileName">The file name</param>
		/// <returns>The encrypted name</returns>
		public string EncryptFileName(string fileName) {
			string decrypted = "";

			const string key = "UBICOSMOS";

			int keyIndex = 0;

			foreach (int c in fileName) {
				int k = key[keyIndex];
				if (c >= 'a' && c <= 'z') {
					int nc = (c + k - 6) % 26;
					decrypted += (char)(int)(nc + (int)'a');
					keyIndex = (keyIndex + 1) % key.Length;
				} else if (c >= 'A' && c <= 'Z') {
					int nc = (c + k + 26) % 26;
					decrypted += (char)(int)(nc + (int)'A');
					keyIndex = (keyIndex + 1) % key.Length;
				} else {
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


            // Enumerate every encrypted file
            foreach (string filePath in Directory.EnumerateFiles(basePath, "*.compressed*", SearchOption.AllDirectories)) {
                // Get the file name
                string filename = Path.GetFileName(filePath);

                // Decrypt the file name without the .spd extension
                string decFileName = filename.Substring(0, filename.Length - 11);

                // Get the output file path
                string outputFilePath = Path.GetDirectoryName(filePath) + "/" + decFileName;

                using (Stream s = FileSystem.GetFileReadStream(filePath)) {
                    using (Reader reader = new Reader(s)) {
                        uint decompressedSize = reader.ReadUInt32();
                        uint compressionAlgorithmID = reader.ReadUInt32();
                        if (compressionAlgorithmID != 1) continue;

                        // Read the file bytes
                        byte[] bytes = reader.ReadBytes((int)s.Length - 8);

                        if (bytes.Length > 0) {
                            byte[] output = LZ4.LZ4Codec.Decode(bytes, 0, bytes.Length, (int)decompressedSize);
                            Util.ByteArrayToFile(outputFilePath, output);
                        } else {
                            Util.ByteArrayToFile(outputFilePath, new byte[0]);
                        }
                    }
                }

                // Delete the original file
                File.Delete(filePath);
            }
        }



		/// <summary>
		/// Decrypts all encrypted files
		/// </summary>
		/// <param name="referencePath">The directory to look for the encrypted files in,
        /// to use as reference for which files should be encrypted/compressed</param>
		/// <param name="basePath">The directory to look for the decrypted files in</param>
		public void EncryptFiles(string referencePath, string basePath) {
            basePath = basePath.Replace("\\", "/");
            if(!basePath.EndsWith("/")) basePath += "/";
			// Enumerate every file
			foreach (string filePath in Directory.EnumerateFiles(basePath, "*.*", SearchOption.AllDirectories)) {
				string relativePath = filePath.Substring(basePath.Length);
                var directory = Path.GetDirectoryName(filePath);
                var relativeDirectory = directory.Length > basePath.Length ? directory.Substring(basePath.Length) : "";
				string filename = Path.GetFileName(filePath);

				bool CheckReference(string filename) => File.Exists(Path.Combine(referencePath, relativeDirectory, filename));

                if(CheckReference(filename)) continue; // File is unencrypted

                byte[] data = File.ReadAllBytes(filePath);

				// Decrypt the file name without the .spd extension
				string encFilename = EncryptFileName(filename) + ".spd";
				string comprFilename = EncryptFileName($"{filename}.compressed") + ".spd";
                string outFilename = encFilename;

                if (CheckReference(comprFilename)) {
                    // Compress data first
                    outFilename = comprFilename;

                    byte[] output = data.Length > 0 ? LZ4.LZ4Codec.EncodeHC(data, 0, data.Length) : new byte[0];

                    using (MemoryStream ms = new MemoryStream()) {
                        using (Writer writer = new Writer(ms)) {
                            writer.Write((uint)data.Length); // Decompressed size
                            writer.Write((uint)1); // Compression algorithm id
                            writer.Write(output);
                        }
                        data = ms.ToArray();
                    }
                } else if (!CheckReference(encFilename)) {
                    throw new System.Exception($"Can't find match for file {relativePath}: {comprFilename} - {encFilename}\n{Path.Combine(referencePath, relativeDirectory, filename)}");
                }
                if (data.Length > 0) {
					using (Rijndael r = Rijndael.Create()) {
						r.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 7, 6, 5, 4, 3, 2, 1, 10 };
						r.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

						using (MemoryStream ms = new MemoryStream()) {
							using (CryptoStream cs = new CryptoStream(ms, r.CreateEncryptor(), CryptoStreamMode.Write))
								cs.Write(data, 0, data.Length);

							data = ms.ToArray();
						}
					}
				}

				// Get the output file path
				string outputFilePath = directory + "/" + outFilename;
				Util.ByteArrayToFile(outputFilePath, data);

				// Delete the original file
				File.Delete(filePath);
			}
		}


		/// <summary>
		/// Gets a binary file to add to the context
		/// </summary>
		/// <param name="context">The context</param>
		/// <param name="filePath">The file path</param>
		/// <param name="endianness">The endianness to use</param>
		/// <returns>The binary file</returns>
		protected override BinaryFile GetFile(Context context, string filePath, Endian endianness = Endian.Little) => new LinearFile(context, filePath, endianness);

        protected override async UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
        {
            var langs = new[]
            {
                new
                {
                    LangCode = "EN",
                    Language = "English",
                    Encoding = Settings.StringEncoding
                },
                new
                {
                    LangCode = "FR",
                    Language = "French",
                    Encoding = Settings.StringEncoding
                },
                new
                {
                    LangCode = "DE",
                    Language = "German",
                    Encoding = Settings.StringEncoding
                },
                new
                {
                    LangCode = "IT",
                    Language = "Italian",
                    Encoding = Encoding.UTF8
                },
                new
                {
                    LangCode = "ES",
                    Language = "Spanish",
                    Encoding = Encoding.UTF8
                },
                new
                {
                    LangCode = "KO",
                    Language = "Korean",
                    Encoding = Encoding.UTF8
                },
                new
                {
                    LangCode = "JA",
                    Language = "Japanese",
                    Encoding = Encoding.UTF8
                },
                new
                {
                    LangCode = "RU",
                    Language = "Russian",
                    Encoding = Encoding.UTF8
                },
                new
                {
                    LangCode = "ZH",
                    Language = "Chinese (Simplified)",
                    Encoding = Encoding.UTF8
                },
                new
                {
                    LangCode = "Zu",
                    Language = "Chinese (Traditional)",
                    Encoding = Encoding.UTF8
                },
            };

            // Create the dictionary
            var loc = new List<KeyValuePair<string, string[]>>();

            // Add each language
            foreach (var lang in langs)
            {
                var lngPath = GetLanguageFilePath(lang.LangCode);

                await AddFile(context, lngPath);

                var langFile = Ray1TextFileFactory.ReadText<TextLocFile>(lngPath, context, encoding: lang.Encoding);

                loc.Add(new KeyValuePair<string, string[]>(lang.Language, langFile.Strings));
            }

            return loc.ToArray();
        }

        #endregion
    }
}