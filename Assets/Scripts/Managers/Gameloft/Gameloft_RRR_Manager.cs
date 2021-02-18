using Cysharp.Threading.Tasks;
using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
			"s0",
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

		public string ObjectsFilePath => "l0b";
		public string FixFilePath => "d2";

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 8).ToArray()),
		});



		public Gameloft_RRR_LevelList LoadLevelList(Context context) {
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(FixFilePath, context);
			return resf.SerializeResource<Gameloft_RRR_LevelList>(s, default, 0, name: "LevelList");
		}

		public virtual int GetWorldIndex(GameSettings settings, Gameloft_RRR_LevelList levelList) {
			return levelList.Levels[settings.Level].World;
		}

		public string GetLevelPath(GameSettings settings) => $"l0a0{settings.Level+1}";
		public string GetBackgroundTileSetPath(GameSettings settings, Gameloft_RRR_LevelList levelList) => $"ts{GetWorldIndex(settings, levelList)}";
		public string GetForegroundTileSetPath(GameSettings settings, Gameloft_RRR_LevelList levelList) => $"t{GetWorldIndex(settings, levelList)}";
		public string GetPuppetPath(int i) => i >= 0 ?  $"s{i}" : "s";

		public override async UniTask LoadFilesAsync(Context context) {
			await context.AddLinearSerializedFileAsync(FixFilePath);
			var levelList = LoadLevelList(context);
			await context.AddLinearSerializedFileAsync(GetBackgroundTileSetPath(context.Settings, levelList));
			await context.AddLinearSerializedFileAsync(GetForegroundTileSetPath(context.Settings, levelList));
			await context.AddLinearSerializedFileAsync(GetLevelPath(context.Settings));
			await context.AddLinearSerializedFileAsync(ObjectsFilePath);
			for (int i = -1; i < 9; i++) {
				await context.AddLinearSerializedFileAsync(GetPuppetPath(i));
			}
		}

		public Dictionary<string, string[]> LoadLocalization(Context context) {
			var langages = new string[]
			{
				"English"
			};
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(FixFilePath, context);
			var loc = resf.SerializeResource<Gameloft_RRR_LocalizationTable>(s, default, 3, name: "Localization");

			return loc.LanguageTables.Select((x, i) => new {
				Lang = langages[i],
				Strings = x.Strings
			}).ToDictionary(x => x.Lang, x => x.Strings);
		}

		public Unity_ObjectManager_GameloftRRR.PuppetData[] LoadPuppets(Context context) {
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(FixFilePath, context);
			var modl = resf.SerializeResource<Gameloft_RRR_ObjectModelList>(s, default, 1, name: "ObjectModelList");
			var resl = resf.SerializeResource<Gameloft_RRR_PuppetResourceList>(s, default, 2, name: "ResourceList");

			Gameloft_Puppet[] puppets = new Gameloft_Puppet[resl.ResourceList.Length];
			for (int i = 0; i < puppets.Length; i++) {
				var rref = resl.ResourceList[i];
				resf = FileFactory.Read<Gameloft_ResourceFile>(GetPuppetPath(rref.FileID), context);
				puppets[i] = resf.SerializeResource<Gameloft_Puppet>(s, default, rref.ResourceID, name: $"Puppets[{i}]");
			}
			var models = new Unity_ObjectManager_GameloftRRR.PuppetData[modl.Models.Length];
			for (int i = 0; i < models.Length; i++) {
				var mod = modl.Models[i];
				if (mod.ResourceReferenceID != -1) {
					models[i] = new Unity_ObjectManager_GameloftRRR.PuppetData(i, resl.ResourceList[mod.ResourceReferenceID], GetCommonDesign(puppets[mod.ResourceReferenceID]));
				} else {
					models[i] = new Unity_ObjectManager_GameloftRRR.PuppetData(i, null, null);
				}
			}
			return models;
		}

		public override async UniTask<Unity_Level> LoadAsync(Context context, bool loadTextures) {

			Controller.DetailedState = "Loading data";
			await Controller.WaitIfNecessary();

			var s = context.Deserializer;
			var levelList = LoadLevelList(context);


			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.Settings), context);
			var lh0 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 0, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Graphics, name: "LayerHeader0");
			var lh1 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 1, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Graphics, name: "LayerHeader1");
			var lh2 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 2, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Collision, name: "LayerHeader2");
			var l0 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 3, onPreSerialize: o => o.Header = lh0, name: "Layer0");
			var l1 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 4, onPreSerialize: o => o.Header = lh1, name: "Layer1");
			var l2 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 5, onPreSerialize: o => o.Header = lh2, name: "Layer2");
			
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetForegroundTileSetPath(context.Settings, levelList), context);
			var ts_f = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Foreground");
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetBackgroundTileSetPath(context.Settings, levelList), context);
			var ts_b = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Background");
			var tileSet_f = GetPuppetImages(ts_f, false);
			var tileSet_b = GetPuppetImages(ts_b, false);
			resf = FileFactory.Read<Gameloft_ResourceFile>(ObjectsFilePath, context);
			var objs = resf.SerializeResource<Gameloft_RRR_Objects>(s, default, context.Settings.Level * 2, name: "Objects");
			
			int cellSize = tileSet_f[0][0].width;

			// Pad foreground tileset with transparent tiles
			var tileset_f_padding = Enumerable.Repeat(TextureHelpers.CreateTexture2D(cellSize, cellSize, clear: true, applyClear: true).CreateTile(), 128 - tileSet_f.Length);
			var tileset_b_padding = Enumerable.Repeat(TextureHelpers.CreateTexture2D(cellSize, cellSize, clear: true, applyClear: true).CreateTile(), 128 - tileSet_b.Length);

			// Load maps
			var maps = new Unity_Map[]
			{
				// Background
				new Unity_Map
                {
                    Width = lh0.Width,
                    Height = lh0.Height,
                    TileSet = new Unity_TileSet[]
                    {
						new Unity_TileSet(tileSet_b.Select(x => x[0].CreateTile()).Concat(tileset_b_padding).ToArray())
                    },
                    MapTiles = l0.TileMap.Select(x => new Unity_Tile(x)).ToArray(),
                    Type = Unity_Map.MapType.Graphics,
					Layer = Unity_Map.MapLayer.Back
                },
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

			// Load objects
			var objManager = new Unity_ObjectManager_GameloftRRR(context, LoadPuppets(context), objs.Objects);
			var unityObjs = objs.Objects.Select((o, i) => (Unity_Object)(new Unity_Object_GameloftRRR(objManager, o))).ToList();

			// Set palette index for loops
			var world = GetWorldIndex(context.Settings, levelList);
			if (world == 2) {
				foreach (var uo in unityObjs) {
					if (((Unity_Object_GameloftRRR)uo).Object.Type == 8) {
						((Unity_Object_GameloftRRR)uo).PaletteIndex = 1;
					}
				}
			}

			// Return level
			return new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: unityObjs,
				localization: LoadLocalization(context),
				defaultMap: 1,
				defaultCollisionMap: 2,
                cellSize: cellSize);
		}
	}
}