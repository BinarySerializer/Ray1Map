using Cysharp.Threading.Tasks;

using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace R1Engine
{
    public class Gameloft_RRR_128x128_cz_Manager : Gameloft_RRR_Manager
    {

		public override string[] ResourceFiles => new string[] {
			"d1",
			"l0a01",
			"l0a03",
			"l0a04",
			"l0a05",
			"l0a06",
			"l0a07",
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
			"s30",
			"t0",
			"t1",
			"t2",
			"ts0",
			"ts1",
			"ts2",
		};

		public override string[] SingleResourceFiles => new string[] {
			"u",
			"sc",
			"st",
			"lj21",
			"lj22",
			"lj23",
			"lj41",
			"lj42",
			"lj43",
			"lj44",
			"g",
		};

		public override string FixFilePath => "d1";
		public override int LevelListResourceIndex => 1;
		public override int ObjectModelListResourceIndex => 2;
		public override int ResourceListResourceIndex => 3;
		public override int LocalizationResourceIndex => 4;


		public override Unity_Map[] LoadMaps(Context context, Gameloft_RRR_LevelList levelList) {
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.GetR1Settings()), context);
			var lh1 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 0, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Graphics, name: "LayerHeader1");
			var lh2 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 1, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Collision, name: "LayerHeader2");
			var l1 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 2, onPreSerialize: o => o.Header = lh1, name: "Layer1");
			var l2 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 3, onPreSerialize: o => o.Header = lh2, name: "Layer2");
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetForegroundTileSetPath(context.GetR1Settings(), levelList), context);
			var ts_f = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Foreground");
			var tileSet_f = GetPuppetImages(ts_f, flipY: false, allowTransparent: false);


			int cellSize = tileSet_f[0][0].width;

			// Pad foreground tileset with transparent tiles
			var tileset_f_padding = Enumerable.Repeat(TextureHelpers.CreateTexture2D(cellSize, cellSize, clear: true, applyClear: true).CreateTile(), 128 - tileSet_f.Length);


			// Load maps
			var maps = new Unity_Map[]
			{
				// Foreground
				new Unity_Map
				{
					Width = lh1.Width,
					Height = lh1.Height,
					TileSet = new Unity_TileSet[]
					{
						new Unity_TileSet(tileSet_f.Select(x => x[0].CreateTile()).Concat(tileset_f_padding).ToArray())
					},
					MapTiles = l1.TileMap.Select(x => new Unity_Tile(x)).ToArray(),
					Type = Unity_Map.MapType.Graphics,
				},
				// Collision
				new Unity_Map
				{
					Width = lh2.Width,
					Height = lh2.Height,
					TileSet = new Unity_TileSet[]
					{
						new Unity_TileSet(cellSize),
					},
					MapTiles = l2.TileMap.Select(x => new Unity_Tile(x)).ToArray(),
					Type = Unity_Map.MapType.Collision,
				}
			};

			return maps;

		}

		public override int[] HardcodedPuppetImageBufferIndices => new int[] { 0, 1, 2, 1, 3, 4, 5, 0, 6, 7, 2, 3, 8, 4, 9, 9, 10, 11, 5, 2, 5, 11, 12, 13, 14 };

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, new int[] { 0, 2, 3, 4, 5, 6 }),
		});
	}
}