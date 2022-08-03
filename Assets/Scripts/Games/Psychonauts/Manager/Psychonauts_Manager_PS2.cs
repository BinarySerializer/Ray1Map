using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PS2;
using PsychoPortal;
using PsychoPortal.Unity;
using UnityEngine;

namespace Ray1Map.Psychonauts
{
    public class Psychonauts_Manager_PS2 : Psychonauts_Manager
    {
        #region Game Actions

        public override GameAction[] GetGameActions(GameSettings settings)
        {
            return base.GetGameActions(settings).Concat(new GameAction[]
            {
                new("Export PS2 Files", false, true, (_, output) => ExportPS2Files(settings, output, false)),
                new("Export & Convert PS2 Files", false, true, (_, output) => ExportPS2Files(settings, output, true)),
                new("Generate PS2 File List", true, false, (input, _) => GeneratePS2FileList(settings, input)),
            }).ToArray();
        }

        public void GeneratePS2FileList(GameSettings settings, string sourceDirPath)
        {
            GameModeSelection[] ps2GameModes =
            {
                GameModeSelection.Psychonauts_PS2_EU, GameModeSelection.Psychonauts_PS2_US, GameModeSelection.Psychonauts_PS2_US_Demo
            };
            GameModeSelection[] gameModes =
            {
                GameModeSelection.Psychonauts_PC_Digital, GameModeSelection.Psychonauts_Xbox_Proto_20041217
            };
            Loader[] ps2Loaders = ps2GameModes.Select(x => new Ray1MapLoader(new PsychonautsSettings(GetVersion(x)), Settings.GameDirectories[x])).ToArray();
            Loader[] loaders = gameModes.Select(x => new Ray1MapLoader(new PsychonautsSettings(GetVersion(x)), Settings.GameDirectories[x])).ToArray();

            try
            {
                PS2.GenerateFileTable(ps2Loaders, loaders, sourceDirPath, GetLogger()).CopyToClipboard();
            }
            finally
            {
                foreach (Loader loader in ps2Loaders)
                    loader.Dispose();
                foreach (Loader loader in loaders)
                    loader.Dispose();
            }
        }

        public void ExportPS2Files(GameSettings settings, string outputPath, bool convertFiles)
        {
            using Ray1MapLoader loader = CreateLoader(settings);

            loader.LoadFilePackages(loader.Logger);

            ExportPS2Files(loader, loader.Logger, loader.FileManager.PS2_FileTable, outputPath, PS2_FileNames.FileTable.ToDictionary(PS2_FileEntry.GetFilePathHash), true, loader.FileManager.PS2_ResourcePacks, convertFiles);

            Debug.Log("Finished exporting");
        }

        public void ExportPS2Files(Loader loader, IBinarySerializerLogger logger, PS2_FileTable fileTable, string outputDir, Dictionary<uint, string> fileNames, bool categorizeUnnamed, Stream[] resourcePaks, bool convertFiles)
        {
            foreach (PS2_FileEntry file in fileTable.Files)
            {
                bool foundName = fileNames.TryGetValue(file.FilePathHash, out string n);
                string name = foundName ? n : $"{file.FilePathHash:X8}";

                byte[] fileBuffer;

                try
                {
                    fileBuffer = file.ReadFile(resourcePaks);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Error reading file {name}: {ex.Message}");
                    continue;
                }

                if (fileBuffer == null)
                    continue;

                if (!foundName && fileNames.Count > 0)
                {
                    const string unnamedDir = "_unnamed";

                    if (categorizeUnnamed && fileBuffer.Length >= 4)
                    {
                        uint magic = BitConverter.ToUInt32(fileBuffer, 0);
                        string ext = null;
                        string type = null;

                        if (magic == 0x4150414B) // APAK
                        {
                            type = "Animation Packs";
                            ext = ".apf";
                        }
                        else if (magic == 0x45504241) // EPBA
                        {
                            type = "Blend Animations";
                            ext = ".pba";
                        }
                        else if (magic == 0x50455645) // PEVE
                        {
                            type = "Event Animations";
                            ext = ".eve";
                        }
                        else if (magic is 0x504A4158 or 0x504A414E) // PJAX or PJAN
                        {
                            type = "Joint Animations";
                            ext = ".ja2";
                        }
                        else if (magic == 0x50535943) // PSYC
                        {
                            type = "Meshes";
                            ext = ".pl2";
                        }
                        else if (magic == 0x4B415050) // PPAK
                        {
                            type = "Level Packs";
                            ext = ".ppf";
                        }
                        else if (magic == 0x61754C1B) // .LUA
                        {
                            type = "Scripts";
                            ext = ".lub";
                        }
                        else if (magic == 0x69746341) // Acti(onFile)
                        {
                            type = "Animation Action Files";
                            ext = ".asd";
                        }
                        else if (magic == 0x4543414D) // MACE
                        {
                            type = "Camera Paths";
                            ext = ".cam";
                        }
                        else if (magic == 0xBA010000)
                        {
                            type = "Videos";
                            ext = ".pss";
                        }
                        else if ((magic & 0xFFFF) == 0x4257) // WB
                        {
                            type = "Sounds";
                            ext = ".pwb";
                        }
                        else if ((magic & 0xFFFF) == 0x4253) // SB
                        {
                            type = "Sounds";
                            ext = ".psb";
                        }
                        else if ((magic & 0xFFFFFF) == 0x325350) // PS2
                        {
                            type = "Textures";
                            ext = ".ps2";
                        }

                        if (type != null && ext != null)
                            name = Path.Combine(unnamedDir, type, $"{name}{ext}");
                        else
                            name = Path.Combine(unnamedDir, name);
                    }
                    else
                    {
                        name = Path.Combine(unnamedDir, name);
                    }
                }

                string outputFile = Path.Combine(outputDir, name);

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                File.WriteAllBytes(outputFile, fileBuffer);

                if (convertFiles)
                {
                    string ext = Path.GetExtension(outputFile);

                    if (ext.Equals(".ps2", StringComparison.InvariantCultureIgnoreCase))
                    {
                        PS2_Texture ps2Tex = Binary.ReadFromBuffer<PS2_Texture>(fileBuffer, loader.Settings, onPreSerializing: (_, x) => x.Pre_IsPacked = false, logger: logger, name: name);

                        Texture2D tex = ps2Tex.ToTexture2D(flipY: true);
                        tex.ExportPNG(Path.ChangeExtension(outputFile, ".png"));
                    }
                }
            }
        }

