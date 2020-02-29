using UnityEngine;
using UnityEngine.Tilemaps;

namespace R1Engine
{
    /// <summary>
    /// Defines a common tile-set
    /// </summary>
    public class Common_Tileset
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="tiles">The tiles in this set</param>
        public Common_Tileset(Tile[] tiles)
        {
            Tiles = tiles;
        }

        /// <summary>
        /// The tiles in this set
        /// </summary>
        public Tile[] Tiles { get; }

        /// <summary>
        /// Sets a tile from a texture at the specified index
        /// </summary>
        /// <param name="texture">The texture to use for the tile</param>
        /// <param name="size">The size of the tile</param>
        /// <param name="index">The index to set to</param>
        public void SetTile(Texture2D texture, int size, int index)
        {
            Tiles[index] = ScriptableObject.CreateInstance<Tile>();
            Tiles[index].sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size, 20);
        }
    }
}