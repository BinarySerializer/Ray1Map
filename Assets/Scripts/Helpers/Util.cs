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

		// For debugging
		public static void ExportPointerArray(SerializerObject s, string path, IEnumerable<Pointer> pointers)
        {
            var p1 = pointers.Where(x => x != null).Distinct().OrderBy(x => x.AbsoluteOffset).ToArray();
            var output = new List<string>();

            for (int i = 0; i < p1.Length - 1; i++)
            {
                var length = p1[i + 1] - p1[i];

                s.DoAt(p1[i], () => output.Add(String.Join(" ", s.SerializeArray<byte>(null, length).Select(x => x.ToString("X2")))));
            }

            File.WriteAllLines(path, output);
        }

		/// <summary>
		/// Convert a byte array to a hex string
		/// </summary>
		/// <param name="Bytes">The byte array to convert</param>
		/// <param name="Align">Should the byte array be split in different lines, this defines the length of one line</param>
		/// <param name="NewLinePrefix">The prefix to add to each new line</param>
		/// <returns></returns>
		public static string ByteArrayToHexString(byte[] Bytes, int? Align = null, string NewLinePrefix = null, int? MaxLines = null) {
			StringBuilder Result = new StringBuilder(Bytes.Length * 2);
			string HexAlphabet = "0123456789ABCDEF";
			int curLine = 0;
			for(int i = 0; i < Bytes.Length; i++) {
				if (i > 0 && Align.HasValue && i % Align == 0) {
					curLine++;
					if (MaxLines.HasValue && curLine >= MaxLines.Value) {
						Result.Append("...");
						return Result.ToString();
					}
					Result.Append("\n" + NewLinePrefix ?? "");
				}
				byte B = Bytes[i];
				Result.Append(HexAlphabet[(int)(B >> 4)]);
				Result.Append(HexAlphabet[(int)(B & 0xF)]);
				if(i < Bytes.Length-1) Result.Append(' ');
			}

			return Result.ToString();
		}
    }
}
