using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Linq;

namespace R1Engine
{
    public class Gameloft_RRR_Manager : Gameloft_BaseManager
    {
		public override string[] ResourceFiles => new string[] {
			"d1",
			"d2",
			"d3",
			"d4",
			"l0a01",
			"l0a02",
			"l0a03",
			"l0a04",
			"l0a05",
			"l0a06",
			"l0a07",
			"l0a08",
			"l0b",
			"s",
			"s1",
			"s2",
			"s3",
			"s4",
			"s5",
			"s6",
			"s7",
			"s8",
			"t0",
			"t1",
			"t2",
			"ts0",
			"ts1",
			"ts2",
		};

		public override string[] SingleResourceFiles => new string[] {
			"u",
			"sp",
			"spp",
			"lj21",
			"lj22",
			"lj23",
			"lj41",
			"lj42",
			"lj43",
			"lj44",
			"g",
			"cp",
		};

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 8).ToArray()),
		});

		public virtual int[] TileSetIndices => new int[] {
			0,
			0,
			0,
			1,
			1,
			1,
			2,
			2
		};

		public virtual int GetTileSetIndex(GameSettings settings) {
			return TileSetIndices[settings.Level];
		}

		public string GetLevelPath(GameSettings settings) => $"l0a0{settings.Level+1}";
		public string GetBackgroundTileSetPath(GameSettings settings) => $"ts{GetTileSetIndex(settings)}";
		public string GetForegroundTileSetPath(GameSettings settings) => $"t{GetTileSetIndex(settings)}";

		public override async UniTask LoadFilesAsync(Context context) {
			await context.AddLinearSerializedFileAsync(GetLevelPath(context.Settings));
			await context.AddLinearSerializedFileAsync(GetBackgroundTileSetPath(context.Settings));
			await context.AddLinearSerializedFileAsync(GetForegroundTileSetPath(context.Settings));
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {
			await UniTask.CompletedTask;
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.Settings), context);
			var lh0 = resf.SerializeResource<Gameloft_MapLayerHeader>(s, default, 0, onPreSerialize: o => o.Type = Gameloft_MapLayerHeader.LayerType.Graphics, name: "LayerHeader0");
			var lh1 = resf.SerializeResource<Gameloft_MapLayerHeader>(s, default, 1, onPreSerialize: o => o.Type = Gameloft_MapLayerHeader.LayerType.Graphics, name: "LayerHeader1");
			var lh2 = resf.SerializeResource<Gameloft_MapLayerHeader>(s, default, 2, onPreSerialize: o => o.Type = Gameloft_MapLayerHeader.LayerType.Collision, name: "LayerHeader2");
			var l0 = resf.SerializeResource<Gameloft_MapLayerData>(s, default, 3, onPreSerialize: o => o.Header = lh0, name: "Layer0");
			var l1 = resf.SerializeResource<Gameloft_MapLayerData>(s, default, 4, onPreSerialize: o => o.Header = lh1, name: "Layer1");
			var l2 = resf.SerializeResource<Gameloft_MapLayerData>(s, default, 5, onPreSerialize: o => o.Header = lh2, name: "Layer2");
			
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetForegroundTileSetPath(context.Settings), context);
			var ts_f = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Foreground");
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetBackgroundTileSetPath(context.Settings), context);
			var ts_b = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Background");
			var tileSet_f = GetPuppetImages(ts_f); // Tile textures. Tile i = tileset_f[i][0]
			var tileSet_b = GetPuppetImages(ts_b); 
			
			// TODO: create maps (dimensions are in lh0-2, data is in l0-2), level object, objManager etc

			throw new NotImplementedException();
		}
	}
}