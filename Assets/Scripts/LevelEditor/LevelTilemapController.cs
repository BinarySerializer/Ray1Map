using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace R1Engine
{
    public class LevelTilemapController : MonoBehaviour {
        /// <summary>
        /// References to specific tilemap gameObjects in inspector
        /// </summary>
        public Tilemap[] CollisionTilemaps;
        public SpriteRenderer[] GraphicsTilemaps;
        public bool[] IsLayerVisible { get; set; }

        public SpriteRenderer tilemapFull;
        public SpriteRenderer tilemapPreview;
        public SpriteRenderer tilemapGrid;
        public bool focusedOnTemplate = false;
        public LevelEditorBehaviour editor;

        public Grid grid;

        /// <summary>
        /// The events
        /// </summary>
        public GameObject Events;

        /// <summary>
        /// Palette buttons
        /// </summary>
        public Button[] paletteButtons;
        //The ui text
        public GameObject paletteText;
        //0 is auto
        public int currentPalette = 1;
        private int _currentPalette = 1;

        // Reference to the background ting
        public SpriteRenderer backgroundTint;

        public SpriteRenderer background;
        public SpriteRenderer backgroundParallax;

        /// <summary>
        /// The type collision tiles
        /// </summary>
        public Tile[] CollisionTiles;
        public Tile[] CollisionTilesHD;
        Dictionary<Unity_MapCollisionTypeGraphic, Tile> CurrentCollisionIcons = new Dictionary<Unity_MapCollisionTypeGraphic, Tile>();
        Dictionary<Unity_MapCollisionTypeGraphic, Dictionary<MapTile.GBAVV_CollisionTileShape, Tile>> CurrentCollisionIconsShaped = new Dictionary<Unity_MapCollisionTypeGraphic, Dictionary<MapTile.GBAVV_CollisionTileShape, Tile>>();

        public LineRenderer[] CollisionLines;
        public GameObject collisionLinesGraphics;
        public GameObject collisionLinesCollision;
        public BoxCollider2D[] CollisionLinesCollision;

        // Infro tracked for when switching between template and normal level
        private Vector3 previousCameraPosNormal;
        private Vector3 previousCameraPosTemplate = new Vector3(0, 0, -10f);
        private int templateMaxY = 0;

        public int camMaxX = 1;
        public int camMaxY = 1;

        public Material additiveMaterial;

        public float CellSizeInUnits { get; set; } = 1f;
        public float CellSizeInUnitsCollision { get; set; } = 1f;

        private Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>[] animatedTiles;
        private int?[][,][] TileIndexOverrides { get; set; }
        private HashSet<Vector2Int> changedTiles = new HashSet<Vector2Int>();

        public bool HasAutoPaletteOption => LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PC || 
                                            LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PocketPC ||
                                            LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PS1_Edu ||
                                            LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PC_Edu;

        public GameObject IsometricCollision { get; set; }
        public Material isometricCollisionMaterial;
        public Material unlitMaterial;

        public bool HasAnimatedTiles { get; private set; } = false;

        public const int Index_Background = -2;
        public const int Index_ParallaxBackground = -1;

        public void InitializeTilemaps() {
            var level = LevelEditorData.Level;

            CellSizeInUnits = level.CellSize / (float)level.PixelsPerUnit;
            CellSizeInUnitsCollision = (level.CellSizeOverrideCollision ?? level.CellSize) / (float)level.PixelsPerUnit;
            if (CellSizeInUnitsCollision != 1f) {
                grid.cellSize = new Vector3(CellSizeInUnitsCollision, CellSizeInUnitsCollision, 0);
            }

            if (level.Background != null) {
                bool wasTiled = background.drawMode == SpriteDrawMode.Tiled;
                if (wasTiled) SetGraphicsLayerTiled(Index_Background, false);
                var tex = level.Background;
                background.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 1), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);
                background.gameObject.SetActive(true);
                if (wasTiled) SetGraphicsLayerTiled(Index_Background, true);
            }
            if (level.ParallaxBackground != null) {
                bool wasTiled = background.drawMode == SpriteDrawMode.Tiled;
                if (wasTiled) SetGraphicsLayerTiled(Index_ParallaxBackground, false);
                var tex = level.ParallaxBackground;
                backgroundParallax.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 1), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);
                backgroundParallax.gameObject.SetActive(true);
                if (wasTiled) SetGraphicsLayerTiled(Index_ParallaxBackground, true);
            }

            // Resize the background tint
            ResizeBackgroundTint(LevelEditorData.MaxWidth, LevelEditorData.MaxHeight);

            // Disable palette buttons based on if there are 3 palettes or not
            if (!HasAutoPaletteOption) {
                paletteText.SetActive(false);
                paletteButtons[0].gameObject.SetActive(false);
                paletteButtons[1].gameObject.SetActive(false);
                paletteButtons[2].gameObject.SetActive(false);
                paletteButtons[3].gameObject.SetActive(false);
            }

            // Init layer visibility
            IsLayerVisible = new bool[LevelEditorData.Level.Maps.Length];
            for (int i = 0; i < IsLayerVisible.Length; i++) {
                IsLayerVisible[i] = true;
            }


            // Set collision tiles
            var collisionTileSet = Settings.UseHDCollisionSheet ? CollisionTilesHD : CollisionTiles;
            if (CellSizeInUnitsCollision != 1f) {
                collisionTileSet = collisionTileSet
                    .Select(t => {
                        if (t == null) return null;
                        Tile newT = ScriptableObject.CreateInstance<Tile>();
                        newT.sprite = Sprite.Create(t.sprite.texture, t.sprite.textureRect, new Vector2(0.5f,0.5f), t.sprite.pixelsPerUnit / CellSizeInUnitsCollision);
                        newT.sprite.name = t.sprite.name;
                        return newT;
                    }).ToArray();
            }
            var types = Enum.GetValues(typeof(Unity_MapCollisionTypeGraphic));
            foreach(var type in types) {
                var typed = (Unity_MapCollisionTypeGraphic)type;
                string name = typed.ToString();
                Tile collisionSprite = collisionTileSet.FirstOrDefault(tile => tile.sprite.name == name);
                if (collisionSprite != null) {
                    CurrentCollisionIcons[typed] = collisionSprite;
                } else {
                    if (typed == Unity_MapCollisionTypeGraphic.None) continue;
                    Tile t = collisionTileSet.FirstOrDefault(tile => tile.sprite.name == "Unknown");
                    if (t != null) {
                        CurrentCollisionIcons[typed] = t;
                    }
                }
            }

            RefreshCollisionTiles();

            // Fill out tiles
            currentPalette = HasAutoPaletteOption ? 0 : 1;
            RefreshTiles(currentPalette);

            var maxWidth = LevelEditorData.MaxWidth;
            var maxHeight = LevelEditorData.MaxHeight;

            // Set max cam sizes
            camMaxX = maxWidth;
            camMaxY = maxHeight;

            if (IsometricCollision == null && level.IsometricData != null) {
                IsometricCollision = level.IsometricData.GetCollisionVisualGameObject(isometricCollisionMaterial);
                IsometricCollision.SetActive(Settings.ShowCollision);
                level.IsometricData.GetCollisionCollidersGameObject();
            }

            // Create collision lines
            if (level.CollisionLines != null)
            {
                CollisionLines = level.CollisionLines.Select(x =>
                {
                    LineRenderer lr = new GameObject("CollisionLine").AddComponent<LineRenderer>();
                    lr.sortingLayerName = "Types";
                    lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                    lr.gameObject.transform.SetParent(collisionLinesGraphics.transform);
                    lr.material = Controller.obj.levelEventController.linkLineMaterial;
                    lr.material.color = x.LineColor;
                    lr.positionCount = 2;
                    lr.widthMultiplier = 0.095f;

                    lr.SetPositions(new Vector3[]
                    {
                        new Vector3(x.Pos_0.x / LevelEditorData.Level.PixelsPerUnit, -(x.Pos_0.y / LevelEditorData.Level.PixelsPerUnit)), 
                        new Vector3(x.Pos_1.x / LevelEditorData.Level.PixelsPerUnit, -(x.Pos_1.y / LevelEditorData.Level.PixelsPerUnit)), 
                    });

                    return lr;
                }).ToArray();

                CollisionLinesCollision = level.CollisionLines.Select((x,i) => {
                    BoxCollider2D bc = new GameObject("CollisionLine").AddComponent<BoxCollider2D>();
                    bc.gameObject.layer = LayerMask.NameToLayer("Collision Lines");
                    bc.gameObject.name = i.ToString();
                    Vector3 pos0 = new Vector3(x.Pos_0.x / LevelEditorData.Level.PixelsPerUnit, -(x.Pos_0.y / LevelEditorData.Level.PixelsPerUnit));
                    Vector3 pos1 = new Vector3(x.Pos_1.x / LevelEditorData.Level.PixelsPerUnit, -(x.Pos_1.y / LevelEditorData.Level.PixelsPerUnit));
                    bc.gameObject.transform.SetParent(collisionLinesCollision.transform);
                    bc.gameObject.transform.localPosition = Vector3.Lerp(pos0, pos1, 0.5f);
                    float angle = Mathf.Atan2(pos1.y - pos0.y, pos1.x - pos0.x) * Mathf.Rad2Deg;
                    bc.gameObject.transform.localRotation = Quaternion.Euler(0,0,angle);
                    bc.size = new Vector2((pos1-pos0).magnitude,0.2f);

                    return bc;
                }).ToArray();
            }
        }

        public void RefreshCollisionTiles() {

            var lvl = LevelEditorData.Level;
            var unsupportedTiles = new HashSet<int>();

            // Resize collision map array
            if (CollisionTilemaps.Length != lvl.Maps.Length) {
                Array.Resize(ref CollisionTilemaps, lvl.Maps.Length);
                for (int i = 1; i < CollisionTilemaps.Length; i++) {
                    var map = lvl.Maps[i];
                    if (map.Type.HasFlag(Unity_Map.MapType.Collision)) {
                        CollisionTilemaps[i] = Instantiate<Tilemap>(CollisionTilemaps[0], new Vector3(0, 0, -i), Quaternion.identity, CollisionTilemaps[0].transform.parent);
                        CollisionTilemaps[i].gameObject.name = "Tilemap Collision " + i;
                        if (lvl.Maps[i].Layer == Unity_Map.MapLayer.Front) {
                            Debug.Log($"Collision map {i} is in front");
                            Tilemap tr = CollisionTilemaps[i];
                            tr.GetComponent<TilemapRenderer>().sortingLayerName = "Types Front";
                        } else if (lvl.Maps[i].Layer == Unity_Map.MapLayer.Back) {
                            Debug.Log($"Collision map {i} is in the back");
                            Tilemap tr = CollisionTilemaps[i];
                            tr.GetComponent<TilemapRenderer>().sortingLayerName = "Types Back";
                        }
                    }
                }
                if (LevelEditorData.Level.Maps.Length > 0 && !LevelEditorData.Level.Maps[0].Type.HasFlag(Unity_Map.MapType.Collision)) {
                    Destroy(CollisionTilemaps[0].gameObject);
                    CollisionTilemaps[0] = null;
                }
            }

            // Fill out types first
            for (int i = 0; i < CollisionTilemaps.Length; i++) {
                var map = lvl.Maps[i];
                if(!map.Type.HasFlag(Unity_Map.MapType.Collision)) continue;
                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++) {
                        var t = map.MapTiles[y * map.Width + x];
                        var collisionIndex = t.Data.CollisionType;

                        // Get the collision index
                        var collisionType = LevelEditorData.Level.GetCollisionTypeGraphicFunc(collisionIndex);

                        // Make sure it's not out of bounds
                        if (collisionType != Unity_MapCollisionTypeGraphic.None &&
                            ((CurrentCollisionIcons[collisionType].sprite?.name.Contains("Unknown") ?? false) || collisionType.ToString().Contains("Unknown")))
                            unsupportedTiles.Add(collisionIndex);

                        if (t.Data.UsesCollisionShape && t.Data.GBAVV_CollisionShape.Value != MapTile.GBAVV_CollisionTileShape.Solid) {
                            // Set the collision tile
                            CollisionTilemaps[i].SetTile(new Vector3Int(x, y, 0), GetShapedCollisionTile(collisionType, t.Data.GBAVV_CollisionShape.Value));
                        } else {
                            // Set the collision tile
                            CollisionTilemaps[i].SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons.TryGetItem(collisionType));
                        }
                    }
                }
            }

            if (unsupportedTiles.Count > 0)
                Debug.LogWarning($"The following collision types are not supported: {String.Join(", ", unsupportedTiles)}");
        }

        private void ConfigureGraphicsMapLayer(int i) {
            var lvl = LevelEditorData.Level;
            if (lvl.Maps[i].Layer == Unity_Map.MapLayer.Front) {
                Debug.Log($"Graphics map {i} is in front");
                SpriteRenderer tr = GraphicsTilemaps[i];
                tr.sortingLayerName = "Tiles Front";
            } else if (lvl.Maps[i].Layer == Unity_Map.MapLayer.Back) {
                Debug.Log($"Graphics map {i} is in the back");
                SpriteRenderer tr = GraphicsTilemaps[i];
                tr.sortingLayerName = "Tiles Back";
            }
        }

        // Used to redraw all tiles with different palette (0 = auto, 1-3 = palette)
        public void RefreshTiles(int palette) 
        {
            Debug.Log($"Refreshing tiles with palette {palette}");

            // Change button visibility
            _currentPalette = palette;
            for (int i = 0; i < paletteButtons.Length; i++) {
                ColorBlock b = paletteButtons[i].colors;
                b.normalColor = _currentPalette == i ? new Color(1, 1, 1) : new Color(0.5f, 0.5f, 0.5f);
                paletteButtons[i].colors = b;
            }

            // Get the current level and map
            var lvl = LevelEditorData.Level;


            // If auto, refresh indexes
            if (palette == 0)
                lvl.AutoApplyPalette();

            // Refresh tiles for every map
            TileIndexOverrides = new int?[lvl.Maps.Length][,][];
            if (GraphicsTilemaps.Length != lvl.Maps.Length) {
                Array.Resize(ref GraphicsTilemaps, lvl.Maps.Length);
                for (int i = 1; i < GraphicsTilemaps.Length; i++) {
                    if(!lvl.Maps[i].Type.HasFlag(Unity_Map.MapType.Graphics)) continue;
                    GraphicsTilemaps[i] = Instantiate<SpriteRenderer>(GraphicsTilemaps[0], new Vector3(0, 0, -i), Quaternion.identity, GraphicsTilemaps[0].transform.parent);
                    GraphicsTilemaps[i].gameObject.name = "Tilemap Graphics " + i;
                    ConfigureGraphicsMapLayer(i);
                }
            }
            if (lvl.Maps.Length > 0) {
                if (!lvl.Maps[0].Type.HasFlag(Unity_Map.MapType.Graphics)) {
                    Destroy(GraphicsTilemaps[0].gameObject);
                    GraphicsTilemaps[0] = null;
                } else {
                    ConfigureGraphicsMapLayer(0);
                }
            }
            animatedTiles = new Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>[GraphicsTilemaps.Length];
            for (int mapIndex = 0; mapIndex < lvl.Maps.Length; mapIndex++) {
                var map = lvl.Maps[mapIndex];
                if(!map.Type.HasFlag(Unity_Map.MapType.Graphics)) continue;
                animatedTiles[mapIndex] = new Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>();
                if (map.IsAdditive) {
                    GraphicsTilemaps[mapIndex].material = additiveMaterial;
                }
                if (map.Alpha.HasValue) {
                    GraphicsTilemaps[mapIndex].color = new Color(1f, 1f, 1f, map.Alpha.Value);
                }
                int cellSize = LevelEditorData.Level.CellSize;
                Texture2D tex = TextureHelpers.CreateTexture2D(map.Width * cellSize, map.Height * cellSize);

                TileIndexOverrides[mapIndex] = new int?[map.Width, map.Height][];
                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++) {
                        var t = map.MapTiles[y * map.Width + x];

                        if (palette != 0)
                            t.PaletteIndex = palette;

                        if (t.PaletteIndex - 1 >= map.TileSet.Length)
                            t.PaletteIndex = 1;

                        TileIndexOverrides[mapIndex][x,y] = new int?[1 + (t.CombinedTiles?.Length ?? 0)];
                        AddAnimatedTile(map, mapIndex, t, x, y);
                        if (t.CombinedTiles != null) {
                            for (int i = 0; i < t.CombinedTiles.Length; i++) {
                                AddAnimatedTile(map, mapIndex, t.CombinedTiles[i], x,y,combinedTileIndex: i);
                            }
                        }
                        FillInTilePixels(tex, t, map, mapIndex, x, y, cellSize);

                        /*GraphicsTilemaps[mapIndex].SetTile(new Vector3Int(x, y, 0), map.GetTile(t, LevelEditorData.CurrentSettings));
                        GraphicsTilemaps[mapIndex].SetTransformMatrix(new Vector3Int(x, y, 0), GraphicsTilemaps[mapIndex].GetTransformMatrix(new Vector3Int(x, y, 0)) * Matrix4x4.Scale(new Vector3(t.Data.HorizontalFlip ? -1 : 1, t.Data.VerticalFlip ? -1 : 1, 1)));*/
                    }
                }
                tex.filterMode = FilterMode.Point;
                tex.Apply();

                bool wasTiled = GraphicsTilemaps[mapIndex].drawMode == SpriteDrawMode.Tiled;
                if (wasTiled) SetGraphicsLayerTiled(mapIndex, false);

                // Note: FullRect is important, otherwise when editing you need to create a new sprite
                GraphicsTilemaps[mapIndex].sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);

                // After setting sprite, set tiled
                if (wasTiled) SetGraphicsLayerTiled(mapIndex, true);
            }

            CreateTilemapGrid();
            CreateTilemapFull();
        }

        public void SetGraphicsLayerTiled(int mapIndex, bool tiled) {
            SpriteRenderer mapRenderer = null;
            var scale = Vector3.one;
            if (mapIndex < 0) {
                if (mapIndex == -1) mapRenderer = backgroundParallax;
                else if(mapIndex == -2) mapRenderer = background;
            } else {
                mapRenderer = GraphicsTilemaps[mapIndex];
                scale.y = -1;
            }
            if (tiled) {
                mapRenderer.drawMode = SpriteDrawMode.Tiled;
                mapRenderer.tileMode = SpriteTileMode.Continuous;
                mapRenderer.transform.localScale = scale;
                mapRenderer.size = new Vector2(LevelEditorData.MaxWidth, LevelEditorData.MaxHeight) * CellSizeInUnits;
            } else {
                mapRenderer.drawMode = SpriteDrawMode.Simple;
                mapRenderer.transform.localScale = scale;
                if (mapRenderer.sprite != null) {
                    // Restore original size
                    mapRenderer.size = mapRenderer.sprite.rect.size / mapRenderer.sprite.pixelsPerUnit;
                }
            }
        }

        private void AddAnimatedTile(Unity_Map map, int mapIndex, Unity_Tile t, int x, int y, int? combinedTileIndex = null) {
            Unity_TileTexture tile = map.GetTile(t, LevelEditorData.CurrentSettings);
            var atInstance = map.GetAnimatedTile(t, LevelEditorData.CurrentSettings);
            if (atInstance != null) {
                HasAnimatedTiles = true;
                atInstance.x = x;
                atInstance.y = y;
                atInstance.combinedTileIndex = combinedTileIndex;
                var at = atInstance.animatedTile;
                if (!animatedTiles[mapIndex].ContainsKey(at)) {
                    animatedTiles[mapIndex][at] = new List<Unity_AnimatedTile.Instance>();
                }
                animatedTiles[mapIndex][at].Add(atInstance);
            }
        }
        private void CreateTilemapGrid() {
            var lvl = LevelEditorData.Level;
            int mapWidth = lvl.GridMap.Width;
            int mapHeight = lvl.GridMap.Height;

            int cellSize = LevelEditorData.Level.CellSize;
            Texture2D tex = TextureHelpers.CreateTexture2D(mapWidth * cellSize, mapHeight * cellSize);

            var map = lvl.GridMap;

            for (int y = 0; y < map.Height; y++) {
                for (int x = 0; x < map.Width; x++) {
                    var t = map.MapTiles[y * map.Width + x];
                    FillInTilePixels(tex, t, lvl.GridMap, -1, x, y, cellSize);
                }
            }

            tex.Apply();
            tilemapGrid.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);
        }

        private void CreateTilemapFull() {
            var lvl = LevelEditorData.Level;
            int mapWidth = 16;
            if(lvl.Maps.Length == 0) return;
            int mapHeight = Mathf.CeilToInt(lvl.Maps[LevelEditorData.CurrentMap].TileSet[0].Tiles.Length / (float)mapWidth);

            int cellSize = LevelEditorData.Level.CellSize;
            Texture2D tex = TextureHelpers.CreateTexture2D(mapWidth * cellSize, mapHeight * cellSize);

            // Refresh the full tilemap template for current map
            int xx = 0;
            int yy = 0;

            var map = lvl.Maps[LevelEditorData.CurrentMap];

            foreach (Unity_TileTexture t in map.TileSet[0].Tiles) {
                FillInTilePixels_Single(tex, t, xx, yy, false, false, cellSize);
                xx++;
                if (xx == mapWidth) {
                    xx = 0;
                    yy++;
                }
            }

            templateMaxY = yy + 1;


            tex.filterMode = FilterMode.Point;
            tex.Apply();
            tilemapFull.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);
        }

        // Resize the background tint
        public void ResizeBackgroundTint(int x, int y) => backgroundTint.transform.localScale = new Vector2(CellSizeInUnits * x, CellSizeInUnits * y);

        // Used for switching the view between template and normal tiles
        public void ShowHideTemplate() {
            focusedOnTemplate = !focusedOnTemplate;
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                if(GraphicsTilemaps[i] != null) GraphicsTilemaps[i].gameObject.SetActive(!focusedOnTemplate);
            }
            tilemapFull.gameObject.SetActive(focusedOnTemplate);

            //Clear the selection square so it doesn't remain and look bad
            editor.tileSelectSquare.Clear();

            if (focusedOnTemplate) {
                editor.ClearSelection();
                //Save camera and set new
                previousCameraPosNormal = Camera.main.transform.position;
                Camera.main.GetComponent<EditorCam>().pos = previousCameraPosTemplate;
                //Resize the background tint for a better effect
                ResizeBackgroundTint(16, templateMaxY);
                //Set max cam sizes
                camMaxX = 16;
                camMaxY = templateMaxY;
            }
            else {
                //Set camera back
                previousCameraPosTemplate = Camera.main.transform.position;
                Camera.main.GetComponent<EditorCam>().pos = previousCameraPosNormal;

                var maxWidth = LevelEditorData.MaxWidth;
                var maxHeight = LevelEditorData.MaxHeight;

                // Resize background tint
                ResizeBackgroundTint(maxWidth, maxHeight);
                // Set max cam sizes
                camMaxX = maxWidth;
                camMaxY = maxHeight;
            }
        }

        // Converts mouse position to worldspace and then tile positions (1 = 16)
        public Vector3 MouseToTileCoords(Vector3 mousePos, bool collision = false) {
            var cs = collision ? CellSizeInUnitsCollision : CellSizeInUnits;
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector3(Mathf.Floor(worldMouse.x / cs) * cs, (Mathf.Floor(worldMouse.y / cs) + 1) * cs, 10);
        }
        public Vector2Int MouseToTileInt(Vector3 mousePos, bool collision = false) {
            var cs = collision ? CellSizeInUnitsCollision : CellSizeInUnits;
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector2Int(Mathf.FloorToInt(worldMouse.x / cs), -(Mathf.FloorToInt(worldMouse.y / cs) + 1));
        }

        public void SetPreviewTilemap(Unity_Map map, Unity_Tile[,] newTiles) {
            int w = newTiles.GetLength(0);
            int h = newTiles.GetLength(1);
            if (w <= 0 || h <= 0) {
                ClearPreviewTilemap();
                return;
            }
            int cellSize = LevelEditorData.Level.CellSize;
            Texture2D tex = new Texture2D(w * cellSize, h * cellSize);
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    Unity_Tile newTile = newTiles[x, y];
                    FillInTilePixels(tex, newTile, map, -1, x, y, cellSize, applyTexture: false);
                }
            }
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            tilemapPreview.sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0, 0), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);
        }
        public void ClearPreviewTilemap() {
            tilemapPreview.sprite = null;
        }

        // Get one common tile at given position
        public Unity_Tile GetTileAtPos(int x, int y) 
        {
            if (LevelEditorData.Level == null)
                return null;

            var map = LevelEditorData.Level.Maps[LevelEditorData.CurrentMap];

            if (focusedOnTemplate)
            {
                // Get the 1-dimensional graphic tile index
                var graphicIndex1D = (y * LevelEditorData.Level.CellSize) + x;

                if (graphicIndex1D > map.TileSet[0].Tiles.Length - 1)
                    graphicIndex1D = 0;

                Unity_Tile t = new Unity_Tile(new MapTile());

                t.Data.TileMapY = (ushort)Mathf.FloorToInt(graphicIndex1D / (float)map.TileSetWidth);
                t.Data.TileMapX = (ushort)(graphicIndex1D - (map.TileSetWidth * t.Data.TileMapY));

                return t;
            }

            var shallow_tile = map.GetMapTile(x, y);
            Unity_Tile u;
            if (shallow_tile == null) {
                u = new Unity_Tile(new MapTile());
            }
            else {
                u = shallow_tile.CloneObj();
            }
            return u;
        }

        public void SetTileBlockAtPos(int x, int y, int w, int h, Unity_Tile[] newTiles) {
            for (int y1 = y; y1 < y1 + h; y1++) {
                for (int x1 = x; x1 < x1 + w; x1++) {
                    SetTileAtPos(x1, y1, newTiles[y1 * w + x1], applyTexture: false);
                }
            }
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                if (GraphicsTilemaps[i] != null) GraphicsTilemaps[i].sprite.texture.Apply();
            }
        }
        public void SetTileBlockAtPos(int startX, int startY, int w, int h, Unity_Tile[,] newTiles) {
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    SetTileAtPos(startX + x, startY + y, newTiles[x,y], applyTexture: false);
                }
            }
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                if (GraphicsTilemaps[i] == null) continue;
                Texture2D tex = GraphicsTilemaps[i].sprite.texture;
                tex.Apply();
                //GraphicsTilemaps[i].sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0, 0), LevelEditorData.EditorManager.PixelsPerUnit, 0, SpriteMeshType.FullRect);
            }
        }

        private void FillInTilePixels(Texture2D tex, Unity_Tile t, Unity_Map map, int mapIndex, int x, int y, int cellSize, bool applyTexture = false, bool combined = false) {
            bool flipY = (t?.Data.VerticalFlip ?? false);
            bool flipX = (t?.Data.HorizontalFlip ?? false);
            var tile = map.GetTile(t, LevelEditorData.CurrentSettings, tileIndexOverride: mapIndex >= 0 ? TileIndexOverrides[mapIndex][x,y][0] : null);
            FillInTilePixels_Single(tex, tile, x, y, flipX, flipY, cellSize, applyTexture: false, overlay: false);

            // Write combined tiles
            if (t?.CombinedTiles?.Length > 0) {
                for(int i = 0; i < t.CombinedTiles.Length; i++) {
                    var ct = t.CombinedTiles[i];
                    var tileTex = map.GetTile(ct, LevelEditorData.CurrentSettings, tileIndexOverride: mapIndex >= 0 ? TileIndexOverrides[mapIndex][x, y][1 + i] : null);
                    FillInTilePixels_Single(tex, tileTex, x, y, ct.Data.HorizontalFlip, ct.Data.VerticalFlip, cellSize, applyTexture: false, overlay: true);
                }
            }
            if (applyTexture) tex.Apply();
        }

        private void FillInTilePixels_Single(Texture2D tex, Unity_TileTexture tile, int x, int y, bool flipX, bool flipY, int cellSize, bool applyTexture = false, bool overlay = false) {
            int texX = x * cellSize;
            int texY = y * cellSize;
            if (!overlay) {
                if (tile?.texture == null) {
                    tex.SetPixels(texX, texY, cellSize, cellSize, new Color[cellSize * cellSize]);
                } else {
                    tex.SetPixels(texX, texY, cellSize, cellSize, tile.GetPixels(flipX, flipY));
                }
            } else {
                if (tile?.texture != null) {
                    Color[] px = tile.GetPixels(flipX, flipY);
                    for (int yy = 0; yy < cellSize; yy++) {
                        for (int xx = 0; xx < cellSize; xx++) {
                            var c = px[(int)(yy * cellSize + xx)];

                            if (c != Color.clear)
                                tex.SetPixel(texX + xx, texY + yy, c);
                        }
                    }
                }
            }

            if (applyTexture) tex.Apply();
        }

        public void SetCollisionTileRendererAtPos(int x, int y, int mapIndex, Unity_Tile newTile, bool setNull = true) {
            if (CollisionTilemaps[mapIndex] == null) return;
            if(setNull) CollisionTilemaps[mapIndex].SetTile(new Vector3Int(x, y, 0), null);
            var type = LevelEditorData.Level.GetCollisionTypeGraphicFunc(newTile.Data.CollisionType);
            if (CurrentCollisionIcons.ContainsKey(type)) {
                CollisionTilemaps[mapIndex].SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons[type]);
            }
        }

        public Tile GetShapedCollisionTile(Unity_MapCollisionTypeGraphic type, MapTile.GBAVV_CollisionTileShape shape) {
            if (type == Unity_MapCollisionTypeGraphic.Solid && shape == MapTile.GBAVV_CollisionTileShape.None) return null;
            var t = CurrentCollisionIcons.TryGetItem(type);
            if (shape == MapTile.GBAVV_CollisionTileShape.Solid) return t;
            if (t == null) return t;
            if (CurrentCollisionIconsShaped.ContainsKey(type) && CurrentCollisionIconsShaped[type].ContainsKey(shape)) {
                return CurrentCollisionIconsShaped[type][shape];
            }
            Tile newT = ScriptableObject.CreateInstance<Tile>();
            Texture2D tex = TextureHelpers.CreateTexture2D((int)t.sprite.textureRect.width, (int)t.sprite.textureRect.height);
            Color[] pixels = t.sprite.texture.GetPixels((int)t.sprite.textureRect.x, (int)t.sprite.textureRect.y, tex.width, tex.height);
            float alpha = 0.4f;
            switch (shape) {
                case MapTile.GBAVV_CollisionTileShape.None:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Left:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (x + y > (tex.width + tex.height) / 2) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Half_Left_1:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (x + (y- tex.height / 2) * 2 > (tex.width + tex.height) / 2 && y >= tex.height / 2) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Half_Left_2:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (y >= tex.height / 2 || x + y*2 > (tex.width + tex.height) / 2) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Third_Left_1:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (x + (y - tex.height * (2 / 3f)) * 3 > (tex.width + tex.height) / 2 && y >= tex.height * (2 / 3f)) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Third_Left_2:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (y >= tex.height * (2 / 3f) ||
                                (x + (y - tex.height * (1 / 3f)) * 3 > (tex.width + tex.height) / 2
                                && y >= tex.height * (1 / 3f))) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Third_Left_3:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (y >= tex.height * (1 / 3f) ||
                                (x + y * 3 > (tex.width + tex.height) / 2)) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Third_Right_3:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if ((tex.width - 1 - x) + (y - tex.height * (2 / 3f)) * 3 > (tex.width + tex.height) / 2 && y >= tex.height * (2/3f)) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Third_Right_2:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (y >= tex.height * (2 / 3f) ||
                                ((tex.width - 1 - x) + (y - tex.height * (1 / 3f)) * 3 > (tex.width + tex.height) / 2
                                && y >= tex.height * (1 / 3f))) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Third_Right_1:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (y >= tex.height * (1 / 3f) ||
                                ((tex.width - 1 - x) + y * 3 > (tex.width + tex.height) / 2)) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Half_Right_2:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if ((tex.width - 1 - x) + (y - tex.height / 2) * 2 > (tex.width + tex.height) / 2 && y >= tex.height / 2) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Half_Right_1:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if (y >= tex.height / 2 || (tex.width - 1 - x) + y * 2 > (tex.width + tex.height) / 2) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                case MapTile.GBAVV_CollisionTileShape.Hill_Right:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            var pix = pixels[y * tex.width + x];
                            if ((tex.width-1-x) + y > (tex.width + tex.height) / 2) {
                                tex.SetPixel(x, y, pix);
                            } else {
                                tex.SetPixel(x, y, new Color(pix.r, pix.g, pix.b, pix.a * alpha));
                            }
                        }
                    }
                    break;
                default:
                    for (int y = 0; y < tex.height; y++) {
                        for (int x = 0; x < tex.width; x++) {
                            tex.SetPixel(x, y, Color.clear);
                        }
                    }
                    break;
            }
            tex.Apply();
            newT.sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f, 0.5f), t.sprite.pixelsPerUnit);
            newT.sprite.name = t.sprite.name;
            if (!CurrentCollisionIconsShaped.ContainsKey(type)) CurrentCollisionIconsShaped[type] = new Dictionary<MapTile.GBAVV_CollisionTileShape, Tile>();
            CurrentCollisionIconsShaped[type][shape] = newT;
            return newT;
        }

        public void SetTileAtPos(int x, int y, Unity_Tile newTile, bool applyTexture = true) 
        {
            var map = LevelEditorData.Level.Maps[LevelEditorData.CurrentMap];
            if (y < 0 || y >= map.Height) return;
            if (x < 0 || x >= map.Width) return;

            // Update tile graphics
            for (int i = 0; i < CollisionTilemaps.Length; i++) {
                SetCollisionTileRendererAtPos(x,y,i,newTile);
            }
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                if (GraphicsTilemaps[i] == null) continue;
                Texture2D tex = GraphicsTilemaps[i].sprite.texture;

                int cellSize = LevelEditorData.Level.CellSize;
                FillInTilePixels(tex, newTile, map, i, x, y, cellSize, applyTexture: applyTexture);
            }

            // Get the tile to set
            var destTile = map.MapTiles[y * map.Width + x];

            // Update destination tile values
            destTile.Data.CollisionType = newTile.Data.CollisionType;
            destTile.PaletteIndex = newTile.PaletteIndex;
            destTile.Data.TileMapX = newTile.Data.TileMapX;
            destTile.Data.TileMapY = newTile.Data.TileMapY;
            destTile.Data.PC_Unk1 = newTile.Data.PC_Unk1;
            destTile.Data.PC_Unk2 = newTile.Data.PC_Unk2;

            // Get the correct transparency mode to set if available
            if (map.TileSetTransparencyModes != null)
                destTile.Data.PC_TransparencyMode = map.TileSetTransparencyModes[(map.TileSetWidth * newTile.Data.TileMapY) + newTile.Data.TileMapX];
        }

        public Unity_Tile SetTypeAtPos(int x, int y, ushort collisionType) 
        {
            var map = LevelEditorData.Level.Maps[LevelEditorData.CurrentMap];

            // Update tile graphics
            for (int i = 0; i < CollisionTilemaps.Length; i++) {
                if(CollisionTilemaps[i] == null) continue;
                CollisionTilemaps[i].SetTile(new Vector3Int(x, y, 0), null);
                var type = LevelEditorData.Level.GetCollisionTypeGraphicFunc(collisionType);
                if (CurrentCollisionIcons.ContainsKey(type)) {
                    CollisionTilemaps[i].SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons[type]);
                }
            }
            // Get the tile to set
            var destTile = map.MapTiles[y * map.Width + x];

            destTile.Data.CollisionType = collisionType;

            return destTile;
        }

        private void CheckPaletteChange() {
            if (_currentPalette != currentPalette) {
                Controller.obj.levelController.controllerTilemap.RefreshTiles(currentPalette);
            }
        }

		private void Update()
        {
            if (Controller.LoadState != Controller.State.Finished) return;

            // Enforce layer visibility
            UpdateMapLayersVisibility();

            if (Settings.ShowGridMap != tilemapGrid.gameObject.activeSelf) {
                tilemapGrid.gameObject.SetActive(Settings.ShowGridMap);
            }

            CheckPaletteChange();

            if (animatedTiles == null || !Settings.AnimateTiles) return;

            for (int mapIndex = 0; mapIndex < animatedTiles.Length; mapIndex++) {
                if (GraphicsTilemaps[mapIndex] == null) continue;
                bool changedTile = false;
                changedTiles.Clear();
                var map = LevelEditorData.Level.Maps[mapIndex];
                Texture2D tex = GraphicsTilemaps[mapIndex].sprite.texture;
                foreach (var animatedTile in animatedTiles[mapIndex].Keys) {
                    foreach (var at in animatedTiles[mapIndex][animatedTile]) 
                    {
                        var animSpeed = (animatedTile.AnimationSpeeds?[at.tileIndex] ?? animatedTile.AnimationSpeed) / 30f;
                        //print("Updating " + at.x + " - " + at.y);
                        at.currentTimer += Time.deltaTime;

                        if (!(at.currentTimer >= animSpeed)) 
                            continue;

                        int frames = Mathf.FloorToInt(at.currentTimer / animSpeed);
                        int oldTileIndex = at.tileIndex;
                        at.tileIndex = (oldTileIndex + frames) % at.animatedTile.TileIndices.Length;

                        int oldIndexInTileset = at.animatedTile.TileIndices[oldTileIndex];
                        int newIndexInTileset = at.animatedTile.TileIndices[at.tileIndex];
                        if (oldIndexInTileset != newIndexInTileset) 
                        {
                            changedTile = true;
                            Vector2Int v = new Vector2Int(at.x,at.y);
                            if(!changedTiles.Contains(v)) changedTiles.Add(v);
                            TileIndexOverrides[mapIndex][at.x, at.y][at.combinedTileIndex.HasValue ? at.combinedTileIndex.Value + 1 : 0] = newIndexInTileset;
                        }
                        at.currentTimer -= frames * animSpeed;
                    }
                }
                int cellSize = LevelEditorData.Level.CellSize;
                foreach (var ct in changedTiles) {
                    var newTile = map.MapTiles[ct.y * map.Width + ct.x];
                    FillInTilePixels(tex, newTile, map, mapIndex, ct.x, ct.y, cellSize, applyTexture: false);
                }
                if (changedTile)
                    tex.Apply();
            }
        }

        public void UpdateMapLayersVisibility()
        {
            if (IsLayerVisible != null)
            {
                for (int i = 0; i < IsLayerVisible.Length; i++)
                {
                    if (GraphicsTilemaps != null && GraphicsTilemaps[i] != null)
                    {
                        if (GraphicsTilemaps[i].gameObject.activeSelf != IsLayerVisible[i])
                        {
                            GraphicsTilemaps[i].gameObject.SetActive(IsLayerVisible[i]);
                        }
                    }
                    if (CollisionTilemaps != null && CollisionTilemaps[i] != null)
                    {
                        if (CollisionTilemaps[i].gameObject.activeSelf != IsLayerVisible[i])
                        {
                            CollisionTilemaps[i].gameObject.SetActive(IsLayerVisible[i]);
                        }
                    }
                }
            }
        }

        public class AnimatedTileProperties {
            public int x;
            public int y;
            public int tileIndex;
            public float currentTimer;

            public AnimatedTileProperties(int x, int y) {
                this.x = x;
                this.y = y;
            }
        }
	}
}