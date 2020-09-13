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

        public SpriteRenderer tilemapFull;
        public SpriteRenderer tilemapPreview;
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

        // Infro tracked for when switching between template and normal level
        private Vector3 previousCameraPosNormal;
        private Vector3 previousCameraPosTemplate = new Vector3(0, 0, -10f);
        private int templateMaxY = 0;

        public int camMaxX = 1;
        public int camMaxY = 1;

        public float CellSizeInUnits { get; set; } = 1f;

        private Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>[] animatedTiles;

        public bool Has3Palettes => LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PC || LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PocketPC || LevelEditorData.CurrentSettings.EngineVersion == EngineVersion.R1_PC_Kit;

        public void InitializeTilemaps() {
            var level = LevelEditorData.Level;

            CellSizeInUnits = level.CellSize / (float)level.PixelsPerUnit;
            if (CellSizeInUnits != 1f) {
                grid.cellSize = new Vector3(CellSizeInUnits, CellSizeInUnits, 0);
            }

            if (level.Background != null) {
                background.sprite = level.Background.CreateSprite();
                background.gameObject.SetActive(true);
            }
            if (level.ParallaxBackground != null) {
                backgroundParallax.sprite = level.ParallaxBackground.CreateSprite();
                backgroundParallax.gameObject.SetActive(true);
            }

            // Resize the background tint
            ResizeBackgroundTint(LevelEditorData.MaxWidth, LevelEditorData.MaxHeight);

            // Disable palette buttons based on if there are 3 palettes or not
            if (!Has3Palettes) {
                paletteText.SetActive(false);
                paletteButtons[0].gameObject.SetActive(false);
                paletteButtons[1].gameObject.SetActive(false);
                paletteButtons[2].gameObject.SetActive(false);
                paletteButtons[3].gameObject.SetActive(false);
            }

            // Get the current collision map
            var collisionMap = level.Maps[LevelEditorData.CurrentCollisionMap];

            // Set collision tiles
            var collisionTileSet = Settings.UseHDCollisionSheet ? CollisionTilesHD : CollisionTiles;
            if (CellSizeInUnits != 1f) {
                collisionTileSet = collisionTileSet
                    .Select(t => {
                        if (t == null) return null;
                        Tile newT = ScriptableObject.CreateInstance<Tile>();
                        newT.sprite = Sprite.Create(t.sprite.texture, t.sprite.rect, new Vector2(0.5f, 0.5f), t.sprite.pixelsPerUnit / CellSizeInUnits);
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

            var unsupportedTiles = new HashSet<int>();

            // Fill out types first
            for (int y = 0; y < collisionMap.Height; y++) {
                for (int x = 0; x < collisionMap.Width; x++) {
                    // Get the collision index
                    var collisionType = LevelEditorData.Level.GetCollisionTypeGraphicFunc(collisionMap.MapTiles[y * collisionMap.Width + x].Data.CollisionType);

                    // Make sure it's not out of bounds
                    if(collisionType != Unity_MapCollisionTypeGraphic.None &&
                        (!CurrentCollisionIcons.ContainsKey(collisionType)
                        || (CurrentCollisionIcons[collisionType].sprite?.name.Contains("Unknown") ?? false)
                        || collisionType.ToString().Contains("Unknown"))) {
                        unsupportedTiles.Add((int)collisionType);
                    }

                    // Set the collision tile
                    for (int i = 0; i < CollisionTilemaps.Length; i++) {
                        CollisionTilemaps[i].SetTile(new Vector3Int(x, y, LevelEditorData.CurrentCollisionMap), CurrentCollisionIcons.TryGetItem(collisionType));
                    }
                }
            }

            if (unsupportedTiles.Count > 0)
                Debug.LogWarning($"The following collision types are not supported: {String.Join(", ", unsupportedTiles)}");

            // Fill out tiles
            currentPalette = Has3Palettes ? 0 : 1;
            RefreshTiles(currentPalette);

            var maxWidth = LevelEditorData.MaxWidth;
            var maxHeight = LevelEditorData.MaxHeight;

            // Set max cam sizes
            camMaxX = maxWidth;
            camMaxY = maxHeight;
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
            if (GraphicsTilemaps.Length != LevelEditorData.Level.Maps.Length) {
                Array.Resize(ref GraphicsTilemaps, LevelEditorData.Level.Maps.Length);
                for (int i = 1; i < GraphicsTilemaps.Length; i++) {
                    GraphicsTilemaps[i] = Instantiate<SpriteRenderer>(GraphicsTilemaps[0], new Vector3(0, 0, -i), Quaternion.identity, GraphicsTilemaps[0].transform.parent);
                    GraphicsTilemaps[i].gameObject.name = "Tilemap Graphics " + i;
                    if (lvl.Maps[i].IsForeground)
                    {
                        Debug.Log($"{i} is in front");
                        //TilemapRenderer tr = GraphicsTilemaps[i].GetComponent<TilemapRenderer>();
                        SpriteRenderer tr = GraphicsTilemaps[i];
                        tr.sortingLayerName = "Tiles Front";
                    }
                }
            }
            animatedTiles = new Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>[GraphicsTilemaps.Length];
            for (int mapIndex = 0; mapIndex < LevelEditorData.Level.Maps.Length; mapIndex++) {
                var map = lvl.Maps[mapIndex];
                animatedTiles[mapIndex] = new Dictionary<Unity_AnimatedTile, List<Unity_AnimatedTile.Instance>>();
                if (map.Alpha.HasValue) {
                    GraphicsTilemaps[mapIndex].color = new Color(1f, 1f, 1f, map.Alpha.Value);
                }
                int cellSize = LevelEditorData.Level.CellSize;
                Texture2D tex = TextureHelpers.CreateTexture2D(map.Width * cellSize, map.Height * cellSize);

                for (int y = 0; y < map.Height; y++) {
                    for (int x = 0; x < map.Width; x++) {
                        var t = map.MapTiles[y * map.Width + x];

                        if (palette != 0)
                            t.PaletteIndex = palette;

                        if (t.PaletteIndex - 1 >= map.TileSet.Length)
                            t.PaletteIndex = 1;

                        Unity_TileTexture tile = map.GetTile(t, LevelEditorData.CurrentSettings);
                        var atInstance = map.GetAnimatedTile(t, LevelEditorData.CurrentSettings);
                        if (atInstance != null) {
                            atInstance.x = x;
                            atInstance.y = y;
                            var at = atInstance.animatedTile;
                            if (!animatedTiles[mapIndex].ContainsKey(at)) {
                                animatedTiles[mapIndex][at] = new List<Unity_AnimatedTile.Instance>();
                            }
                            animatedTiles[mapIndex][at].Add(atInstance);
                        }

                        FillInTilePixels(tex, tile, t, x, y, cellSize);

                        /*GraphicsTilemaps[mapIndex].SetTile(new Vector3Int(x, y, 0), map.GetTile(t, LevelEditorData.CurrentSettings));
                        GraphicsTilemaps[mapIndex].SetTransformMatrix(new Vector3Int(x, y, 0), GraphicsTilemaps[mapIndex].GetTransformMatrix(new Vector3Int(x, y, 0)) * Matrix4x4.Scale(new Vector3(t.Data.HorizontalFlip ? -1 : 1, t.Data.VerticalFlip ? -1 : 1, 1)));*/
                    }
                }
                tex.filterMode = FilterMode.Point;
                tex.Apply();
                // Note: FullRect is important, otherwise when editing you need to create a new sprite
                GraphicsTilemaps[mapIndex].sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), LevelEditorData.Level.PixelsPerUnit, 0, SpriteMeshType.FullRect);
            }

            CreateTilemapFull();
        }

        private void CreateTilemapFull() {
            var lvl = LevelEditorData.Level;
            int mapWidth = 16;
            int mapHeight = Mathf.CeilToInt(lvl.Maps[LevelEditorData.CurrentMap].TileSet[0].Tiles.Length / (float)mapWidth);

            int cellSize = LevelEditorData.Level.CellSize;
            Texture2D tex = TextureHelpers.CreateTexture2D(mapWidth * cellSize, mapHeight * cellSize);

            // Refresh the full tilemap template for current map
            int xx = 0;
            int yy = 0;

            foreach (Unity_TileTexture t in lvl.Maps[LevelEditorData.CurrentMap].TileSet[0].Tiles) {
                FillInTilePixels(tex, t, null, xx, yy, cellSize);
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
                GraphicsTilemaps[i].gameObject.SetActive(!focusedOnTemplate);
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
        public Vector3 MouseToTileCoords(Vector3 mousePos) {
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector3(Mathf.Floor(worldMouse.x / CellSizeInUnits) * CellSizeInUnits, (Mathf.Floor(worldMouse.y / CellSizeInUnits) + 1) * CellSizeInUnits, 10);
        }
        public Vector2Int MouseToTileInt(Vector3 mousePos) {
            var worldMouse = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10));
            return new Vector2Int(Mathf.FloorToInt(worldMouse.x / CellSizeInUnits), -(Mathf.FloorToInt(worldMouse.y / CellSizeInUnits) + 1));
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
                    Unity_TileTexture tile = map.GetTile(newTile, LevelEditorData.CurrentSettings);
                    FillInTilePixels(tex, tile, newTile, x, y, cellSize, applyTexture: false);
                }
            }
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

            return map.GetMapTile(x, y);
        }

        public void SetTileBlockAtPos(int x, int y, int w, int h, Unity_Tile[] newTiles) {
            for (int y1 = y; y1 < y1 + h; y1++) {
                for (int x1 = x; x1 < x1 + w; x1++) {
                    SetTileAtPos(x1, y1, newTiles[y1 * w + x1], applyTexture: false);
                }
            }
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                GraphicsTilemaps[i].sprite.texture.Apply();
            }
        }
        public void SetTileBlockAtPos(int startX, int startY, int w, int h, Unity_Tile[,] newTiles) {
            for (int y = 0; y < h; y++) {
                for (int x = 0; x < w; x++) {
                    SetTileAtPos(startX + x, startY + y, newTiles[x,y], applyTexture: false);
                }
            }
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                Texture2D tex = GraphicsTilemaps[i].sprite.texture;
                tex.Apply();
                //GraphicsTilemaps[i].sprite = Sprite.Create(tex, new Rect(0,0,tex.width, tex.height), new Vector2(0, 0), LevelEditorData.EditorManager.PixelsPerUnit, 0, SpriteMeshType.FullRect);
            }
        }

        private void FillInTilePixels(Texture2D tex, Unity_TileTexture tile, Unity_Tile t, int x, int y, int cellSize, bool applyTexture = false) {
            int texX = x * cellSize;
            int texY = y * cellSize;
            bool flipY = (t?.Data.VerticalFlip ?? false);
            bool flipX = (t?.Data.HorizontalFlip ?? false);
            if (tile?.texture == null) {
                tex.SetPixels(texX, texY, cellSize, cellSize, new Color[cellSize * cellSize]);
            } else {
                var tileTex = tile.texture;
                var baseX = (int)tile.rect.x;
                var baseY = (int)tile.rect.y;
                if (baseX != 0 || baseY != 0 || tileTex.width != cellSize || tileTex.height != cellSize) {
                    for (int j = 0; j < cellSize; j++) {
                        for (int k = 0; k < cellSize; k++) {
                            int tileY = flipY ? (cellSize - 1 - j) : j;
                            int tileX = flipX ? (cellSize - 1 - k) : k;
                            tex.SetPixel(texX + tileX, texY + tileY, tileTex.GetPixel(k + baseX, j + baseY));
                        }
                    }
                } else {
                    Color[] pixels = tileTex.GetPixels();
                    if (flipX || flipY) {
                        for (int j = 0; j < cellSize; j++) {
                            for (int k = 0; k < cellSize; k++) {
                                int tileY = flipY ? (cellSize - 1 - j) : j;
                                int tileX = flipX ? (cellSize - 1 - k) : k;
                                tex.SetPixel(texX + tileX, texY + tileY, pixels[j * cellSize + k]);
                            }
                        }
                    } else {
                        tex.SetPixels(texX, texY, cellSize, cellSize, pixels);
                    }
                }
            }
            if (applyTexture) tex.Apply();
        }

        public void SetTileAtPos(int x, int y, Unity_Tile newTile, bool applyTexture = true) 
        {
            var map = LevelEditorData.Level.Maps[LevelEditorData.CurrentMap];
            if (y < 0 || y >= map.Height) return;
            if (x < 0 || x >= map.Width) return;

            // Update tile graphics
            for (int i = 0; i < CollisionTilemaps.Length; i++) {
                CollisionTilemaps[i].SetTile(new Vector3Int(x, y, 0), null);
                var type = LevelEditorData.Level.GetCollisionTypeGraphicFunc(newTile.Data.CollisionType);
                if (CurrentCollisionIcons.ContainsKey(type)) {
                    CollisionTilemaps[i].SetTile(new Vector3Int(x, y, 0), CurrentCollisionIcons[type]);
                }
            }
            for (int i = 0; i < GraphicsTilemaps.Length; i++) {
                Texture2D tex = GraphicsTilemaps[i].sprite.texture;
                Unity_TileTexture tile = map.GetTile(newTile, LevelEditorData.CurrentSettings);

                int cellSize = LevelEditorData.Level.CellSize;
                FillInTilePixels(tex, tile, newTile, x, y, cellSize, applyTexture: applyTexture);
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

        public Unity_Tile SetTypeAtPos(int x, int y, byte collisionType) 
        {
            var map = LevelEditorData.Level.Maps[LevelEditorData.CurrentMap];

            // Update tile graphics
            for (int i = 0; i < CollisionTilemaps.Length; i++) {
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

            CheckPaletteChange();

            if (animatedTiles == null || !Settings.AnimateTiles) return;

            for (int mapIndex = 0; mapIndex < animatedTiles.Length; mapIndex++) {
                bool changedTile = false;
                var map = LevelEditorData.Level.Maps[mapIndex];
                Texture2D tex = GraphicsTilemaps[mapIndex].sprite.texture;
                foreach (var animatedTile in animatedTiles[mapIndex].Keys) {
                    var animSpeed = animatedTile.AnimationSpeed / 30f;
                    foreach (var at in animatedTiles[mapIndex][animatedTile]) {
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
                            var newTile = map.MapTiles[at.y * map.Width + at.x];
                            Unity_TileTexture tile = map.GetTile(newTile, LevelEditorData.CurrentSettings, tileIndexOverride: newIndexInTileset);

                            int cellSize = LevelEditorData.Level.CellSize;
                            FillInTilePixels(tex, tile, newTile, at.x, at.y, cellSize, applyTexture: false);
                        }
                        at.currentTimer -= frames * animSpeed;
                    }
                }

                if (changedTile)
                    tex.Apply();
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