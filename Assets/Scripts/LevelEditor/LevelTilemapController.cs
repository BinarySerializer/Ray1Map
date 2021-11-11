using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Ray1Map
{
    public class LevelTilemapController : MonoBehaviour {
        public bool[] IsLayerVisible { get; set; }

        // Prefabs
        public SpriteRenderer PrefabMapLayerRenderer;
        public SpriteRenderer PrefabTextureLayerRenderer;
        public Tilemap PrefabMapLayerCollision;
        // Parents
        public Transform ParentMapLayer;
        public Transform ParentTextureLayer;
        public Transform ParentMapLayerCollision;

        public SpriteRenderer tilemapFull;
        public SpriteRenderer tilemapPreview;
        public SpriteRenderer tilemapGrid;
        public bool focusedOnTemplate = false;
        public LevelEditorBehaviour editor;

        public Grid grid;

        readonly double GoldenRatio = (1 + Math.Sqrt(5)) / 2;

        const int layerStartZ = 100;

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

        // Reference to the background tint
        public SpriteRenderer backgroundTint;

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
        public BoxCollider[] CollisionLinesCollision3D;

        // Infro tracked for when switching between template and normal level
        private Vector3 previousCameraPosNormal;
        private Vector3 previousCameraPosTemplate = new Vector3(0, 0, -10f);
        private int templateMaxY = 0;

        public Rect TilemapBounds { get; set; } = new Rect(Vector2.zero, Vector2.one);
        public Rect CameraBounds { get; set; } = new Rect(Vector2.zero, Vector2.one);

        public Material additiveMaterial;

        public float CellSizeInUnits { get; set; } = 1f;
        public float CellSizeInUnitsCollision { get; set; } = 1f;

        private HashSet<Vector2Int> changedTiles = new HashSet<Vector2Int>();

        public bool HasAutoPaletteOption => LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PC || 
                                            LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PocketPC ||
                                            LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PS1_Edu ||
                                            LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PC_Edu;

        public GameObject IsometricCollision { get; set; }
        public Material isometricCollisionMaterial;

        // 3D Visual Materials
        public Material unlitMaterial;
        public Material unlitTransparentCutoutMaterial;
        public Material unlitAdditiveMaterial;
        public Material unlitTransparentMaterial;

        public bool IsAnimated { get; private set; } = false;

        public void InitializeLevelLayerRenderers(Unity_Level level) {
            if (level.Layers == null) return;
            for (int layerIndex = 0; layerIndex < level.Layers.Length; layerIndex++) {
                var l = level.Layers[layerIndex];
                if(l == null) continue;
                switch (l) {
                    case Unity_Layer_Texture lt:
                        if(lt.Graphics != null) break;
                        lt.Graphics = Instantiate<SpriteRenderer>(PrefabTextureLayerRenderer, lt.PositionOffset + new Vector3(0, 0, layerStartZ - layerIndex), Quaternion.identity, ParentTextureLayer);
                        lt.Graphics.gameObject.name = lt.Name;
                        lt.InitSprites(LevelEditorData.Level.PixelsPerUnit);

                        bool wasTiled = lt.Graphics.drawMode == SpriteDrawMode.Tiled;
                        if (wasTiled) SetGraphicsLayerTiled(layerIndex, false);
                        lt.Graphics.sprite = lt.MainSprite;
                        lt.Graphics.gameObject.SetActive(true);
                        if (wasTiled) SetGraphicsLayerTiled(layerIndex, true);
                        break;
                    case Unity_Layer_Map lm:
                        break;
                }
            }
        }

        public void InitializeTilemaps() {
            var level = LevelEditorData.Level;
            if(level.Layers == null) level.InitializeDefaultLayers();

            CellSizeInUnits = level.CellSize / (float)level.PixelsPerUnit;
            CellSizeInUnitsCollision = (level.CellSizeOverrideCollision ?? level.CellSize) / (float)level.PixelsPerUnit;
            if (CellSizeInUnitsCollision != 1f) {
                grid.cellSize = new Vector3(CellSizeInUnitsCollision, CellSizeInUnitsCollision, 0);
            }
            InitializeLevelLayerRenderers(level);


            // Get level size and update editor bounds
            SetTilemapBounds(LevelEditorData.MinX, LevelEditorData.MinY, LevelEditorData.MaxX, LevelEditorData.MaxY);
            
            // Disable palette buttons based on if there are 3 palettes or not
            if (!HasAutoPaletteOption) {
                paletteText.SetActive(false);
                paletteButtons[0].gameObject.SetActive(false);
                paletteButtons[1].gameObject.SetActive(false);
                paletteButtons[2].gameObject.SetActive(false);
                paletteButtons[3].gameObject.SetActive(false);
            }

            // Init layer visibility
            IsLayerVisible = new bool[LevelEditorData.Level.Layers.Length];
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
                    //lr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
                    lr.gameObject.layer = x.Is3D ? LayerMask.NameToLayer("3D Collision Lines") : LayerMask.NameToLayer("Collision Lines");
                    lr.gameObject.transform.SetParent(collisionLinesGraphics.transform);
                    lr.material = Controller.obj.levelEventController.linkLineMaterial;
                    lr.material.color = x.LineColor;
                    lr.positionCount = 2;
                    lr.widthMultiplier = x.UnityWidth;

                    lr.SetPositions(new Vector3[]
                    {
                        x.GetUnityPosition(0, LevelEditorData.Level.IsometricData),
                        x.GetUnityPosition(1, LevelEditorData.Level.IsometricData),
                    });

                    return lr;
                }).ToArray();

                CollisionLinesCollision = level.CollisionLines.Select((x,i) => {
                    if (x.Is3D) {
                        return null;
                    } else {
                        BoxCollider2D bc = new GameObject("CollisionLine").AddComponent<BoxCollider2D>();
                        bc.gameObject.layer = LayerMask.NameToLayer("Collision Lines");
                        bc.gameObject.name = i.ToString();
                        Vector3 pos0 = x.GetUnityPosition(0, LevelEditorData.Level.IsometricData);
                        Vector3 pos1 = x.GetUnityPosition(1, LevelEditorData.Level.IsometricData);
                        bc.gameObject.transform.SetParent(collisionLinesCollision.transform);
                        bc.gameObject.transform.localPosition = Vector3.Lerp(pos0, pos1, 0.5f);
                        float angle = Mathf.Atan2(pos1.y - pos0.y, pos1.x - pos0.x) * Mathf.Rad2Deg;
                        bc.gameObject.transform.localRotation = Quaternion.Euler(0, 0, angle);
                        bc.size = new Vector2((pos1 - pos0).magnitude, 0.2f);

                        return bc;
                    }
                }).ToArray();
                CollisionLinesCollision3D = level.CollisionLines.Select((x, i) => {
                    if (x.Is3D) {
                        BoxCollider bc = new GameObject("CollisionLine").AddComponent<BoxCollider>();
                        bc.gameObject.layer = LayerMask.NameToLayer("3D Collision Lines");
                        bc.gameObject.name = i.ToString();
                        Vector3 pos0 = x.GetUnityPosition(0, LevelEditorData.Level.IsometricData);
                        Vector3 pos1 = x.GetUnityPosition(1, LevelEditorData.Level.IsometricData);
                        bc.gameObject.transform.SetParent(collisionLinesCollision.transform);
                        bc.gameObject.transform.localPosition = Vector3.Lerp(pos0, pos1, 0.5f);
                        if (pos1 - pos0 != Vector3.zero) {
                            bc.gameObject.transform.localRotation = Quaternion.LookRotation(pos1 - pos0, Vector3.up);
                            bc.size = new Vector3(0.2f, 0.2f, (pos1 - pos0).magnitude);
                        } else {
                            bc.size = new Vector3(0.2f, 0.2f, 0.2f);
                        }

                        return bc;
                    } else {
                        return null;
                    }
                }).ToArray();
            }
        }

        private void SetTilemapBounds(float minX, float minY, float maxX, float maxY) {
            TilemapBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
            UpdateCameraBounds();
            ResizeBackgroundTint();

        }

        private void UpdateCameraBounds() {
            var w = TilemapBounds.width;
            var h = TilemapBounds.height;
            CameraBounds = new Rect(
                TilemapBounds.xMin * CellSizeInUnits,
                -TilemapBounds.yMax * CellSizeInUnits,
                w * CellSizeInUnits,
                h * CellSizeInUnits);
            // Set max zoom for camera
            Controller.obj.levelController.editor.cam.maxZoomOrthographic =
                Mathf.Max(Controller.obj.levelController.editor.cam.minZoomOrthographic,
                (float)(w * CellSizeInUnits / GoldenRatio),
                (float)(h * CellSizeInUnits / GoldenRatio));
        }

        public void RefreshCollisionTiles() {

            var lvl = LevelEditorData.Level;
            var unsupportedTiles = new HashSet<int>();

            // Initialize collision renderers for all collision map layers
            for (int i = 0; i < lvl.Layers.Length; i++) {
                var l = lvl.Layers[i];
                if (l is Unity_Layer_Map lm) {
                    var map = lm.Map;
                    if (map.Type.HasFlag(Unity_Map.MapType.Collision)) {
                        lm.CollisionTilemap = Instantiate<Tilemap>(PrefabMapLayerCollision, lm.PositionOffset + new Vector3(0, 0, layerStartZ - i), Quaternion.identity, ParentMapLayerCollision);
                        lm.CollisionTilemap.gameObject.name = $"{lm.Name} - Collision";
                        lm.Collision = lm.CollisionTilemap.GetComponent<TilemapRenderer>();
                        ConfigureCollisionMapLayer(i);
                    }
                }
            }

            // Fill out types first
            for (int i = 0; i < lvl.Layers.Length; i++) {
                var lm = lvl.Layers[i] as Unity_Layer_Map;
                if(lm == null) continue;
                var map = lm.Map;
                if (!map.Type.HasFlag(Unity_Map.MapType.Collision)) continue;
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
                            lm.CollisionTilemap.SetTile(new Vector3Int(x, y, 0), GetShapedCollisionTile(collisionType, t.Data.GBAVV_CollisionShape.Value));
                        } else {
                            // Set the collision tile
                            lm.CollisionTilemap.SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons.TryGetItem(collisionType));
                        }
                    }
                }
            }

            if (unsupportedTiles.Count > 0)
                Debug.LogWarning($"The following collision types are not supported: {String.Join(", ", unsupportedTiles)}");
        }

        private void ConfigureGraphicsMapLayer(int i) {
            var lvl = LevelEditorData.Level;
            Unity_Map.MapLayer mapLayer = Unity_Map.MapLayer.Middle;
            SpriteRenderer tr = null;
            switch (lvl.Layers[i]) {
                case Unity_Layer_Map lm:
                    mapLayer = lm.Map.Layer;
                    tr = lm.Graphics;
                    break;
                case Unity_Layer_Texture lt:
                    mapLayer = lt.Layer;
                    tr = lt.Graphics;
                    break;
            }
            if(tr == null) return;
            if (mapLayer == Unity_Map.MapLayer.Front) {
                //Debug.Log($"Graphics map {i} is in front");
                tr.sortingLayerName = "Tiles Front";
            } else if (mapLayer == Unity_Map.MapLayer.Back) {
                //Debug.Log($"Graphics map {i} is in the back");
                tr.sortingLayerName = "Tiles Back";
            } else {
                tr.sortingLayerName = "Tiles";
            }
            tr.sortingOrder = 0;
        }

        private void ConfigureCollisionMapLayer(int i) {
            var lvl = LevelEditorData.Level;
            Unity_Map.MapLayer mapLayer = Unity_Map.MapLayer.Middle;
            TilemapRenderer tr = null;
            switch (lvl.Layers[i]) {
                case Unity_Layer_Map lm:
                    mapLayer = lm.Map.Layer;
                    tr = lm.Collision;
                    break;
            }
            if (tr == null) return;
            if (mapLayer == Unity_Map.MapLayer.Front) {
                //Debug.Log($"Collision map {i} is in front");
                tr.sortingLayerName = "Types Front";
            } else if (mapLayer == Unity_Map.MapLayer.Back) {
                //Debug.Log($"Collision map {i} is in the back");
                tr.sortingLayerName = "Types Back";
            } else {
                tr.sortingLayerName = "Types";
            }
            tr.sortingOrder = 0;
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

            IsAnimated = IsAnimated || lvl.Layers.Any(l => l.IsAnimated);

            // If auto, refresh indexes
            if (palette == 0)
                lvl.AutoApplyPalette();

            // Initialize renderers for all graphics layers
            for (int i = 0; i < lvl.Layers.Length; i++) {
                var l = lvl.Layers[i];
                if (l is Unity_Layer_Map lm) {
                    var map = lm.Map;
                    if (map.Type.HasFlag(Unity_Map.MapType.Graphics)) {
                        if (lm.Graphics == null) {
                            lm.Graphics = Instantiate<SpriteRenderer>(PrefabMapLayerRenderer, lm.PositionOffset + new Vector3(0, 0, layerStartZ - i), Quaternion.identity, ParentMapLayer);
                        }
                        lm.Graphics.gameObject.name = $"{lm.Name} - Graphics";
                        ConfigureGraphicsMapLayer(i);
                    }
                }
            }
            for (int i = 0; i < lvl.Layers.Length; i++) {
                var lm = lvl.Layers[i] as Unity_Layer_Map;
                if (lm == null) continue;
                var map = lm.Map;
                if (!map.Type.HasFlag(Unity_Map.MapType.Graphics)) continue;
                lm.AnimatedTiles = new Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>();
                if (map.IsAdditive) lm.Graphics.material = additiveMaterial;
                if (map.Alpha.HasValue) lm.Graphics.color = new Color(1f, 1f, 1f, map.Alpha.Value);

                int cellSize = LevelEditorData.Level.CellSize;
                Texture2D tex = TextureHelpers.CreateTexture2D(map.Width * cellSize, map.Height * cellSize);

                lm.TileIndexOverrides = new int?[map.Width, map.Height][];
                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++) {
                        var t = map.MapTiles[y * map.Width + x];

                        if (palette != 0)
                            t.PaletteIndex = palette;

                        if (t.PaletteIndex - 1 >= map.TileSet.Length)
                            t.PaletteIndex = 1;

                        lm.TileIndexOverrides[x,y] = new int?[1 + (t.CombinedTiles?.Length ?? 0)];
                        AddAnimatedTile(map, i, t, x, y);
                        if (t.CombinedTiles != null) {
                            for (int ct = 0; ct < t.CombinedTiles.Length; ct++) {
                                AddAnimatedTile(map, i, t.CombinedTiles[ct], x,y,combinedTileIndex: ct);
                            }
                        }
                        FillInTilePixels(tex, t, map, i, x, y, cellSize);

                        /*GraphicsTilemaps[mapIndex].SetTile(new Vector3Int(x, y, 0), map.GetTile(t, LevelEditorData.CurrentSettings));
                        GraphicsTilemaps[mapIndex].SetTransformMatrix(new Vector3Int(x, y, 0), GraphicsTilemaps[mapIndex].GetTransformMatrix(new Vector3Int(x, y, 0)) * Matrix4x4.Scale(new Vector3(t.Data.HorizontalFlip ? -1 : 1, t.Data.VerticalFlip ? -1 : 1, 1)));*/
                    }
                }
                tex.filterMode = FilterMode.Point;
                tex.Apply();

                bool wasTiled = lm.Graphics.drawMode == SpriteDrawMode.Tiled;
                if (wasTiled) SetGraphicsLayerTiled(i, false);

                // Note: FullRect is important, otherwise when editing you need to create a new sprite
                lm.Graphics.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);

                // After setting sprite, set tiled
                if (wasTiled) SetGraphicsLayerTiled(i, true);
            }

            if(lvl.GridMap != null) CreateTilemapGrid();
            CreateTilemapFull();
        }

        public void SetGraphicsLayerTiled(int layerIndex, bool tiled) {
            SpriteRenderer renderer = null;
            var scale = Vector3.one;
            var layer = LevelEditorData.Level.Layers[layerIndex];
            switch (layer) {
                case Unity_Layer_Map lm:
                    renderer = lm.Graphics;
                    scale.y = -1;
                    break;
                case Unity_Layer_Texture lt:
                    renderer = lt.Graphics;
                    break;
            }
            if (tiled) {
                renderer.drawMode = SpriteDrawMode.Tiled;
                renderer.tileMode = SpriteTileMode.Continuous;
                renderer.transform.localScale = scale;
                renderer.size = new Vector2(LevelEditorData.MaxX, LevelEditorData.MaxY) * CellSizeInUnits;
            } else {
                renderer.drawMode = SpriteDrawMode.Simple;
                renderer.transform.localScale = scale;
                if (renderer.sprite != null) {
                    // Restore original size
                    renderer.size = renderer.sprite.rect.size / renderer.sprite.pixelsPerUnit;
                }
            }
        }

        private void AddAnimatedTile(Unity_Map map, int layerIndex, Unity_Tile t, int x, int y, int? combinedTileIndex = null) {
            Unity_TileTexture tile = map.GetTile(t, LevelEditorData.CurrentSettings);
            var atInstance = map.GetAnimatedTile(t, LevelEditorData.CurrentSettings);
            if (atInstance != null) {
                IsAnimated = true;
                atInstance.x = x;
                atInstance.y = y;
                atInstance.combinedTileIndex = combinedTileIndex;
                var at = atInstance.animatedTile;
                var l = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
                if (l != null) {
                    l.HasAnimatedTiles = true;
                    if (!l.AnimatedTiles.ContainsKey(at)) {
                        l.AnimatedTiles[at] = new List<Unity_AnimatedTile.Instance>();
                    }
                    l.AnimatedTiles[at].Add(atInstance);
                }
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
            if(lvl.Maps == null || lvl.Maps.Length == 0) return;
            var layer = LevelEditorData.Level.Layers[LevelEditorData.CurrentLayer] as Unity_Layer_Map;
            if(layer == null) return;
            var map = layer.Map;
            int mapHeight = Mathf.CeilToInt(map.TileSet[0].Tiles.Length / (float)mapWidth);

            int cellSize = LevelEditorData.Level.CellSize;
            Texture2D tex = TextureHelpers.CreateTexture2D(mapWidth * cellSize, mapHeight * cellSize);

            // Refresh the full tilemap template for current map
            int xx = 0;
            int yy = 0;

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
        public void ResizeBackgroundTint() {
            var z = backgroundTint.transform.localPosition.z;
            var rect = CameraBounds;
            backgroundTint.transform.localPosition = new Vector3(rect.xMin, rect.yMax, z);
            backgroundTint.transform.localScale = new Vector3(rect.width, rect.height);
        }

        // Used for switching the view between template and normal tiles
        public void ShowHideTemplate() {
            focusedOnTemplate = !focusedOnTemplate;
            UpdateLayersVisibility();
            tilemapFull.gameObject.SetActive(focusedOnTemplate);

            //Clear the selection square so it doesn't remain and look bad
            editor.tileSelectSquare.Clear();

            if (focusedOnTemplate) {
                editor.ClearSelection();
                //Save camera and set new
                previousCameraPosNormal = Camera.main.transform.position;
                Camera.main.GetComponent<EditorCam>().pos = previousCameraPosTemplate;

                SetTilemapBounds(0, 0, 16, templateMaxY);
            }
            else {
                //Set camera back
                previousCameraPosTemplate = Camera.main.transform.position;
                Camera.main.GetComponent<EditorCam>().pos = previousCameraPosNormal;

                SetTilemapBounds(LevelEditorData.MinX, LevelEditorData.MinY, LevelEditorData.MaxX, LevelEditorData.MaxY);
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

            var layer = LevelEditorData.Level.Layers[LevelEditorData.CurrentLayer] as Unity_Layer_Map;
            if (layer == null) return null;
            var map = layer.Map;

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

        public void SetTileBlockAtPos(int layerIndex, int x, int y, int w, int h, Unity_Tile[] newTiles) {
            for (int y1 = y; y1 < y1 + h; y1++) {
                for (int x1 = x; x1 < x1 + w; x1++) {
                    SetTileAtPos(layerIndex, x1, y1, newTiles[y1 * w + x1], applyTexture: false);
                }
            }
            var l = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
            if (l?.Graphics != null) l.Graphics.sprite.texture.Apply();
        }
        public void SetTileBlockAtPos(int layerIndex, int startX, int startY, int w, int h, Unity_Tile[,] newTiles) {
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    SetTileAtPos(layerIndex, startX + x, startY + y, newTiles[x,y], applyTexture: false);
                }
            }
            var l = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
            if (l?.Graphics == null) return;
            Texture2D tex = l.Graphics.sprite.texture;
            tex.Apply();
            //GraphicsTilemaps[i].sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0, 0), LevelEditorData.EditorManager.PixelsPerUnit, 0, SpriteMeshType.FullRect);
        }

        private void FillInTilePixels(Texture2D tex, Unity_Tile t, Unity_Map map, int layerIndex, int x, int y, int cellSize, bool applyTexture = false, bool combined = false) {
            bool flipY = (t?.Data.VerticalFlip ?? false);
            bool flipX = (t?.Data.HorizontalFlip ?? false);
            int?[,][] TileIndexOverrides = null;
            if (layerIndex >= 0) {
                var layer = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
                TileIndexOverrides = layer.TileIndexOverrides;
            }
            var tile = map.GetTile(t, LevelEditorData.CurrentSettings, tileIndexOverride: TileIndexOverrides != null ? TileIndexOverrides[x,y][0] : null);
            FillInTilePixels_Single(tex, tile, x, y, flipX, flipY, cellSize, applyTexture: false, overlay: false);

            // Write combined tiles
            if (t?.CombinedTiles?.Length > 0) {
                for(int i = 0; i < t.CombinedTiles.Length; i++) {
                    var ct = t.CombinedTiles[i];
                    var tileTex = map.GetTile(ct, LevelEditorData.CurrentSettings, tileIndexOverride: TileIndexOverrides != null ? TileIndexOverrides[x, y][1 + i] : null);
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

        public void SetCollisionTileRendererAtPos(int x, int y, int layerIndex, Unity_Tile newTile, bool setNull = true) {
            var lm = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
            if (lm?.CollisionTilemap == null) return;
            if(setNull) lm.CollisionTilemap.SetTile(new Vector3Int(x, y, 0), null);
            var type = LevelEditorData.Level.GetCollisionTypeGraphicFunc(newTile.Data.CollisionType);
            if (CurrentCollisionIcons.ContainsKey(type)) {
                lm.CollisionTilemap.SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons[type]);
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

        public void SetTileAtPos(int layerIndex, int x, int y, Unity_Tile newTile, bool applyTexture = true) {
            var layer = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
            if (layer == null) return;
            var map = layer.Map;
            if (y < 0 || y >= map.Height) return;
            if (x < 0 || x >= map.Width) return;

            // Update tile graphics
            if(layer.CollisionTilemap != null) SetCollisionTileRendererAtPos(x, y, layerIndex, newTile);
            if (layer.Graphics != null) {
                Texture2D tex = layer.Graphics.sprite.texture;

                int cellSize = LevelEditorData.Level.CellSize;
                FillInTilePixels(tex, newTile, map, layerIndex, x, y, cellSize, applyTexture: applyTexture);
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

        public Unity_Tile SetTypeAtPos(int layerIndex, int x, int y, ushort collisionType) {
            var layer = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
            if (layer == null) return null;
            var map = layer.Map;

            // Update tile graphics
            if (layer.CollisionTilemap != null) {
                layer.CollisionTilemap.SetTile(new Vector3Int(x, y, 0), null);
                var type = LevelEditorData.Level.GetCollisionTypeGraphicFunc(collisionType);
                if (CurrentCollisionIcons.ContainsKey(type)) {
                    layer.CollisionTilemap.SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons[type]);
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

        public void UpdateLayer_Map_Texture(int i) {
            var layer = LevelEditorData.Level.Layers[i];
            var layer_map = layer as Unity_Layer_Map;
            var layer_texture = layer as Unity_Layer_Texture;
            SpriteRenderer graphics = layer_map?.Graphics ?? layer_texture?.Graphics;
            var collision = layer_map?.Collision;
            var map = layer_map?.Map;
            bool enforcedVisibility = false;

            if (focusedOnTemplate) {
                enforcedVisibility = true;
                layer.SetVisible(false);
            } else if (LevelEditorData.Level.IsometricData != null) {
                bool is3D = Controller.obj?.levelController?.editor?.cam?.FreeCameraMode ?? false;
                var settings3D = map?.Settings3D ?? layer_texture?.Settings3D;
                if (!layer.ShowIn3DView) {
                    if (is3D) {
                        enforcedVisibility = true;
                        layer.SetVisible(false);
                    }
                } else if (settings3D != null) {
                    Vector3 pos;
                    Vector3 posCol;
                    Quaternion q;
                    Vector3 scale;
                    int sortingOrder = 0;
                    int sortingOrderCol = 0;
                    if (!is3D) {
                        // Move map back into original position
                        pos = new Vector3(0, 0, layerStartZ - i);
                        posCol = pos;
                        q = Quaternion.identity;
                        scale = new Vector3(1,-1,1);
                        if (graphics != null) ConfigureGraphicsMapLayer(i);
                        if (layer_map?.Collision != null) ConfigureCollisionMapLayer(i);
                    } else {
                        // Move map to desired position
                        if (settings3D.Mode == Unity_Map.FreeCameraSettings.Mode3D.FixedPosition) {
                            pos = settings3D.Position ?? Vector3.zero;
                            q = settings3D.Rotation ?? Quaternion.identity;
                            posCol = settings3D.PositionCollision ?? pos;
                            scale = settings3D.Scale ?? Vector3.one;
                            sortingOrder = settings3D.SortingOrderGraphics ?? 0;
                            sortingOrderCol = settings3D.SortingOrderCollision ?? 0;
                        } else if (settings3D.Mode == Unity_Map.FreeCameraSettings.Mode3D.Billboard) {
                            pos = settings3D.Position ?? Vector3.zero;
                            posCol = settings3D.PositionCollision ?? pos;
                            q = settings3D.Rotation ?? Quaternion.identity;
                            scale = settings3D.Scale ?? Vector3.one;
                            sortingOrder = settings3D.SortingOrderGraphics ?? 0;
                            sortingOrderCol = settings3D.SortingOrderCollision ?? 0;

                            Camera cam = Controller.obj.levelEventController.editor.cam.camera3D;
                            Quaternion lookRot = Quaternion.LookRotation(
                                cam.transform.rotation * Vector3.forward,
                                cam.transform.rotation * Vector3.up);
                            q = lookRot * q;
                            pos = cam.transform.TransformPoint(pos);
                            posCol = cam.transform.TransformPoint(posCol);
                        } else {
                            pos = new Vector3(0, 0, layerStartZ - i);
                            posCol = pos;
                            q = Quaternion.identity;
                            scale = Vector3.one;
                        }
                        if (settings3D.Mode == Unity_Map.FreeCameraSettings.Mode3D.FixedPosition
                            || settings3D.Mode == Unity_Map.FreeCameraSettings.Mode3D.Billboard) {
                            // Update sorting info
                            if (graphics != null) {
                                graphics.sortingLayerName = "Object Sprites";
                                graphics.sortingOrder = sortingOrder;
                            }
                            if (layer_map?.Collision != null) {
                                layer_map.Collision.sortingLayerName = "Object Sprites";
                                layer_map.Collision.sortingOrder = sortingOrderCol;
                            }
                        }
                    }
                    if (graphics != null) {
                        graphics.transform.localPosition = pos;
                        graphics.transform.localRotation = q;
                        graphics.transform.localScale = scale;
                    }
                    if (layer_map?.Collision != null) {
                        layer_map.Collision.transform.localPosition = posCol;
                        layer_map.Collision.transform.localRotation = q;
                        layer_map.Collision.transform.localScale = scale;
                    }
                }
            }
            // Change map visibility
            if (!enforcedVisibility) {
                if (IsLayerVisible != null && IsLayerVisible.Length > i) {
                    layer.SetVisible(IsLayerVisible[i]);
                }
            }
        }

		private void Update()
        {
            if (Controller.LoadState != Controller.State.Finished) return;

            // Enforce layer visibility
            UpdateLayersVisibility();

            bool showGridMap = Settings.ShowGridMap;
            if (LevelEditorData.Level.IsometricData != null) {
                bool is3D = Controller.obj?.levelController?.editor?.cam?.FreeCameraMode ?? false;
                showGridMap = showGridMap && !is3D;
            }

            if (LevelEditorData.Level.GridMap != null) {
                if (showGridMap != tilemapGrid.gameObject.activeSelf) {
                    tilemapGrid.gameObject.SetActive(showGridMap);
                }
            } else {
                if (tilemapGrid.gameObject.activeSelf) {
                    tilemapGrid.gameObject.SetActive(false);
                }
            }

            CheckPaletteChange();
            if (!IsAnimated || !Settings.AnimateTiles) return;

            for(int i = 0; i < LevelEditorData.Level.Layers.Length; i++) {
                var layer = LevelEditorData.Level.Layers[i];
                switch (layer) {
                    case Unity_Layer_Map lm:
                        Animate_Layer_Map(i);
                        break;
                    case Unity_Layer_Texture lt:
                        Animate_Layer_Texture(i);
                        break;
                }
            }
        }

        public void Animate_Layer_Map(int layerIndex) {
            var layer = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Map;
            if (layer?.Graphics == null || !layer.IsAnimated) return;
            bool changedTile = false;
            changedTiles.Clear();
            var map = layer.Map;
            Texture2D tex = layer.Graphics.sprite.texture;
            foreach (var animatedTile in layer.AnimatedTiles.Keys) {
                foreach (var at in layer.AnimatedTiles[animatedTile]) {
                    var animSpeed = (animatedTile.AnimationSpeeds?[at.tileIndex] ?? animatedTile.AnimationSpeed) / LevelEditorData.FramesPerSecond;
                    //print("Updating " + at.x + " - " + at.y);
                    at.currentTimer += Time.deltaTime;

                    if (!(at.currentTimer >= animSpeed))
                        continue;

                    int frames = Mathf.FloorToInt(at.currentTimer / animSpeed);
                    int oldTileIndex = at.tileIndex;
                    at.tileIndex = (oldTileIndex + frames) % at.animatedTile.TileIndices.Length;

                    if (animatedTile.IgnoreFirstTile && at.tileIndex == 0 && at.animatedTile.TileIndices.Length > 1)
                        at.tileIndex++;

                    int oldIndexInTileset = at.animatedTile.TileIndices[oldTileIndex];
                    int newIndexInTileset = at.animatedTile.TileIndices[at.tileIndex];
                    if (oldIndexInTileset != newIndexInTileset) {
                        changedTile = true;
                        Vector2Int v = new Vector2Int(at.x, at.y);
                        if (!changedTiles.Contains(v)) changedTiles.Add(v);
                        layer.TileIndexOverrides[at.x, at.y][at.combinedTileIndex.HasValue ? at.combinedTileIndex.Value + 1 : 0] = newIndexInTileset;
                    }
                    at.currentTimer -= frames * animSpeed;
                }
            }
            int cellSize = LevelEditorData.Level.CellSize;
            foreach (var ct in changedTiles) {
                var newTile = map.MapTiles[ct.y * map.Width + ct.x];
                FillInTilePixels(tex, newTile, map, layerIndex, ct.x, ct.y, cellSize, applyTexture: false);
            }
            if (changedTile)
                tex.Apply();
        }

        public void Animate_Layer_Texture(int layerIndex) {
            var lt = LevelEditorData.Level.Layers[layerIndex] as Unity_Layer_Texture;
            if (lt?.Graphics == null || !lt.IsAnimated) return;

            int curTex = Mathf.FloorToInt(lt.CurrentAnimatedTexture);
            if (lt.AnimSpeed != 0) {
                lt.CurrentAnimatedTexture += Time.deltaTime * (LevelEditorData.FramesPerSecond / lt.AnimSpeed);
            }
            if (lt.CurrentAnimatedTexture >= lt.Sprites.Length) lt.CurrentAnimatedTexture = 0;
            int newTex = Mathf.FloorToInt(lt.CurrentAnimatedTexture);
            if (newTex != curTex) {

                bool wasTiled = lt.Graphics.drawMode == SpriteDrawMode.Tiled;
                if (wasTiled) SetGraphicsLayerTiled(layerIndex, false);
                lt.Graphics.sprite = lt.Sprites[newTex];
                if (wasTiled) SetGraphicsLayerTiled(layerIndex, true);
            }
        }

        public void UpdateLayersVisibility()
        {
            if (IsLayerVisible != null)
            {
                for (int i = 0; i < IsLayerVisible.Length; i++)
                {
                    UpdateLayer_Map_Texture(i);
                    /*if (GraphicsTilemaps != null && GraphicsTilemaps[i] != null)
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
                    }*/
                }
            }
        }
    }
}