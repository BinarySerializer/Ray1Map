using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions_ExportGearBF : Jade_GameActions {
		public Jade_GameActions_ExportGearBF(Jade_BaseManager manager) : base(manager) { }

		public async UniTask ExportGearBF(GameSettings settings, string bfPath, string outputDir, string extension) {
			if (bfPath == null) return;
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				GEAR_BigFile bf = await LoadGearBF(context, bfPath);

				var s = context.Deserializer;
				foreach (var crc in bf.FileCRC32) {
					await bf.SerializeFile(s, crc, (filesize) => {
						var bytes = s.SerializeArray<byte>(default, filesize, name: "bytes");
						Util.ByteArrayToFile(Path.Combine(outputDir, $"{crc:X8}.{extension}"), bytes);
					});
				}
			}
			// TODO: "texture_map.txt" is the key<->filename map for the textures.bf file. The file ID is CRC32 of filename.
			// TODO: Figure out the correct converted filename (the .bra is replaced, but with what?)
		}
		public async UniTask ExportTexturesGearBF(GameSettings settings, string outputDir) {
			string bfPath = TexturesGearBFPath;
			if (bfPath == null) return;
			using (var context = new Ray1MapContext(settings)) {
				await LoadFilesAsync(context);
				GEAR_BigFile bf = await LoadGearBF(context, bfPath);
				var s = context.Deserializer;
				HashSet<int> exportedCRC = new HashSet<int>();

				int getCrc(string str) {
					var crcObj = new Ionic.Crc.CRC32();
					var bytes = Encoding.GetBytes(str);
					int crc32 = 0;
					using (var ms = new MemoryStream(bytes)) {
						crc32 = crcObj.GetCrc32(ms);
					}
					return crc32;
				}
				async UniTask<byte[]> readBytes(int crc) {
					byte[] bytes = null;
					await bf.SerializeFile(s, crc, (filesize) => {
						bytes = s.SerializeArray<byte>(default, filesize, name: "bytes");
					});
					return bytes;
				}
				async UniTask<byte[]> readAndExport(string filename) {
					var crc = getCrc(filename);
					byte[] bytes = await readBytes(getCrc(filename));
					if (bytes != null) {
						if (!exportedCRC.Contains(crc)) exportedCRC.Add(crc);
						Util.ByteArrayToFile(Path.Combine(outputDir, filename), bytes);
					}
					return bytes;
				}
				byte[] textureMapBytes = await readAndExport("texture_map.txt");
				if (textureMapBytes != null) {
					using (var ms = new MemoryStream(textureMapBytes)) {
						using (var reader = new StreamReader(ms, Encoding)) {
							string line;
							while ((line = reader.ReadLine()) != null) {
								var split = line.Split(';');
								if (split.Length == 2) {
									var filename = split[0];
									var key = split[1];

									if (filename.Length > 4) {
										var basename = filename.Substring(0, filename.Length - 4);
										await readAndExport($"{basename}_ps3.dds");
										await readAndExport($"{basename}_nm_ps3.dds");
									}
								}
							}
						}
					}
				}
				foreach (var crc in bf.FileCRC32) {
					if (exportedCRC.Contains(crc)) continue;
					byte[] bytes = await readBytes(crc);
					if (bytes != null) {
						Util.ByteArrayToFile(Path.Combine(outputDir, "unnamed", $"{crc:X8}.dds"), bytes);
					}
				}
			}
		}

	}
}
