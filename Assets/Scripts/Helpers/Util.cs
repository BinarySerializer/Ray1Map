using Cysharp.Threading.Tasks;
using ImageMagick;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer;
using BinarySerializer.GBA;
using UnityEngine;

namespace R1Engine
{
    public static class Util {
        public static bool ByteArrayToFile(string fileName, byte[] byteArray) {
			if (byteArray == null) return false;
            if (FileSystem.mode == FileSystem.Mode.Web) return false;
            try {
				Directory.CreateDirectory(new System.IO.FileInfo(fileName).Directory.FullName);
                using (var fs = new FileStream(fileName, System.IO.FileMode.Create, FileAccess.Write)) {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            } catch (Exception ex) {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

		private static readonly string[] SizeSuffixes =
				  { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		public static string SizeSuffix(Int64 value, int decimalPlaces = 1) {
			if (value < 0) { return "-" + SizeSuffix(-value); }

			int i = 0;
			decimal dValue = value;
			while (Math.Round(dValue, decimalPlaces) >= 1000) {
				dValue /= 1024;
				i++;
			}

			return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
		}
        public static BaseColor[] CreateDummyPalette(int length, bool firstTransparent = true, int? wrap = null) {
            BaseColor[] pal = new BaseColor[length];
            if(wrap == null) wrap = length;
            if (firstTransparent) {
                pal[0] = BaseColor.Clear;
            }
            for (int i = firstTransparent ? 1 : 0; i < length; i++) {
                float val = (float)(i % wrap.Value) / (wrap.Value - 1);
                float bv = val;
                pal[i] = new CustomColor(bv,bv,bv);
            }
            return pal;
        }

        public static uint NextPowerOfTwo(uint v) {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

		public static string NormalizePath(string path, bool isFolder) {
			string newPath = path.Replace("\\", "/");
			if (isFolder && !newPath.EndsWith("/")) newPath += "/";
			return newPath;
		}

		// For debugging
		public static void ExportPointerArray(SerializerObject s, string path, IEnumerable<Pointer> pointers)
        {
            var p1 = pointers.Where(x => x != null).Distinct().OrderBy(x => x.AbsoluteOffset).ToArray();
            var output = new List<string>();

            for (int i = 0; i < p1.Length - 1; i++)
            {
                var length = p1[i + 1] - p1[i];

                s.DoAt(p1[i], () => output.Add($"{p1[i]}: byte[{length:000}] {String.Join(" ", s.SerializeArray<byte>(null, length).Select(x => x.ToString("X2")))}"));
            }

            File.WriteAllLines(path, output);
        }

        public static void CopyDir(string inputDir, string outputDir)
        {
            foreach (string dirPath in Directory.GetDirectories(inputDir, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(inputDir, outputDir));

            foreach (string newPath in Directory.GetFiles(inputDir, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(inputDir, outputDir), true);
        }

        public static void RenameFilesToUpper(string inputDir)
        {
            foreach (var file in Directory.GetFiles(inputDir, "*", SearchOption.AllDirectories))
            {
                var dir = Path.GetDirectoryName(file);
                var fileName = Path.GetFileName(file);

                // Move to temp name
                var tempPath = Path.Combine(dir, $"TEMP_{fileName}");
                File.Move(file, tempPath);

                // Move to upper-case name
                File.Move(tempPath, Path.Combine(dir, fileName.ToUpper()));
            }
        }

        public static async UniTask EnumerateLevelsAsync(Func<GameSettings, UniTask> action)
        {
            var manager = Settings.GetGameManager;
            var settings = Settings.GetGameSettings;

            foreach (var vol in manager.GetLevels(settings))
            {
                settings.EduVolume = vol.Name;

                foreach (var world in vol.Worlds)
                {
                    settings.World = world.Index;

                    foreach (var map in world.Maps)
                    {
                        settings.Level = map;
                        await action(settings);
                    }
                }
            }
        }

        public static void FindMatchingEncoding(params KeyValuePair<string, byte[]>[] input)
        {
            if (input.Length < 2)
                throw new Exception("Too few strings to check!");

            // Get all possible encodings
            var encodings = Encoding.GetEncodings().Select(x => Encoding.GetEncoding(x.CodePage)).ToArray();

            // Keep a list of all matching ones
            var matches = new List<Encoding>();

            // Helper method for getting all matching encodings
            IEnumerable<Encoding> GetMatches(KeyValuePair<string, byte[]> str)
            {
                var m = encodings.Where(enc => enc.GetString(str.Value).Equals(str.Key, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                Debug.Log($"Matching encodings for {str.Key}: {String.Join(", ", m.Select(x => $"{x.EncodingName} ({x.CodePage})"))}");
                return m;
            }

            // Add matches for the first one
            matches.AddRange(GetMatches(input.First()));

            // Check remaining ones, removing any which don't match
            foreach (var str in input.Skip(1))
            {
                var ma = GetMatches(str);
                matches.RemoveAll(x => !ma.Contains(x));
            }

            // Log the result
            Debug.Log($"Matching encodings for all: {String.Join(", ", matches.Select(x => $"{x.EncodingName} ({x.CodePage})"))}");
        }

        public static void ExportWAVChunks(Context context, WAVChunk[] chunks, string outputDir)
        {
            var index = 0;

            foreach (var chunk in chunks)
            {
                if (chunk.ChunkHeader == "LIST")
                {
                    var list = chunk.SerializeTo<WAVListChunk>(context);
                    ExportWAVChunks(context, list.Chunks, Path.Combine(outputDir, $"{index}_{list.ListHeader}"));
                }
                else
                {
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{index}_{chunk.ChunkHeader}"), chunk.Data);
                }

                index++;
            }
        }

        public static void ExportAllCompressedData(Context context, Pointer offset, IStreamEncoder encoder, byte[] header, string outputDir, int alignment = 4, int minDecompSize = 33)
        {
            var s = context.Deserializer;

            s.Goto(offset);

            // Keep track of blocks
            var blocks = new List<Tuple<long, long, int>>();

            // Enumerate every byte
            for (int i = 0; i < s.CurrentLength; i += alignment)
            {
                // Go to the offset
                s.Goto(offset + i);

                // Check for compression header
                if (s.SerializeArray<byte>(default, header.Length).SequenceEqual(header))
                {
                    if (encoder is GBA_LZSSEncoder)
                    {
                        // Get the decompressed size
                        var decompressedSizeValue = s.SerializeArray<byte>(default, 3);
                        Array.Resize(ref decompressedSizeValue, 4);
                        var decompressedSize = BitConverter.ToUInt32(decompressedSizeValue, 0);

                        // Skip if the decompressed size is too low
                        if (decompressedSize < minDecompSize)
                            continue;
                    }

                    // Go back to the offset
                    s.Goto(offset + i);

                    // Attempt to decompress
                    try
                    {
                        byte[] data = null;

                        s.DoEncoded(encoder, () => data = s.SerializeArray<byte>(default, s.CurrentLength));

                        // Make sure we got some data
                        if (data != null && data.Length >= minDecompSize)
                        {
                            ByteArrayToFile(Path.Combine(outputDir, $"Block_0x{(offset + i).StringAbsoluteOffset}.dat"), data);

                            blocks.Add(new Tuple<long, long, int>((offset + i).AbsoluteOffset, s.CurrentPointer - (offset + i), data.Length));
                        }
                    }
                    catch
                    {
                        // Ignore exceptions...
                    }
                }
            }

            var log = new List<string>();

            for (int i = 0; i < blocks.Count; i++)
            {
                var (off, compressedSize, size) = blocks[i];

                var end = off + compressedSize;

                log.Add($"0x{off:X8} - 0x{end:X8} (0x{compressedSize:X8} - 0x{size:X8}) - ");

                if (i != blocks.Count - 1)
                {
                    var dif = blocks[i + 1].Item1 - end;

                    if (dif >= 4)
                        log.Add($"0x{end:X8} - 0x{end + dif:X8} (0x{dif:X8})              - ");
                }
            }

            File.WriteAllLines(Path.Combine(outputDir, "blocks_log.txt"), log);
        }

        public static Texture2D ToTileSetTexture(byte[] imgData, Color[] pal, TileEncoding encoding, int tileWidth, bool flipY, int wrap = 32, Func<int, Color[]> getPalFunc = null, bool flipTileX = false, bool flipTileY = false, bool flipX = false)
        {
            int bpp;

            switch (encoding)
            {
                case TileEncoding.Planar_2bpp:
                    bpp = 2; break;
                case TileEncoding.Planar_4bpp:
                case TileEncoding.Linear_4bpp:
                case TileEncoding.Linear_4bpp_ReverseOrder:
                    bpp = 4; break;
                case TileEncoding.Linear_8bpp: 
                    bpp = 8; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null);
            }

            int tileSize = tileWidth * tileWidth * bpp / 8;
            int tilesetLength = imgData.Length / tileSize;

            int tilesX = Math.Min(tilesetLength, wrap);
            int tilesY = Mathf.CeilToInt(tilesetLength / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth);

            for (int i = 0; i < tilesetLength; i++)
            {
                int tileY = ((i / wrap)) * tileWidth;
                int tileX = (i % wrap) * tileWidth;

                tex.FillInTile(
                    imgData: imgData, 
                    imgDataOffset: i * tileSize, 
                    pal: getPalFunc?.Invoke(i) ?? pal, 
                    encoding: encoding, 
                    tileWidth: tileWidth, 
                    flipTextureY: flipY, 
                    flipTextureX: flipX, 
                    tileX: tileX, 
                    tileY: tileY, 
                    flipTileX: flipTileX, 
                    flipTileY: flipTileY);
            }

            tex.Apply();

            return tex;
        }

        public static void FillInTile(this Texture2D tex, byte[] imgData, int imgDataOffset, Color[] pal, TileEncoding encoding, int tileWidth, bool flipTextureY, int tileX, int tileY, bool flipTileX = false, bool flipTileY = false, bool ignoreTransparent = false, bool flipTextureX = false)
        {
            FillRegion(tex, imgData, imgDataOffset, pal, encoding,
                tileX, tileY, tileWidth, tileWidth,
                flipTextureX: flipTextureX, flipTextureY: flipTextureY,
                flipRegionX: flipTileX, flipRegionY: flipTileY,
                ignoreTransparent: ignoreTransparent);
        }

        public static void FillRegion(this Texture2D tex, byte[] imgData, int imgDataOffset,
            Color[] pal, TileEncoding encoding,
            int regionX, int regionY,
            int regionWidth, int regionHeight,
            bool flipTextureX = false, bool flipTextureY = false,            
            bool flipRegionX = false, bool flipRegionY = false,
            bool ignoreTransparent = false, Func<int,int,int,Color?> paletteFunction = null) {
            bool reverseOrder = (encoding == TileEncoding.Linear_4bpp_ReverseOrder);

            // Fill in tile pixels
            for (int y = 0; y < regionHeight; y++) {

                var yy = regionY + y;

                if (flipTextureY)
                    yy = tex.height - yy - 1;
                if (yy < 0 || yy >= tex.height) continue;

                for (int x = 0; x < regionWidth; x++) {
                    var xx = regionX + x;

                    if (flipTextureX)
                        xx = tex.width - xx - 1;

                    if (xx < 0 || xx >= tex.width) continue;
                    Color? c = null;

                    if (encoding == TileEncoding.Linear_8bpp) {
                        int index = imgDataOffset + (((flipRegionY ? (regionHeight - y - 1) : y) * regionWidth + (flipRegionX ? (regionWidth - x - 1) : x)));

                        var b = imgData[index];

                        c = paletteFunction != null ? paletteFunction.Invoke(b, xx, yy) : pal[b];
                    } else if (encoding == TileEncoding.Linear_4bpp || encoding == TileEncoding.Linear_4bpp_ReverseOrder) {
                        int index = imgDataOffset + (((flipRegionY ? (regionHeight - y - 1) : y) * regionWidth + (flipRegionX ? (regionWidth - x - 1) : x)) / 2);

                        var b = imgData[index];
                        var v = (flipRegionX ^ reverseOrder) ?
                            BitHelpers.ExtractBits(b, 4, x % 2 == 1 ? 0 : 4) :
                            BitHelpers.ExtractBits(b, 4, x % 2 == 0 ? 0 : 4);

                        c = paletteFunction != null ? paletteFunction.Invoke(v, xx, yy) : pal[v];
                    } else if (encoding == TileEncoding.Planar_4bpp) {
                        int off = (flipRegionY ? (regionHeight - y - 1) : y) * regionWidth + (flipRegionX ? (regionWidth - x - 1) : x);

                        var offset1 = imgDataOffset;
                        var offset2 = imgDataOffset + 16;

                        var bit0 = BitHelpers.ExtractBits(imgData[offset1 + ((off / 8) * 2)], 1, off % 8);
                        var bit1 = BitHelpers.ExtractBits(imgData[offset1 + ((off / 8) * 2 + 1)], 1, off % 8);
                        var bit2 = BitHelpers.ExtractBits(imgData[offset2 + ((off / 8) * 2)], 1, off % 8);
                        var bit3 = BitHelpers.ExtractBits(imgData[offset2 + ((off / 8) * 2 + 1)], 1, off % 8);

                        int b = 0;

                        b = BitHelpers.SetBits(b, bit0, 1, 0);
                        b = BitHelpers.SetBits(b, bit1, 1, 1);
                        b = BitHelpers.SetBits(b, bit2, 1, 2);
                        b = BitHelpers.SetBits(b, bit3, 1, 3);

                        c = paletteFunction != null ? paletteFunction.Invoke(b, xx, yy) : pal[b];
                    } else if (encoding == TileEncoding.Planar_2bpp) {
                        int index = imgDataOffset + (((flipRegionY ? (regionHeight - y - 1) : y) * regionWidth + (flipRegionX ? (regionWidth - x - 1) : x)) / 8) * 2;
                        var b0 = imgData[index];
                        var b1 = imgData[index + 1];
                        int actualX = flipRegionX ? x : 7 - x;
                        var v = (BitHelpers.ExtractBits(b1, 1, actualX) << 1) | BitHelpers.ExtractBits(b0, 1, actualX);

                        c = paletteFunction != null ? paletteFunction.Invoke(v, xx, yy) : pal[v];
                    } else if (encoding == TileEncoding.Linear_32bpp_RGBA) {
                        int index = imgDataOffset + (((flipRegionY ? (regionHeight - y - 1) : y) * regionWidth + (flipRegionX ? (regionWidth - x - 1) : x))) * 4;
                        var r = imgData[index + 0];
                        var g = imgData[index + 1];
                        var b = imgData[index + 2];
                        var a = imgData[index + 3];
                        c = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
                    } else {
                        c = Color.clear;
                    }

                    if (c.HasValue) {
                        if (!ignoreTransparent || c.Value.a > 0) {
                            tex.SetPixel(xx, yy, c.Value);
                        }
                    }
                }
            }
        }


        public static Texture2D ToTileSetTexture(BaseColor[] imgData, int tileWidth, bool flipY, int wrap = 32) {
            int tileSize = tileWidth * tileWidth;
            int tilesetLength = imgData.Length / tileSize;

            int tilesX = Math.Min(tilesetLength, wrap);
            int tilesY = Mathf.CeilToInt(tilesetLength / (float)wrap);

            var tex = TextureHelpers.CreateTexture2D(tilesX * tileWidth, tilesY * tileWidth);

            for (int i = 0; i < tilesetLength; i++) {
                int tileY = ((i / wrap)) * tileWidth;
                int tileX = (i % wrap) * tileWidth;

                tex.FillRegion(imgData, i * tileSize, tileX, tileY, tileWidth, tileWidth, flipY);
            }

            tex.Apply();

            return tex;
        }

        public static void FillRegion(this Texture2D tex,
            BaseColor[] imgData, int imgDataOffset,
            int regionX, int regionY,
            int regionWidth, int regionHeight,
            bool flipTextureX = false, bool flipTextureY = false,
            bool flipRegionX = false, bool flipRegionY = false,
            bool ignoreTransparent = false) {
            // Fill in tile pixels
            for (int y = 0; y < regionHeight; y++) {

                var yy = regionY + y;

                if (flipTextureY)
                    yy = tex.height - yy - 1;
                if (yy < 0 || yy >= tex.height) continue;

                for (int x = 0; x < regionWidth; x++) {
                    var xx = regionX + x;
                    
                    if (flipTextureX)
                        xx = tex.width - xx - 1;
                    if (xx < 0 || xx >= tex.width) continue;
                    Color c;

                    int index = imgDataOffset
                        + (((flipRegionY ? (regionHeight - y - 1) : y) * regionWidth
                        + (flipRegionX ? (regionWidth - x - 1) : x)));
                    c = imgData[index].GetColor();

                    if (!ignoreTransparent || c.a > 0) {
                        tex.SetPixel(xx, yy, c);
                    }
                }
            }
        }

        public static Texture2D GetGridTex(int cellSize)
        {
            var tex = TextureHelpers.CreateTexture2D(cellSize, cellSize);

            for (int y = 0; y < cellSize; y++)
            {
                for (int x = 0; x < cellSize; x++)
                {
                    if (y == cellSize - 1 || x == cellSize - 1)
                        tex.SetPixel(x, y, new Color(1,1,1,0.25f));
                    else
                        tex.SetPixel(x, y, Color.clear);
                }
            }

            tex.Apply();

            return tex;
        }


        public static int GCF(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        public static int LCM(int a, int b)
        {
            return (a / GCF(a, b)) * b;
        }

        public static int LCM(IList<int> numbers, int i = 0)
        {
            if (i + 2 == numbers.Count)
                return LCM(numbers[i], numbers[i + 1]);
            else
                return LCM(numbers[i], LCM(numbers, i + 1));
        }

        public static Color[] ConvertGBAPalette(IEnumerable<BaseColor> palette, int? transparentIndex = 0) => palette.Select((x, i) => {
            Color c = x.GetColor();
            if (!transparentIndex.HasValue || i != transparentIndex.Value) {
                c.a = 1f;
            } else {
                c.a = 0f;
            }
            return c;
        }).ToArray();
        public static Color[][] ConvertAndSplitGBAPalette(BaseColor[] palette, bool firstTransparent = true)
            => palette
            .Split(palette.Length / 16, 16)
            .Select(p => ConvertGBAPalette(p, transparentIndex: firstTransparent ? (int?)0 : null))
            .ToArray();
        public static Color[][] ConvertAndSplitGBCPalette(RGBA5551Color[] palette, int? transparentIndex = 0)
            => palette
            .Split(palette.Length / 4, 4)
            .Select(p => ConvertGBAPalette(p, transparentIndex: transparentIndex))
            .ToArray();


        public static IEnumerable<T[]> Split<T>(this T[] array, int length, int size) => Enumerable.Range(0, length).Select(x => array.Skip(size * x).Take(size).ToArray());

        public static void GetGBASize(byte shape, byte size, out int width, out int height)
        {
            width = 1;
            height = 1;

            switch (shape)
            {
                case 0: // Square
                    width = 1 << size;
                    height = width;
                    break;

                case 1: // Wide
                    switch (size)
                    {
                        case 0: width = 2; height = 1; break;
                        case 1: width = 4; height = 1; break;
                        case 2: width = 4; height = 2; break;
                        case 3: width = 8; height = 4; break;
                    }
                    break;

                case 2: // Tall
                    switch (size)
                    {
                        case 0: width = 1; height = 2; break;
                        case 1: width = 1; height = 4; break;
                        case 2: width = 2; height = 4; break;
                        case 3: width = 4; height = 8; break;
                    }
                    break;
            }
        }

        public enum TileEncoding
        {
            Planar_2bpp,
            Planar_4bpp,
            Linear_4bpp,
            Linear_4bpp_ReverseOrder,
            Linear_8bpp,
            Linear_32bpp_RGBA,
        }

        public static void ExportAnim(IList<Texture2D> frames, int speed, bool center, bool saveAsGif, string outputDir, string primaryName, string secondaryName)
        {
            if (saveAsGif)
            {
                ExportAnimAsGif(frames, speed, center, true, Path.Combine(outputDir, $"{primaryName} - {secondaryName}.gif"));
            }
            else
            {
                var frameIndex = 0;

                foreach (var tex in frames)
                {
                    Util.ByteArrayToFile(Path.Combine(outputDir, $"{primaryName}", $"{secondaryName}", $"{frameIndex}.png"), tex.EncodeToPNG());
                    frameIndex++;
                }
            }
        }
        public static void ExportAnimAsGif(IList<Texture2D> frames, int speed, bool center, bool trim, string filePath)
        {
            using (var collection = new MagickImageCollection())
            {
                int index = 0;

                var maxWidth = frames.Max(x => x.width);
                var maxHeight = frames.Max(x => x.height);

                foreach (var frameTex in frames)
                {
                    var img = frameTex.ToMagickImage();
                    collection.Add(img);
                    collection[index].AnimationDelay = speed;
                    collection[index].AnimationTicksPerSecond = 60;

                    if (center)
                        collection[index].Extent(maxWidth, maxHeight, Gravity.Center, new MagickColor());

                    if (trim)
                        collection[index].Trim();
                    collection[index].GifDisposeMethod = GifDisposeMethod.Background;
                    index++;
                }

                // Save gif
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                collection.Write(filePath);
            }
        }
    }
}