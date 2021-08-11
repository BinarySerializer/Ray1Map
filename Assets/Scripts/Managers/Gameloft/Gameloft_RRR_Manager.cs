﻿using Cysharp.Threading.Tasks;

using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

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

		public virtual int[] HardcodedPuppetImageBufferIndices => null;

		public string ObjectsFilePath => "l0b";
		public virtual string FixFilePath => "d2";
		public virtual int LevelListResourceIndex => 0;
		public virtual int ObjectModelListResourceIndex => 1;
		public virtual int ResourceListResourceIndex => 2;
		public virtual int LocalizationResourceIndex => 3;

		public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(new GameInfo_World[]
		{
			new GameInfo_World(0, Enumerable.Range(0, 8).ToArray()),
		});



		public Gameloft_RRR_LevelList LoadLevelList(Context context) {
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(FixFilePath, context);
			return resf.SerializeResource<Gameloft_RRR_LevelList>(s, default, LevelListResourceIndex, name: "LevelList");
		}

		public virtual int GetWorldIndex(GameSettings settings, Gameloft_RRR_LevelList levelList) {
			return levelList.Levels[settings.Level].World;
		}

		public string GetLevelPath(GameSettings settings) => GetLevelPath(settings.Level);
		public string GetLevelPath(int level) => $"l0a0{level + 1}";
		public string GetBackgroundTileSetPath(GameSettings settings, Gameloft_RRR_LevelList levelList) => $"ts{GetWorldIndex(settings, levelList)}";
		public string GetForegroundTileSetPath(GameSettings settings, Gameloft_RRR_LevelList levelList) => $"t{GetWorldIndex(settings, levelList)}";
		public string GetPuppetPath(int i) => i >= 0 ?  $"s{i}" : "s";

		public override async UniTask LoadFilesAsync(Context context) {
			await context.AddLinearFileAsync(FixFilePath);
			var levelList = LoadLevelList(context);
			await context.AddLinearFileAsync(GetBackgroundTileSetPath(context.GetR1Settings(), levelList));
			await context.AddLinearFileAsync(GetForegroundTileSetPath(context.GetR1Settings(), levelList));
			await context.AddLinearFileAsync(GetLevelPath(context.GetR1Settings()));
			await context.AddLinearFileAsync(ObjectsFilePath);
			for (int i = -1; i < 9; i++) {
				await context.AddLinearFileAsync(GetPuppetPath(i));
			}
		}

		public KeyValuePair<string, string[]>[] LoadLocalization(Context context) {
			var langages = new string[]
			{
				"English"
			};
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(FixFilePath, context);
			var loc = resf.SerializeResource<Gameloft_RRR_LocalizationTable>(s, default, LocalizationResourceIndex, name: "Localization");

			return loc.LanguageTables.Select((x, i) => new KeyValuePair<string, string[]>(langages[i], x.Strings)).ToArray();
		}

		public Unity_ObjectManager_GameloftRRR.PuppetData[] LoadPuppets(Context context) {
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(FixFilePath, context);
			var modl = resf.SerializeResource<Gameloft_RRR_ObjectModelList>(s, default, ObjectModelListResourceIndex, name: "ObjectModelList");
			var resl = resf.SerializeResource<Gameloft_RRR_PuppetResourceList>(s, default, ResourceListResourceIndex, name: "ResourceList");

			Gameloft_Puppet[] puppets = new Gameloft_Puppet[resl.ResourceList.Length];
			for (int i = 0; i < puppets.Length; i++) {
				var rref = resl.ResourceList[i];
				resf = FileFactory.Read<Gameloft_ResourceFile>(GetPuppetPath(rref.FileID), context);
				puppets[i] = resf.SerializeResource<Gameloft_Puppet>(s, default, rref.ResourceID, onPreSerialize: p => p.UseOtherPuppetImageData = rref.Byte3 < 0, name: $"Puppets[{i}]");
			}
			var hardcodedIndices = HardcodedPuppetImageBufferIndices;
			if (hardcodedIndices != null) {
				var puppetsWithImageData = puppets.Where(p => !p.UseOtherPuppetImageData).ToArray();
				for (int i = 0; i < puppets.Length; i++) {
					if (puppets[i].UseOtherPuppetImageData) {
						puppets[i].ImageData = puppetsWithImageData[hardcodedIndices[i]].ImageData;
					}
				}
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

		public virtual Unity_Map[] LoadMaps(Context context, Gameloft_RRR_LevelList levelList) {
			var s = context.Deserializer;
			var resf = FileFactory.Read<Gameloft_ResourceFile>(GetLevelPath(context.GetR1Settings()), context);
			var lh0 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 0, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Graphics, name: "LayerHeader0");
			var lh1 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 1, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Graphics, name: "LayerHeader1");
			var lh2 = resf.SerializeResource<Gameloft_RRR_MapLayerHeader>(s, default, 2, onPreSerialize: o => o.Type = Gameloft_RRR_MapLayerHeader.LayerType.Collision, name: "LayerHeader2");
			var l0 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 3, onPreSerialize: o => o.Header = lh0, name: "Layer0");
			var l1 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 4, onPreSerialize: o => o.Header = lh1, name: "Layer1");
			var l2 = resf.SerializeResource<Gameloft_RRR_MapLayerData>(s, default, 5, onPreSerialize: o => o.Header = lh2, name: "Layer2");
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetForegroundTileSetPath(context.GetR1Settings(), levelList), context);
			var ts_f = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Foreground");
			resf = FileFactory.Read<Gameloft_ResourceFile>(GetBackgroundTileSetPath(context.GetR1Settings(), levelList), context);
			var ts_b = resf.SerializeResource<Gameloft_Puppet>(s, default, 0, name: "Background");
			var tileSet_f = GetPuppetImages(ts_f, flipY: false, allowTransparent: true);
			var tileSet_b = GetPuppetImages(ts_b, flipY: false, allowTransparent: false);


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

			return maps;

		}

		public override async UniTask<Unity_Level> LoadAsync(Context context) 
        {

			Controller.DetailedState = "Loading data";
			await Controller.WaitIfNecessary();

			var s = context.Deserializer;
			var levelList = LoadLevelList(context);

			var maps = LoadMaps(context, levelList);
			
			var resf = FileFactory.Read<Gameloft_ResourceFile>(ObjectsFilePath, context);
			var objs = resf.SerializeResource<Gameloft_RRR_Objects>(s, default, context.GetR1Settings().Level * 2, name: "Objects");

			// Load objects
			var objManager = new Unity_ObjectManager_GameloftRRR(context, LoadPuppets(context), objs.Objects);
			var unityObjs = objs.Objects.Select((o, i) => (Unity_Object)(new Unity_Object_GameloftRRR(objManager, o))).ToList();

			// Set palette index for loops
			var world = GetWorldIndex(context.GetR1Settings(), levelList);
			if (s.GetR1Settings().GameModeSelection != GameModeSelection.RaymanRavingRabbidsMobile_208x208_s40v3) {
				if (world == 2) {
					foreach (var uo in unityObjs) {
						if (((Unity_Object_GameloftRRR)uo).Object.Type == 8) {
							((Unity_Object_GameloftRRR)uo).PaletteIndex = 1;
						}
					}
				}
			}

			// Return level
			return new Unity_Level(
                maps: maps,
                objManager: objManager,
                eventData: unityObjs,
				localization: LoadLocalization(context),
				defaultLayer: 1,
				defaultCollisionLayer: 2,
				getCollisionTypeGraphicFunc: x => ((Gameloft_RRR_CollisionType)x).GetCollisionTypeGraphic(),
				getCollisionTypeNameFunc: x => ((Gameloft_RRR_CollisionType)x).ToString(),
                cellSize: (int)maps[0].TileSet[0].Tiles[0].rect.width);
		}
	}
}