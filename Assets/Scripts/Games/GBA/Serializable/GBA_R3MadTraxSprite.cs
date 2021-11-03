using BinarySerializer;
using UnityEngine;

namespace Ray1Map
{
    public class GBA_R3MadTraxSprite : BinarySerializable
    {
        // Set before serializing
        public int Width { get; set; }
        public int Height { get; set; }
        public int AssembleWidth { get; set; }
        public int AssembleHeight { get; set; }

        public RGBA5551Color[] Palette { get; set; }
        public byte[] TileData { get; set; }

        public Texture2D ToTexture2D()
        {
            const int tileWidth = 8;
            const int tileLength = tileWidth * tileWidth / 2;

            var tex = TextureHelpers.CreateTexture2D(Width * AssembleWidth * tileWidth, Height * AssembleHeight * tileWidth);

            var assembleLength = Width * Height * tileLength;

            for (int assembleY = 0; assembleY < AssembleHeight; assembleY++)
            {
                for (int assembleX = 0; assembleX < AssembleWidth; assembleX++)
                {
                    var assembleOffset = assembleLength * (assembleY * AssembleWidth + assembleX);

                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            var relTileOffset = tileLength * (y * Width + x);

                            tex.FillInTile(
                                imgData: TileData,
                                imgDataOffset: assembleOffset + relTileOffset,
                                pal: Util.ConvertGBAPalette(Palette),
                                encoding: Util.TileEncoding.Linear_4bpp,
                                tileWidth: tileWidth,
                                flipTextureY: true,
                                tileX: tileWidth * (assembleX * Width + x),
                                tileY: tileWidth * (assembleY * Height + y));
                        }
                    }
                }
            }

            tex.Apply();

            return tex;
        }

        public override void SerializeImpl(SerializerObject s)
        {
            Palette = s.SerializeObjectArray<RGBA5551Color>(Palette, 16, name: nameof(Palette));
            TileData = s.SerializeArray<byte>(TileData, Width * Height * AssembleWidth * AssembleHeight * 0x20, name: nameof(TileData));
        }
    }
}