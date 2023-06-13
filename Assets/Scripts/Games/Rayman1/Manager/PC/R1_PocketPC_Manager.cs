using BinarySerializer;
using BinarySerializer.Image;
using BinarySerializer.Ray1;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer.Ray1.PC;
using UnityEngine;

namespace Ray1Map.Rayman1
{
    /// <summary>
    /// The game manager for Rayman Ultimate (Pocket PC)
    /// </summary>
    public class R1_PocketPC_Manager : R1_PC_Manager
    {
        #region Values and paths

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override GameInfo_Volume[] GetLevels(GameSettings settings) => GameInfo_Volume.SingleVolume(WorldHelpers.EnumerateWorlds().Select(w => new GameInfo_World((int)w, Directory.EnumerateFiles(settings.GameDirectory + GetWorldFolderPath(w), $"{GetShortWorldName(w)}??.lev.gz", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray());

        /// <summary>
        /// Gets the folder path for the specified world
        /// </summary>
        /// <param name="world">The world</param>
        /// <returns>The world folder path</returns>
        public override string GetWorldFolderPath(World world) => GetDataPath() + GetWorldName(world).ToLower() + "/";

        /// <summary>
        /// Gets the file path for the big ray file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The big ray file path</returns>
        public override string GetBigRayFilePath(GameSettings settings) => GetDataPath() + $"bray.dat.gz";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => String.Empty;

        /// <summary>
        /// Gets the file path for the allfix file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The allfix file path</returns>
        public override string GetAllfixFilePath(GameSettings settings) => GetDataPath() + $"allfix.dat.gz";

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetWorldFolderPath(settings.R1_World) + $"{GetShortWorldName(settings.R1_World).ToLower()}{settings.Level}.lev.gz";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"ray{settings.World}.wld.gz";

        /// <summary>
        /// Gets the short name for the world
        /// </summary>
        /// <returns>The short world name</returns>
        public override string GetShortWorldName(World world)
        {
            switch (world)
            {
                case World.Jungle:
                    return "JUN";
                case World.Music:
                    return "MUS";
                case World.Mountain:
                    return "MON";
                case World.Image:
                    return "IMG";
                case World.Cave:
                    return "CAV";
                case World.Cake:
                    return "CAK";
                default:
                    throw new ArgumentOutOfRangeException(nameof(world), world, null);
            }
        }

        public string GetVignetteFilePath(int index) => GetDataPath() + $"dat/{index:00}.dat.gz";

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets a binary file to add to the context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="filePath">The file path</param>
        /// <param name="endianness">The endianness to use</param>
        /// <returns>The binary file</returns>
        protected override BinaryFile GetFile(Context context, string filePath, Endian endianness = Endian.Little) => new EncodedLinearFile(context, filePath, new GzipEncoder(), endianness);

        protected override async UniTask<KeyValuePair<string, string[]>[]> LoadLocalizationAsync(Context context)
        {
            var lngPath = GetLanguageFilePath();

            await AddFile(context, lngPath);

            // Read the language file
            var lng = Ray1TextFileFactory.ReadText<PC_LNGFile>(lngPath, context);

            // Set the common localization
            return new KeyValuePair<string, string[]>[]
            {
                new KeyValuePair<string, string[]>("English1", lng.Strings[0]),
                new KeyValuePair<string, string[]>("English2", lng.Strings[1]),
                new KeyValuePair<string, string[]>("English3", lng.Strings[2]),
                new KeyValuePair<string, string[]>("French", lng.Strings[3]),
                new KeyValuePair<string, string[]>("German", lng.Strings[4]),
            };
        }

        public override async UniTask<Texture2D> LoadBackgroundVignetteAsync(Context context, WorldFile world, LevelFile level, bool parallax)
        {
            return (await LoadPCXAsync(context, world.PcxFileIndexes[parallax ? level.ScrollDiffFNDIndex : level.FNDIndex])).ToTexture(true);
        }

        public override async UniTask<PCX> GetWorldMapVigAsync(Context context)
        {
            return await LoadPCXAsync(context, 46);
        }

        protected async UniTask<PCX> LoadPCXAsync(Context context, int index)
        {
            var xor = LoadVignetteHeader(context)[index].XORKey;

            var path = GetVignetteFilePath(index);

            await AddFile(context, path);

            var s = context.Deserializer;
            PCX pcx = null;

            s.DoAt(context.GetRequiredFile(path).StartPointer, () =>
            {
                s.DoProcessed(new Xor8Processor(xor), () =>
                {
                    // Read the data
                    pcx = s.SerializeObject<PCX>(default, name: $"VIGNET");
                });
            });

            return pcx;
        }

        protected FileArchiveEntry[] LoadVignetteHeader(Context context)
        {
            var s = context.Deserializer;

            const string key = "VIGNET_Header";

            byte[] headerBytes = ArchiveHeaderTables.GetHeader(context.GetRequiredSettings<Ray1Settings>(), "VIGNET.DAT");
            int headerLength = headerBytes.Length / 12;

            if (!context.FileExists(key))
            {
                var headerStream = new MemoryStream(headerBytes);
                s.Context.AddStreamFile(key, headerStream);
            }

            BinaryFile file = context.GetRequiredFile(key);

            return s.DoAt(file.StartPointer, () => s.SerializeObjectArray<FileArchiveEntry>(default, headerLength, name: "Entries"));
        }

        public override void ExtractVignette(GameSettings settings, string vigPath, string outputDir)
        {
            // Create a new context
            using (var context = new Ray1MapContext(settings))
            {
                FileArchiveEntry[] entries = LoadVignetteHeader(context);
                var s = context.Deserializer;

                // Extract every .pcx file
                for (int i = 0; i < entries.Length; i++)
                {
                    var path = GetVignetteFilePath(i);
                    var file = new EncodedLinearFile(context, path, new GzipEncoder());

                    context.AddFile(file);

                    s.DoAt(file.StartPointer, () =>
                    {
                        s.DoProcessed(new Xor8Processor(entries[i].XORKey), () =>
                        {
                            // Read the data
                            var pcx = s.SerializeObject<PCX>(default, name: $"PCX[{i}]");

                            // Convert to a texture
                            var tex = pcx.ToTexture(true);

                            // Write the bytes
                            Util.ByteArrayToFile(Path.Combine(outputDir, $"{i}.png"), tex.EncodeToPNG());
                        });
                    });
                }
            }
        }

        #endregion
    }
}