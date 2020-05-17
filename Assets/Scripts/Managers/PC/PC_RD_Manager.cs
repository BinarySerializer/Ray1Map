using R1Engine.Serialize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace R1Engine
{
    /// <summary>
    /// The game manager for Rayman Designer (PC)
    /// </summary>
    public class PC_RD_Manager : PC_Manager
    {
        #region Static Properties

        /// <summary>
        /// The events which are multi-colored
        /// </summary>
        public static EventType[] MultiColoredEvents => new[]
        {
            EventType.MS_compteur,
            EventType.MS_wiz_comptage,
            EventType.MS_pap,
        };

        /// <summary>
        /// The DES which are multi-colored
        /// </summary>
        public static string[] MultiColoredDES => new[]
        {
            "WIZCOMPT",
            "PCH",
        };

        #endregion

        #region Values and paths

        /// <summary>
        /// Gets the file path for the specified level
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The level file path</returns>
        public override string GetLevelFilePath(GameSettings settings) => GetDataPath() + $"{GetShortWorldName(settings.World)}{settings.Level:00}.LEV";

        /// <summary>
        /// Gets the file path for the specified world file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The world file path</returns>
        public override string GetWorldFilePath(GameSettings settings) => GetDataPath() + $"RAY{((int)settings.World + 1):00}.WLD";

        /// <summary>
        /// Gets the file path for the vignette file
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The vignette file path</returns>
        public override string GetVignetteFilePath(GameSettings settings) => GetDataPath() + $"VIGNET.DAT";

        /// <summary>
        /// Gets the file path for the primary sound file
        /// </summary>
        /// <returns>The primary sound file path</returns>
        public override string GetSoundFilePath() => GetDataPath() + $"SNDD8B.DAT";

        /// <summary>
        /// Gets the file path for the primary sound manifest file
        /// </summary>
        /// <returns>The primary sound manifest file path</returns>
        public override string GetSoundManifestFilePath() => GetDataPath() + $"SNDH8B.DAT";

        /// <summary>
        /// Indicates if the game has 3 palettes it swaps between
        /// </summary>
        public override bool Has3Palettes => false;

        /// <summary>
        /// Gets the levels for each world
        /// </summary>
        /// <param name="settings">The game settings</param>
        /// <returns>The levels</returns>
        public override KeyValuePair<World, int[]>[] GetLevels(GameSettings settings) => EnumHelpers.GetValues<World>().Select(w => new KeyValuePair<World, int[]>(w, Directory.EnumerateFiles(settings.GameDirectory + GetDataPath(), $"{GetShortWorldName(w)}??.LEV", SearchOption.TopDirectoryOnly)
            .Select(FileSystem.GetFileNameWithoutExtensions)
            .Select(x => Int32.Parse(x.Substring(3)))
            .ToArray())).ToArray();

        /// <summary>
        /// Gets the DES file names, in order, for the world
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="includeExtension">Indicates if the file extension should be included</param>
        /// <returns>The DES file names</returns>
        public override string[] GetDESNames(Context context, bool includeExtension)
        {
            // Get the key
            var key = $"DES{context.Settings.World}";

            // Attempt to get names from cache
            var names = context.GetStoredObject<string[]>(key);

            // If not found, retrieve them
            if (names == null)
            {
                // Get the names and include a dummy item for DES 0
                names = new string[]
                {
                    "DES_0.DES"
                }.Concat(EnumerateWLDManifest(context).Where(str => str.Contains("DES"))).ToArray();

                // Cache the names
                context.StoreObject(key, names);
            }

            // Return the names
            return includeExtension ? names : names.Select(x => x.Remove(x.Length - 4)).ToArray();
        }

        /// <summary>
        /// Gets the ETA file names, in order, for the world
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The ETA file names</returns>
        /// <param name="includeExtension">Indicates if the file extension should be included</param>
        public override string[] GetETANames(Context context, bool includeExtension)
        {
            // Get the key
            var key = $"ETA{context.Settings.World}";

            // Attempt to get names from cache
            var names = context.GetStoredObject<string[]>(key);

            // If not found, retrieve them
            if (names == null)
            {
                // Get the names
                names = EnumerateWLDManifest(context).Where(str => str.Contains("ETA")).ToArray();

                // Cache the names
                context.StoreObject(key, names);
            }

            // Return the names
            return includeExtension ? names : names.Select(x => x.Remove(x.Length - 4)).ToArray();
        }

        /// <summary>
        /// Enumerates the strings in a .wld manifest
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns>The found strings</returns>
        protected IEnumerable<string> EnumerateWLDManifest(Context context)
        {
            // Get the encoding
            var e = Settings.StringEncoding;

            // TODO: Find better way to parse this
            // Read the world file and get the last data group
            var wld = FileFactory.Read<PC_WorldFile>(GetWorldFilePath(context.Settings), context,
                (s, data) => data.FileType = PC_WorldFile.Type.World).Unknown5;

            // Get the DES file names
            for (int i = 1; i < wld.Length; i += 13)
            {
                // Read the bytes until we reach NULL
                var length = 0;

                for (int j = 0; j < 13; j++, length++)
                {
                    if (wld[i + j] == 0x00)
                        break;
                }

                // Get the string
                var str = e.GetString(wld, i, length);

                // Return it
                yield return str;
            }
        }

        /// <summary>
        /// Gets the DES file name for the current index in the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="desIndex">The DES index</param>
        /// <param name="includeExtension">Indicates if the file extension should be included</param>
        /// <returns>The file name</returns>
        public string GetDESFileName(Context context, int desIndex, bool includeExtension)
        {
            // Get the file names
            var desNames = GetDESNames(context, includeExtension);

            // Return the name
            return desNames.ElementAtOrDefault(desIndex) ?? $"DES_{desIndex}";
        }

        /// <summary>
        /// Gets the ETA file name for the current index in the current context
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="etaIndex">The ETA index</param>
        /// <param name="includeExtension">Indicates if the file extension should be included</param>
        /// <returns>The file name</returns>
        public string GetETAFileName(Context context, int etaIndex, bool includeExtension)
        {
            // Get the file names
            var etaNames = GetETANames(context, includeExtension);

            // Return the name
            return etaNames.ElementAtOrDefault(etaIndex) ?? $"ETA_{etaIndex}";
        }

        /// <summary>
        /// Gets the archive files which can be extracted
        /// </summary>
        public override ArchiveFile[] GetArchiveFiles(GameSettings settings)
        {
            return new ArchiveFile[]
            {
                new ArchiveFile($"PCMAP/COMMON.DAT"),
                new ArchiveFile($"PCMAP/SNDD8B.DAT"),
                new ArchiveFile($"PCMAP/SNDH8B.DAT"),
                new ArchiveFile($"PCMAP/VIGNET.DAT", ".pcx"),
            }.Concat(Directory.GetDirectories(settings.GameDirectory + "PCMAP").Select(Path.GetFileName).SelectMany(x => new ArchiveFile[]
            {
                new ArchiveFile($"PCMAP/{x}/SNDSMP.DAT"),
                new ArchiveFile($"PCMAP/{x}/SPECIAL.DAT"),
            })).ToArray();
        }

        /// <summary>
        /// Gets additional sound archives
        /// </summary>
        /// <param name="settings">The game settings</param>
        public override AdditionalSoundArchive[] GetAdditionalSoundArchives(GameSettings settings) => 
            Directory.GetDirectories(settings.GameDirectory + "PCMAP").
                Select(Path.GetFileName).
                Select(x => new AdditionalSoundArchive($"SMP ({x})", new ArchiveFile($"PCMAP/{x}/SNDSMP.DAT"))).ToArray();

        #endregion

        #region Manager Methods

        /// <summary>
        /// Gets a common design
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="des">The DES</param>
        /// <param name="palette">The palette to use</param>
        /// <param name="desIndex">The DES index</param>
        /// <returns>The common design</returns>
        public override Common_Design GetCommonDesign(Context context, PC_DES des, IList<ARGBColor> palette, int desIndex)
        {
            // Check if the DES is multi-colored
            if (!MultiColoredDES.Contains(GetDESFileName(context, desIndex + 1, false)))
                return base.GetCommonDesign(context, des, palette, desIndex);

            // Create the common design
            Common_Design commonDesign = new Common_Design
            {
                Sprites = new List<Sprite>(),
                Animations = new List<Common_Animation>()
            };

            // Process the image data
            var processedImageData = ProcessImageData(des.ImageData, des.RequiresBackgroundClearing);

            // Add sprites for each color
            for (int i = 0; i < 6; i++)
            {
                // Hack to get correct colors
                var p = palette.Skip(i * 8 + 1).ToList();
                
                p.Insert(0, new ARGBColor(0, 0, 0));

                if (i % 2 != 0)
                {
                    p[8] = palette[i * 8];
                }
                    
                // Sprites
                foreach (var s in des.ImageDescriptors)
                {
                    // Get the texture
                    Texture2D tex = GetSpriteTexture(s, p, processedImageData);

                    // Add it to the array
                    commonDesign.Sprites.Add(tex == null ? null : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0f, 1f), 16, 20));
                }
            }

            // Add animations for each color
            for (int i = 0; i < 6; i++)
            {
                // Animations
                foreach (var a in des.AnimationDescriptors)
                {
                    // Create a clone animation
                    var ca = new PC_AnimationDescriptor
                    {
                        LayersPerFrame = a.LayersPerFrame,
                        Unknown1 = a.Unknown1,
                        FrameCount = a.FrameCount,
                        Unknown2 = a.Unknown2,
                        Unknown3 = a.Unknown3,
                        FrameTableOffset = a.FrameTableOffset,
                        Layers = a.Layers.Select(x => new Common_AnimationLayer
                        {
                            IsFlippedHorizontally = x.IsFlippedHorizontally,
                            XPosition = x.XPosition,
                            YPosition = x.YPosition,
                            ImageIndex = (byte)(x.ImageIndex + (des.ImageDescriptors.Length * i))
                        }).ToArray(),
                        Frames = a.Frames
                    };

                    // Add the animation to list
                    commonDesign.Animations.Add(ca.ToCommonAnimation());
                }
            }

            return commonDesign;
        }

        /// <summary>
        /// Gets an editor manager from the specified objects
        /// </summary>
        /// <param name="level">The common level</param>
        /// <param name="context">The context</param>
        /// <param name="designs">The common design</param>
        /// <returns>The editor manager</returns>
        public override BaseEditorManager GetEditorManager(Common_Lev level, Context context, Common_Design[] designs) => new PC_RD_EditorManager(level, context, this, designs);

        #endregion
    }
}