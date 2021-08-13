using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;
using Cysharp.Threading.Tasks;

namespace R1Engine.Jade {
	public class WOR_WorldList : Jade_File {
		public Jade_GenericReference[] Worlds { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Worlds = s.SerializeObjectArray<Jade_GenericReference>(Worlds, FileSize / 8, name: nameof(Worlds));
		}

		public async UniTask ResolveReferences(SerializerObject s) {
			int worldIndex = 0;
			foreach (var world in Worlds) {
				if (!world.IsNull && Loader.IsBinaryData && Loader.Bin.CurrentPosition.AbsoluteOffset >= Loader.Bin.TotalSize) {
					s.LogWarning($"World {worldIndex + 1}/{Worlds.Length} ({world.Key}) could not be serialized");
					break;
				}
				Controller.DetailedState = $"Loading world {worldIndex + 1}/{Worlds.Length}";
				bool hasLoadedWorld = Loader.LoadedWorlds.Any(w => w.Key == world.Key);
				world.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontUseCachedFile);
				await Loader.LoadBinOrNot(s);

				if (world?.Value != null && world.Value is WOR_World w) {
					Controller.DetailedState = $"Loading world {worldIndex+1}/{Worlds.Length}: {w.Name}";
					await w.JustAfterLoad(s, hasLoadedWorld);

					if (Loader.ShouldExportVars) {
						foreach (var gao in w.SerializedGameObjects) {
							var ai = gao?.Extended?.AI?.Value;
							if (ai != null) {
								var name = gao.Name.Replace(".gao", "");
								ai?.Vars?.Value?.ExportVarsOverview(w.Name, $"{name}_instance");
								ai?.Vars?.Value?.ExportIDAStruct(w.Name, $"{name}_instance");
								ai?.Model?.Value?.Vars?.ExportVarsOverview(w.Name, $"{name}_model");
								ai?.Model?.Value?.Vars?.ExportIDAStruct(w.Name, $"{name}_model");
							}
						}
					}
				}
				worldIndex++;

			}
		}

		public async UniTask ResolveReferences_Montreal(SerializerObject s, bool isEditor) {
			Loader.Cache.Clear();
			int worldIndex = 0;
			foreach (var world in Worlds) {
				if (world.IsNull) continue;

				Controller.DetailedState = $"Loading world {worldIndex + 1}/{Worlds.Length}";
				bool hasLoadedWorld = Loader.LoadedWorlds.Any(w => w.Key == world.Key);
				bool isWOW = world.Type == Jade_FileType.FileType.WOR_World;
				if (!isWOW) {
					if (BinFileHeader == null) {
						// Unbinarized
						world.Resolve(queue: LOA_Loader.QueueType.Current);
						await Loader.LoadLoop(s);
						if (world.Value != null) {
							var newList = (WOR_WorldList)world.Value;
							await newList.ResolveReferences_Montreal(s, isEditor);
						}
					} else {
						throw new NotImplementedException($"WOL: A non-WOW file was referenced: {world}");
					}
				}
				if (!hasLoadedWorld) {
					await Jade_Montreal_BaseManager.LoadWorld_Montreal(s, world, worldIndex, Worlds.Length, isEditor);
				}
				worldIndex++;

			}
		}
		public override string Export_Extension => "wol";
	}
}
