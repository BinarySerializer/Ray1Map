using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace R1Engine {
    public static class Util {
        public static bool ByteArrayToFile(string fileName, byte[] byteArray) {
			if (byteArray == null) return false;
            if (FileSystem.mode == FileSystem.Mode.Web) return false;
            try {
				Directory.CreateDirectory(new System.IO.FileInfo(fileName).Directory.FullName);
                using (var fs = new FileStream(fileName, System.IO.FileMode.Create, FileAccess.Write)) {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

		private static readonly string[] SizeSuffixes =
				  { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		public static string SizeSuffix(Int64 value, int decimalPlaces = 1) {
			if (value < 0) { return "-" + SizeSuffix(-value); }

			int i = 0;
			decimal dValue = value;
			while (Math.Round(dValue, decimalPlaces) >= 1000) {
				dValue /= 1024;
				i++;
			}

			return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
		}

		public static string NormalizePath(string path, bool isFolder) {
			string newPath = path.Replace("\\", "/");
			if (isFolder && !newPath.EndsWith("/")) newPath += "/";
			return newPath;
		}

		/// <summary>
		/// Convert a byte array to a hex string
		/// </summary>
		/// <param name="Bytes">The byte array to convert</param>
		/// <param name="Align">Should the byte array be split in different lines, this defines the length of one line</param>
		/// <param name="NewLinePrefix">The prefix to add to each new line</param>
		/// <returns></returns>
		public static string ByteArrayToHexString(byte[] Bytes, int? Align = null, string NewLinePrefix = null) {
			StringBuilder Result = new StringBuilder(Bytes.Length * 2);
			string HexAlphabet = "0123456789ABCDEF";

			for(int i = 0; i < Bytes.Length; i++) {
				if (i > 0 && Align.HasValue && i % Align == 0) {
					Result.Append("\n" + NewLinePrefix ?? "");
				}
				byte B = Bytes[i];
				Result.Append(HexAlphabet[(int)(B >> 4)]);
				Result.Append(HexAlphabet[(int)(B & 0xF)]);
				if(i < Bytes.Length-1) Result.Append(' ');
			}

			return Result.ToString();
		}

		public static string DecryptSparkName(string str) {
			string decrypted = "";
			string key = "UBICOSMOS";
			string keyL = "ubicosmos";
			int keyIndex = 0;
			for (int i = 0; i < str.Length; i++) {
				int c = str[i];
				int k = key[keyIndex];
				if (c >= 'a' && c <= 'z') {
					int nc = (c - k - 6) % 26;
					decrypted += (char)(int)(nc + (int)'a');
					keyIndex = (keyIndex + 1) % key.Length;
				} else if (c >= 'A' && c <= 'Z') {
					int nc = (c - k + 26) % 26;
					decrypted += (char)(int)(nc + (int)'A');
					keyIndex = (keyIndex + 1) % key.Length;
				} else {
					decrypted += (char)c;
					//keyIndex = 0;
				}
			}
			return decrypted;
		}

		public static void DecryptRaymanClassic(string basePath) {
			IEnumerable<string> files = Directory.EnumerateFiles(basePath, "*.spd*", SearchOption.AllDirectories).Select(f => f.Substring(basePath.Length));
			foreach (string f in files) {
				string dirname = Path.GetDirectoryName(f);
				string filename = Path.GetFileName(f);
				string filenameNoExt = filename.Substring(0, filename.Length - 4);
				string decFileName = Util.DecryptSparkName(filenameNoExt);
				MonoBehaviour.print(filenameNoExt + " - " + decFileName);
				using (Stream s = FileSystem.GetFileReadStream(basePath + f)) {
					using (Reader reader = new Reader(s)) {
						byte[] bytes = reader.ReadBytes((int)s.Length);
						if (bytes.Length > 0) {
							using (Rijndael r = Rijndael.Create()) {
								r.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 7, 6, 5, 4, 3, 2, 1, 10 };
								r.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
								using (MemoryStream ms = new MemoryStream()) {
									using (CryptoStream cs = new CryptoStream(ms, r.CreateDecryptor(), CryptoStreamMode.Write)) {
										cs.Write(bytes, 0, bytes.Length);
									}
									byte[] dec = ms.ToArray();
									Util.ByteArrayToFile(basePath + "Decrypted/" + dirname + "/" + decFileName, dec);
								}
							}
						} else {
							Util.ByteArrayToFile(basePath + "Decrypted/" + dirname + "/" + decFileName, new byte[0]);
						}
					}
				}
			}
		}
	}
}