        #endregion

        #region Load

        public override PsychonautsMeshFrag LoadMeshFrag(Ray1MapLoader loader, MeshFrag meshFrag, Transform parent, int index, PsychonautsTexture[] textures, PsychonautsSkeleton[] skeletons, Matrix4x4[][] bindPoses)
        {
            // Convert PS2 mesh data to common format
            try
            {
                InitPS2MeshFrag(loader, meshFrag);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to initialize PS2 mesh frag: {ex.Message}");
                return null;
            }

            return base.LoadMeshFrag(loader, meshFrag, parent, index, textures, skeletons, bindPoses);
        }

        public void InitPS2MeshFrag(Ray1MapLoader loader, MeshFrag meshFrag)
        {
            const string key = "geo";

            // NOTE: This is all very work in process and barely functions. PS2 graphics are complicated :/

            // Some helpful links I found for when I get back to working on this again:
            // https://psi-rockin.github.io/ps2tek
            // https://github.com/PCSX2/pcsx2/issues/1803 > https://github.com/Fireboyd78/driver-tools/tree/dev/GMC2Snooper/PS2
            // https://github.com/PCSX2/pcsx2/tree/master/pcsx2

            try
            {
                // Serialize using BinarySerializer for now (Psychonauts normally uses PsychoPortal)
                loader.Context.AddFile(new StreamFile(loader.Context, key, new MemoryStream(meshFrag.PS2_GeometryBuffer)));
                PS2_GeometryCommands cmds = FileFactory.Read<PS2_GeometryCommands>(loader.Context, key);

                List<VertexNotexNorm> vertices = new();
                List<List<PS2_UV16>> uvs = new();

                float toFloat(int value) => BitConverter.ToSingle(BitConverter.GetBytes(value));
                uint[] row = new uint[4];
                
                // Execute program
                foreach (PS2_GeometryCommand cmd in cmds.Commands)
                {
                    if (cmd.VIFCode.IsUnpack)
                    {
                        VIFcode_Unpack unpack = cmd.VIFCode.GetUnpack();
                        uint addr = unpack.ADDR;

                        // TODO: Unpack these. Why is there no data? How does the masking work? Is the cycle related?
                        if (unpack.M)
                            continue;

                        switch (addr) // TODO: Is using the address like this reliable?
                        {
                            // GIFTag
                            case 0:
                                // Verify flags match
                                if (cmd.GIFTag.PRIM.PrimitiveType == PRIM_PrimitiveType.TriangleStrip !=
                                    meshFrag.MaterialFlags.HasFlag(MaterialFlags.Tristrip))
                                {
                                    throw new Exception("Triangle strip flags are not set correctly");
                                }

                                // TODO: Get relevant properties from PRIM such as shading, alpha blending etc. Also get count from here!

                                break;

                            // Unused?
                            case 1:
                                throw new Exception("Data written to address 1");

                            // Vertices
                            case 2:
                                foreach (PS2_Vector3_Int16 vec in cmd.Vertices)
                                {
                                    vertices.Add(new VertexNotexNorm()
                                    {
                                        // TODO: Fix the offset here. Hard-coding it to this only works in some levels. Game
                                        //       seems to convert it to a float using the row data?
                                        Vertex = new Vec3(
                                            vec.X - 0x8000,
                                            vec.Y - 0x8000,
                                            vec.Z - 0x8000),

                                        // TODO: Either find a defined normal or calculate it manually
                                        Normal = new NormPacked3(),
                                    });
                                }
                                break;

                            case 3:
                                // TODO: Implement. Vertex colors/normals?
                                break;

                            case 4:
                                // TODO: Implement. Vertex colors/normals?
                                break;

                            // UV
                            case 5:
                            case 6:
                            case 7:
                                // TODO: Cleaner way of handling this. Psychonauts can have up to 3 UV-maps.
                                int uvIndex = (int)(addr - 5);

                                if (uvs.Count <= uvIndex)
                                    uvs.Insert(uvIndex, new List<PS2_UV16>());

                                foreach (PS2_UV16 uv in cmd.UVs)
                                    uvs[uvIndex].Add(uv);

                                if (uvs[uvIndex].Count != vertices.Count)
                                    throw new Exception($"UVs count don't match vertices {uvs[uvIndex].Count} != {vertices.Count}");

                                break;

                        }
                    }
                    else
                    {
                        switch (cmd.VIFCode.CMD)
                        {
                            case VIFcode.Command.STROW:
                                row = cmd.ROW; // Default data to fill rows with?
                                break;

                            case VIFcode.Command.STCOL:
                                Debug.LogWarning("Column command is used"); // Hopefully this is never used
                                break;

                            case VIFcode.Command.STMASK:
                                Debug.LogWarning("Mask command is used"); // Multiple 2-bit values?
                                break;
                        }
                    }

                }

                // Convert to Psychonauts UV sets
                List<UVSet> uvSets = Enumerable.Range(0, uvs.First().Count).Select(x => new UVSet()
                {
                    UVs = new UV[uvs.Count]
                }).ToList();

                for (var uvIndex = 0; uvIndex < uvs.Count; uvIndex++)
                {
                    List<PS2_UV16> uvList = uvs[uvIndex];

                    if (uvList.Count != uvSets.Count)
                        throw new Exception("UV count mismatch");

                    for (int i = 0; i < uvList.Count; i++)
                    {
                        uvSets[i].UVs[uvIndex] = new UV()
                        {
                            Pre_Version = 316, // TODO: This is a hack to use floats. Convert to integer values.

                            // TODO: Is this even correct? Value range should be 0-1, but it's usually above 250...
                            U_Float = toFloat(uvList[i].U | (int)row[0]),
                            V_Float = toFloat(uvList[i].V | (int)row[1])
                        };
                    }
                }

                // TODO: This is because of some UV arrays not being defined as normal packed data. For now we just trim the data
                //       so the count matches and Unity can load it... But we should find a better solution.
                if (uvSets.Count > vertices.Count)
                    uvSets.RemoveRange(vertices.Count, uvSets.Count - vertices.Count);
                else if (vertices.Count > uvSets.Count)
                    vertices.RemoveRange(uvSets.Count, vertices.Count - uvSets.Count);

                // Vertices
                meshFrag.Vertices = vertices.ToArray();

                // TODO: Implement vertex colors - verify with PC/Xbox release to make sure they're hanled correctly
                // Vertex colors
                //meshFrag.HasVertexColors = 1;
                //meshFrag.VertexColors = cmds.Commands.Where(x => x.VertexColors != null).SelectMany(x => x.VertexColors).Select(x => new PsychoPortal.RGBA8888Color
                //{
                //    Red = x.R,
                //    Green = x.G,
                //    Blue = x.B,
                //    Alpha = 255
                //}).ToArray();

                // TODO: Optimize this by reusing polygons
                // Polygons
                meshFrag.PolygonIndexBuffer = Enumerable.Range(0, meshFrag.Vertices.Length).Select(x => (short)x).ToArray();

                //Debug.Log(String.Join(Environment.NewLine, vertices.Select(x => x.Vertex)));
                //Debug.Log(String.Join(Environment.NewLine, uvSets.Select(x => x.UVs[0].ToVec2())));

                // UVs
                meshFrag.UVScale = 1; // TODO: Does the PS2 version use some sort of UV scaling like other versions?
                meshFrag.UVSets = uvSets.ToArray();

                if (uvSets.Count != vertices.Count)
                    throw new Exception($"Invalid UV count. {uvSets.Count} != {vertices.Count}");
            }
            finally
            {
                loader.Context.RemoveFile(key);
            }
        }

        #endregion
    }
}
