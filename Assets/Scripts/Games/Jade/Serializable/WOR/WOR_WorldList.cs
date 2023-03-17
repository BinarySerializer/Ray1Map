using System;
using System.Linq;
using BinarySerializer;
using Cysharp.Threading.Tasks;

namespace Ray1Map.Jade {
	public class WOR_WorldList : Jade_File {
		public Jade_GenericReference[] Worlds { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			Worlds = s.SerializeObjectArray<Jade_GenericReference>(Worlds, FileSize / 8, name: nameof(Worlds));
		}

		public async UniTask ResolveReferences(SerializerObject s, bool isPrefabs = false) {
			int worldIndex = 0;
			foreach (var world in Worlds) {
				if (!world.IsNull && Loader.IsBinaryData && Loader.Bin.CurrentPosition.AbsoluteOffset >= Loader.Bin.TotalSize) {
					s.SystemLogger?.LogWarning($"World {worldIndex + 1}/{Worlds.Length} ({world.Key}) could not be serialized");
					break;
				}
				Controller.DetailedState = $"Loading world {worldIndex + 1}/{Worlds.Length}";
				bool hasLoadedWorld = Loader.LoadedWorlds.Any(w => w.Key == world.Key);
				world.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.DontUseCachedFile);
				await Loader.LoadBinOrNot(s);

				if (world?.Value != null && world.Value is WOR_World w) {
					Controller.DetailedState = $"Loading world {worldIndex+1}/{Worlds.Length}: {w.Name}";

					/*if (isPrefabs && worldIndex == Worlds.Length - 1) {
						Jade_Reference<AI_Instance> univers = new Jade_Reference<AI_Instance>(Context, Loader.BigFiles[0].UniverseKey);
						univers.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.DontUseCachedFile);
						await Loader.LoadBinOrNot(s);
					}*/

					await w.JustAfterLoad(s, hasLoadedWorld, doPrefabsCheck: isPrefabs && worldIndex == Worlds.Length - 1);

					foreach (var gao in w.SerializedGameObjects) {
						var ai = gao?.Extended?.AI?.Value;
						if (ai != null) {
							ai.CheckVariables();
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
