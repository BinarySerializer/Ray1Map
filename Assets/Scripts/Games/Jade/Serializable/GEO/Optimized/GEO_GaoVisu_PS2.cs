using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using BinarySerializer.PS2;
using BinarySerializer.PSP;

namespace Ray1Map.Jade {
	public class GEO_GaoVisu_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		#region Un-optimize GeometricObject data (PSP)
		public void ExecuteGEPrograms(OBJ_GameObject jadeGao, GEO_GeometricObject geo, GEO_GeoObject_PS2 obj) {
			int elementsLength = System.Math.Max((obj.ElementData?.ElementDatas?.Length ?? 0), (Elements?.Length ?? 0));
			List<GEO_GeometricObjectElement> elements = new List<GEO_GeometricObjectElement>();

			List<Jade_Vector> vertices = new List<Jade_Vector>();
			List<Jade_Vector> normals = new List<Jade_Vector>();
			List<GEO_GeometricObject.UV> uvs = new List<GEO_GeometricObject.UV>();
			List<Jade_Color> colors = new List<Jade_Color>();
			uint smoothingGroup = 0;

			// UVs are in ST format, we need texture width & height to convert them to UV
			Jade_TextureReference[] MainTextures = null;
			var gro_m = jadeGao?.Base?.Visual?.Material?.Value;
			if (gro_m != null) {
				Jade_TextureReference GetTextureFromRenderObject(GRO_Struct renderObject) {
					if (renderObject == null) return null;
					if (renderObject.Type == GRO_Type.MAT_MTT) {
						var mat_mtt = (MAT_MTT_MultiTextureMaterial)renderObject.Value;

						if ((mat_mtt.Levels?.Length ?? 0) > 0) {
							for (int i = 0; i < mat_mtt.Levels.Length; i++) {
								var texRef = mat_mtt.Levels[i].Texture;
								if (texRef?.Info != null) return texRef;
							}
						}
					} else if (renderObject.Type == GRO_Type.MAT_SIN) {
						var mat_sin = (MAT_SIN_SingleMaterial)renderObject.Value;
						var texRef = mat_sin.Texture;
						if (texRef?.Info != null) return texRef;
					}
					return null;
				}
				if (gro_m.RenderObject.Type == GRO_Type.MAT_MTT || gro_m.RenderObject.Type == GRO_Type.MAT_SIN) {
					MainTextures = new Jade_TextureReference[1];
					MainTextures[0] = GetTextureFromRenderObject(gro_m.RenderObject);
				} else if (gro_m.RenderObject.Type == GRO_Type.MAT_MSM) {
					var mat = (MAT_MSM_MultiSingleMaterial)gro_m.RenderObject.Value;
					MainTextures = new Jade_TextureReference[mat.Materials.Length];
					for (int i = 0; i < mat.Materials.Length; i++) {
						var gro_m_sub = mat.Materials[i]?.Value?.RenderObject;
						MainTextures[i] = GetTextureFromRenderObject(gro_m_sub);
					}
				}
			}
			var texturesLength = MainTextures?.Length ?? 0;

			for (int elementIndex = 0; elementIndex < elementsLength; elementIndex++) {
				Element visuElement = (Elements.Length > elementIndex) ? Elements[elementIndex] : null;
				GEO_GeoObject_PS2.ElementDataBuffer.ElementData objElement = (obj.ElementData.ElementDatas.Length > elementIndex) ? obj.ElementData.ElementDatas[elementIndex] : null;
				int subElementsLength = System.Math.Max((objElement?.MeshElements?.Length ?? 0), (visuElement?.Refs?.Length ?? 0));
				var materialId = obj?.Elements_MaterialId[elementIndex] ?? 0;

				Jade_TextureReference curTex = null;
				if (texturesLength > 0) {
					if (texturesLength == 1) {
						curTex = MainTextures[0];
					} else {
						if (materialId < texturesLength) {
							curTex = MainTextures[materialId];
						}
					}
				}
				uint w = (uint?)curTex?.Content?.Content_JTX?.PSP_Content?.Width ?? curTex?.Info?.Content_JTX?.Width ?? curTex?.Info?.Width ?? 64;
				uint h = (uint?)curTex?.Content?.Content_JTX?.PSP_Content?.Height ?? curTex?.Info?.Content_JTX?.Height ?? curTex?.Info?.Height ?? 64;
				//float uScale = w / 2f;
				//float vScale = h / 2f;

				float uScale = 32f;//h / 2f;
				float vScale = uScale;
				//float uScale = System.Math.Max(w, h) / 4f;
				//float vScale = uScale;

				List<GEO_GeometricObjectElement.Triangle> triangles = new List<GEO_GeometricObjectElement.Triangle>();

				for (int subElementIndex = 0; subElementIndex < subElementsLength; subElementIndex++) {
					MeshElementRef visuSubElement = ((visuElement?.Refs?.Length ?? 0) > subElementIndex) ? visuElement.Refs[subElementIndex] : null;
					GEO_GeoObject_PS2.ElementDataBuffer.MeshElement objSubElement = ((objElement?.MeshElements?.Length ?? 0) > subElementIndex) ? objElement.MeshElements[subElementIndex] : null;

					List<PSP_GEData> geData = new List<PSP_GEData>();
					if (visuSubElement != null) {
						geData.AddRange(visuSubElement.GEPrograms);;
					}
					if (objSubElement != null) {
						geData.AddRange(objSubElement.GEPrograms);
					}


					foreach (var ge in geData) {
						if(ge.SerializedCommands == null) continue;

						List<Jade_Vector> currentVertices = new List<Jade_Vector>();
						List<Jade_Vector> currentNormals = new List<Jade_Vector>();
						List<GEO_GeometricObject.UV> currentUVs = new List<GEO_GeometricObject.UV>();
						List<Jade_Color> currentColors = new List<Jade_Color>();
						List<GEO_GeometricObjectElement.Triangle> currentTriangles = new List<GEO_GeometricObjectElement.Triangle>();

						GE_PrimitiveType primitiveType = GE_PrimitiveType.Triangles;
						GE_Command_VertexType vertexType = null;
						foreach (var cmd in ge.SerializedCommands) {
							switch (cmd.Command) {
								case GE_CommandType.PRIM:
									primitiveType = ((GE_Command_PrimitiveKick)cmd.Data).PrimitiveType;
									break;
								case GE_CommandType.VTYPE:
									vertexType = (GE_Command_VertexType)cmd.Data;
									break;
							}
							if (cmd.LinkedVertices != null) {
								int lastVerticesCount = currentVertices.Count;
								int lastUVsCount = currentUVs.Count;
								foreach (var vtxLine in cmd.LinkedVertices) {
									foreach (var vtx in vtxLine.Vertices) {
										if (vtx.Position != null) {
											var vec = new Jade_Vector(
												(vtx.Position[0].Value) * (objSubElement?.VertexScale?.X ?? 1f) + (objSubElement?.VertexOffset?.X ?? 0),
												(vtx.Position[1].Value) * (objSubElement?.VertexScale?.Y ?? 1f) + (objSubElement?.VertexOffset?.Y ?? 0),
												(vtx.Position[2].Value) * (objSubElement?.VertexScale?.Z ?? 1f) + (objSubElement?.VertexOffset?.Z ?? 0));
											currentVertices.Add(vec);
										}
										if (vtx.Normal != null) {
											var vec = new Jade_Vector(
												vtx.Normal[0].Value,
												vtx.Normal[1].Value,
												vtx.Normal[2].Value);
											currentNormals.Add(vec);
										}
										if (vtx.UV != null) {
											var uv = new GEO_GeometricObject.UV(
												vtx.UV[0].Value * uScale,
												vtx.UV[1].Value * vScale);
											if (BitHelpers.ExtractBits64(ge.Flags, 1, 7) == 1) {
												uv.U /= 2f;
												uv.V /= 2f;
											}
											currentUVs.Add(uv);
										}
										if (vtx.Color != null) {
											var usedCol = vtx.Color.Color;
											var col = new Jade_Color(usedCol.Red, usedCol.Green, usedCol.Blue, usedCol.Alpha);
											currentColors.Add(col);
										}
									}
								}
								int newVerticesCount = currentVertices.Count;
								int newUVsCount = currentUVs.Count;
								int curVerticesCount = (newVerticesCount - lastVerticesCount);
								switch (primitiveType) {
									case GE_PrimitiveType.TriangleStrips:
										for (int i = 2; i < curVerticesCount; i++) {
											var tri = new GEO_GeometricObjectElement.Triangle();
											if (vertexType.PositionFormat != GE_VertexNumberFormat.None) {
												tri.Vertex0 = (ushort)(lastVerticesCount + i - 2);
												tri.Vertex1 = (ushort)(lastVerticesCount + i - 1);
												tri.Vertex2 = (ushort)(lastVerticesCount + i - 0);
											}
											if (vertexType.TextureFormat != GE_VertexNumberFormat.None) {
												tri.UV0 = (ushort)(lastUVsCount + i - 2);
												tri.UV1 = (ushort)(lastUVsCount + i - 1);
												tri.UV2 = (ushort)(lastUVsCount + i - 0);
											}
											if (i % 2 == 1) {
												ushort tmp = tri.Vertex0;
												tri.Vertex0 = tri.Vertex1;
												tri.Vertex1 = tmp;
												tmp = tri.UV0;
												tri.UV0 = tri.UV1;
												tri.UV1 = tmp;
											}
											tri.SmoothingGroup = smoothingGroup;
											currentTriangles.Add(tri);

											/*tri = new GEO_GeometricObjectElement.Triangle() {
												SmoothingGroup = tri.SmoothingGroup,
												Vertex0 = tri.Vertex0,
												Vertex1 = tri.Vertex2,
												Vertex2 = tri.Vertex1,
												UV0 = tri.UV0,
												UV1 = tri.UV2,
												UV2 = tri.UV1,
											};
											currentTriangles.Add(tri);*/
										}
										break;
									case GE_PrimitiveType.Triangles:
										for (int i = 0; i < curVerticesCount / 3; i++) {
											var tri = new GEO_GeometricObjectElement.Triangle();
											if (vertexType.PositionFormat != GE_VertexNumberFormat.None) {
												tri.Vertex0 = (ushort)(lastVerticesCount + i * 3 + 0);
												tri.Vertex1 = (ushort)(lastVerticesCount + i * 3 + 1);
												tri.Vertex2 = (ushort)(lastVerticesCount + i * 3 + 2);
											}
											if (vertexType.TextureFormat != GE_VertexNumberFormat.None) {
												tri.UV0 = (ushort)(lastUVsCount + i * 3 + 0);
												tri.UV1 = (ushort)(lastUVsCount + i * 3 + 1);
												tri.UV2 = (ushort)(lastUVsCount + i * 3 + 2);
											}

											tri.SmoothingGroup = smoothingGroup;
											currentTriangles.Add(tri);


											/*tri = new GEO_GeometricObjectElement.Triangle() {
												SmoothingGroup = tri.SmoothingGroup,
												Vertex0 = tri.Vertex0,
												Vertex1 = tri.Vertex2,
												Vertex2 = tri.Vertex1,
												UV0 = tri.UV0,
												UV1 = tri.UV2,
												UV2 = tri.UV1,
											};
											currentTriangles.Add(tri);*/
										}
										break;
									default:
										throw new System.NotImplementedException($"Unsupported PrimitiveType {primitiveType}");
								}
								smoothingGroup++;
							}
						}


						int totalVerticesCount = vertices.Count;
						int totalUVsCount = uvs.Count;
						int currentUVsCount = currentUVs.Count;
						vertices.AddRange(currentVertices);
						normals.AddRange(currentNormals);
						uvs.AddRange(currentUVs);
						colors.AddRange(currentColors);
						foreach (var tri in currentTriangles) {
							if (currentUVsCount > 0) {
								tri.UV0 += (ushort)totalUVsCount;
								tri.UV1 += (ushort)totalUVsCount;
								tri.UV2 += (ushort)totalUVsCount;
							}
							tri.Vertex0 += (ushort)totalVerticesCount;
							tri.Vertex1 += (ushort)totalVerticesCount;
							tri.Vertex2 += (ushort)totalVerticesCount;
						}
						triangles.AddRange(currentTriangles);
					}
					/*foreach (var ge in geData) {
						ge.Execute(geo.Context);
					}*/
				}

				GEO_GeometricObjectElement element = new GEO_GeometricObjectElement() {
					GeometricObject = geo,
					MaterialID = obj.Elements_MaterialId[elementIndex],
					Triangles = triangles.ToArray(),
				};
				element.TrianglesCount = (uint)element.Triangles.Length;
				elements.Add(element);
			}

			geo.Elements = elements.ToArray();
			geo.ElementsCount = (uint)geo.Elements.Length;
			geo.Vertices = vertices.ToArray();
			geo.VerticesCount = (uint)geo.Vertices.Length;
			geo.UVs = uvs.ToArray();
			geo.UVsCount = (uint)geo.UVs.Length;
			if (normals.Count > 0) {
				geo.Normals = normals.ToArray();
				geo.Montreal_HasNormals = 1;
			}
			if (colors.Count > 0) {
				if (jadeGao?.Base?.Visual != null) {
					jadeGao.Base.Visual.VertexColors = colors.ToArray();
					jadeGao.Base.Visual.VertexColorsCount = (uint)jadeGao.Base.Visual.VertexColors.Length;
				} else {
					geo.Colors = colors.ToArray();
					geo.Montreal_HasColors = 1;
					geo.ColorsCount = (uint)geo.Colors.Length;
				}
			}
		}
		#endregion

		#region Un-optimize GeometricObject data (PS2)
		public void ExecuteChainPrograms(OBJ_GameObject jadeGao, GEO_GeometricObject geo, GEO_GeoObject_PS2 obj) {
			List<Jade_Vector> vertices = new List<Jade_Vector>();
			List<Jade_Vector> normals = new List<Jade_Vector>();
			List<GEO_GeometricObject.UV> uvs = new List<GEO_GeometricObject.UV>();
			List<Jade_Color> colors = new List<Jade_Color>();
			int elementsLength = System.Math.Max((obj.ElementData?.ElementDatas?.Length ?? 0), (Elements?.Length ?? 0));
			List<GEO_GeometricObjectElement> elements = new List<GEO_GeometricObjectElement>();

			for (int elementIndex = 0; elementIndex < elementsLength; elementIndex++) {
				Element visuElement = (Elements.Length > elementIndex) ? Elements[elementIndex] : null;
				GEO_GeoObject_PS2.ElementDataBuffer.ElementData objElement = (obj.ElementData.ElementDatas.Length > elementIndex) ? obj.ElementData.ElementDatas[elementIndex] : null;
				int subElementsLength = System.Math.Max((objElement?.MeshElements?.Length ?? 0), (visuElement?.Refs?.Length ?? 0));

				List<GEO_GeometricObjectElement.Triangle> triangles = new List<GEO_GeometricObjectElement.Triangle>();



				for (int subElementIndex = 0; subElementIndex < subElementsLength; subElementIndex++) {
					MeshElementRef visuSubElement = ((visuElement?.Refs?.Length ?? 0) > subElementIndex) ? visuElement.Refs[subElementIndex] : null;
					GEO_GeoObject_PS2.ElementDataBuffer.MeshElement objSubElement = ((objElement?.MeshElements?.Length ?? 0) > subElementIndex) ? objElement.MeshElements[subElementIndex] : null;

					List<PS2_DMAChainData> chainData = new List<PS2_DMAChainData>();
					List<PS2_DMAChainProgram> chainPrograms = new List<PS2_DMAChainProgram>();
					if (visuSubElement != null) {
						chainData.AddRange(visuSubElement.InstanceVIFPrograms);
						chainPrograms.AddRange(visuSubElement.DMAChainPrograms);
					}
					if (objSubElement != null) {
						chainData.AddRange(objSubElement.VIFPrograms);
						chainPrograms.AddRange(objSubElement.DMAChainPrograms);
					}
					//string testPath = Path.Combine(Path.GetDirectoryName(Settings.LogFile), "test_jade_ps2");
					int curDMAId = 0;


					int minBonesCount = 2;
					// TODO: Determine based on Visu? check code?
					if (geo.Context.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_WW)) {
						minBonesCount = 1;
					}
					bool hasBones = objSubElement?.BonesCount >= minBonesCount;


					IEnumerable<byte[]> EnumerateVIFPrograms() {
						if (chainPrograms.Any()) {
							chainPrograms.Sort((x, y) => x.DMAChainProgramID.CompareTo(y.DMAChainProgramID));
							foreach (var prog in chainPrograms) {
								if (prog.Commands.Length > 0) {
									using (MemoryStream ms = new MemoryStream()) {
										using (Writer w = new Writer(ms, isLittleEndian: true, leaveOpen: true)) {
											foreach (var c in prog.Commands) {
												c.Transfer(w, chainData);
											}
										}
										curDMAId = prog.DMAChainProgramID;
										yield return ms.ToArray();
									}
								}
							}
						}
						foreach (var c in chainData) {
							curDMAId = -1;
							if (c.DMAChainDataID == -1) yield return c.Bytes;
						}
					}
					IEnumerable<PS2_VU_Data> EnumerateVUData(byte[] vifProg, int vifProgIndex) {
						VIF_Commands VIFProgram = null;
						var chainProgKey = $"ChainProgram_DMA{curDMAId}__{jadeGao.Key}__{elementIndex}-{subElementIndex}-{vifProgIndex}";
						using (Context c = new Context("", serializerLog: new ParentContextSerializerLog(Context.SerializerLog))) {
							// Parse VIF program
							var file = c.AddStreamFile(chainProgKey, new MemoryStream(vifProg));
							try {
								var s = c.Deserializer;
								var parserPass1 = new VIF_Parser() {
									IsVIF1 = true
								};
								VIFProgram = FileFactory.Read<VIF_Commands>(c, file.FilePath, (_, v) => v.Pre_Parser = parserPass1);
							} finally {
								c.RemoveFile(file);
							}
							// Execute VIF program
							if (VIFProgram != null) {
								var s = c.Deserializer;
								var parser = new VIF_Parser() {
									IsVIF1 = true
								};
								var mcIndex = 0;
								PS2_VU_Data ExecuteMicroProgram() {
									parser.HasPendingChanges = false;
									byte[] microProg = parser.GetCurrentBuffer();
									if (microProg != null) {
										//Util.ByteArrayToFile(Path.Combine(testPath, $"{jadeGao.Key}__{geo.GRO.Object.Key}__{elementIndex}-{subElementIndex}-{vifProgramIndex}_DMA{curDMAId}_{mcIndex}.bin"), microProg);
										var mcKey = $"{chainProgKey}_{mcIndex}";
										mcIndex++;
										try {
											file = new StreamFile(c, mcKey, new MemoryStream(microProg), endianness: Endian.Little);
											c.AddFile(file);
											//var tops = parser.TOPS * 16;

											s.Goto(file.StartPointer);

											var vuData = s.SerializeObject<PS2_VU_Data>(default,
												onPreSerialize: v => {
													v.Pre_DMAId = curDMAId;
													v.Pre_HasBones = hasBones;
												},
												name: nameof(PS2_VU_Data));
											return vuData;
										} finally {
											c.RemoveFile(mcKey);
										}
									}
									return null;
								}
								foreach (var cmd in VIFProgram.Commands) {
									if (parser.StartsNewMicroProgram(cmd) && parser.HasPendingChanges) {
										var vuData = ExecuteMicroProgram();
										if (vuData != null) yield return vuData;
									}
									parser.ExecuteCommand(cmd, executeFull: true);
								}
								if (parser.HasPendingChanges) {
									var vuData = ExecuteMicroProgram();
									if (vuData != null) yield return vuData;
								}
							}
						}
					}

					int vifProgramIndex = 0;


					List<Jade_Vector> currentVertices = new List<Jade_Vector>();
					List<Jade_Vector> currentNormals = new List<Jade_Vector>();
					List<GEO_GeometricObject.UV> currentUVs = new List<GEO_GeometricObject.UV>();
					List<Jade_Color> currentColors = new List<Jade_Color>();
					List<GEO_GeometricObjectElement.Triangle> currentTriangles = new List<GEO_GeometricObjectElement.Triangle>();

					bool isFillingVertices = true;
					bool isFillingUVs = true;
					bool isFillingNormals = true;
					bool isFillingColors = true;
					bool isFillingTriangles = true;

					foreach (var vifProg in EnumerateVIFPrograms()) {
						int triIndex = 0;
						int verticesCount = 0;
						int uvsCount = 0;
						foreach (var vuData in EnumerateVUData(vifProg, vifProgramIndex)) {
							int curCount = vuData.GIFTag.NLOOP;
							int verticesCountStart = verticesCount;
							int uvsCountStart = uvsCount;

							// Vertices
							if (isFillingVertices) {
								currentVertices.AddRange(vuData.Vertices.Take(curCount).Select(v => new Jade_Vector(v.X, v.Y, v.Z)));
							}
							verticesCount += curCount;
							
							// UVs
							if (vuData.UVs != null && isFillingUVs) {
								currentUVs.AddRange(vuData.UVs.Take(curCount).Select(v => new GEO_GeometricObject.UV(v.UFloat, v.VFloat)));
							}
							if (vuData.UVs != null) uvsCount += curCount;

							// Normals
							if (vuData.Normals != null && isFillingNormals) {
								currentNormals.AddRange(vuData.Normals.Take(curCount).Select(v => new Jade_Vector(v.X, v.Y, v.Z)));
							}

							// Colors
							if (vuData.Colors != null && isFillingColors) {
								currentColors.AddRange(vuData.Colors.Take(curCount).Select(v => new Jade_Color(v.Red, v.Green, v.Blue, v.Alpha)));
							}

							// Triangles
							for (int i = 0; i < curCount; i++) {
								//bool order = i % 2 == 0;//vuData.Vertices[i].F < 0;
								int isNotInStrip = vuData.UVs?[i]?.IsNotIncludedInStrip
									?? vuData.Normals?[i]?.IsNotIncludedInStrip
									?? ((i >= 2) ? 0 : 1);
								if (isNotInStrip == 0) {
									if (isFillingTriangles && (verticesCountStart + (i - 2) < 0)) {
										throw new System.Exception($"{jadeGao.Key}: Incorrect triangle strip. See GEO_GaoVisu_PS2");
									}
									if (isFillingTriangles) {
										var tri = new GEO_GeometricObjectElement.Triangle();
										tri.SmoothingGroup = (uint)subElementIndex;
										tri.Vertex0 = (ushort)(verticesCountStart + (i - 2));
										tri.Vertex1 = (ushort)(verticesCountStart + (i - 1));
										tri.Vertex2 = (ushort)(verticesCountStart + (i - 0));
										if (vuData.UVs != null) {
											tri.UV0 = (ushort)(uvsCountStart + (i - 2));
											tri.UV1 = (ushort)(uvsCountStart + (i - 1));
											tri.UV2 = (ushort)(uvsCountStart + (i - 0));
										}
										currentTriangles.Add(tri);

										tri = new GEO_GeometricObjectElement.Triangle() {
											SmoothingGroup = tri.SmoothingGroup,
											Vertex0 = tri.Vertex0,
											Vertex1 = tri.Vertex2,
											Vertex2 = tri.Vertex1,
											UV0 = tri.UV0,
											UV1 = tri.UV2,
											UV2 = tri.UV1,
										};
										currentTriangles.Add(tri);
									} else if (isFillingUVs) {
										var tri = currentTriangles[triIndex];
										if (vuData.UVs != null) {
											tri.UV0 = (ushort)(uvsCountStart + (i - 2));
											tri.UV1 = (ushort)(uvsCountStart + (i - 1));
											tri.UV2 = (ushort)(uvsCountStart + (i - 0));
										}
										tri = currentTriangles[triIndex + 1];
										if (vuData.UVs != null) {
											tri.UV0 = (ushort)(uvsCountStart + (i - 2));
											tri.UV2 = (ushort)(uvsCountStart + (i - 0));
											tri.UV1 = (ushort)(uvsCountStart + (i - 1));
										}
									}
									triIndex += 2;
								}
							}
						}

						if(verticesCount > 0) isFillingVertices = false;
						if(uvsCount > 0) isFillingUVs = false;
						if(currentNormals.Any()) isFillingNormals = false;
						if(currentColors.Any()) isFillingColors = false;
						if(currentTriangles.Any()) isFillingTriangles = false;

						vifProgramIndex++;
					}

					int totalVerticesCount = vertices.Count;
					int totalUVsCount = uvs.Count;
					int currentUVsCount = currentUVs.Count;
					vertices.AddRange(currentVertices);
					normals.AddRange(currentNormals);
					uvs.AddRange(currentUVs);
					colors.AddRange(currentColors);
					foreach (var tri in currentTriangles) {
						if (currentUVsCount > 0) {
							tri.UV0 += (ushort)totalUVsCount;
							tri.UV1 += (ushort)totalUVsCount;
							tri.UV2 += (ushort)totalUVsCount;
						}
						tri.Vertex0 += (ushort)totalVerticesCount;
						tri.Vertex1 += (ushort)totalVerticesCount;
						tri.Vertex2 += (ushort)totalVerticesCount;
					}
					triangles.AddRange(currentTriangles);
				}
				GEO_GeometricObjectElement element = new GEO_GeometricObjectElement() {
					GeometricObject = geo,
					MaterialID = obj.Elements_MaterialId[elementIndex],
					Triangles = triangles.ToArray()

				};
				element.TrianglesCount = (uint)element.Triangles.Length;
				elements.Add(element);
			}
			geo.Elements = elements.ToArray();
			geo.ElementsCount = (uint)geo.Elements.Length;
			geo.Vertices = vertices.ToArray();
			geo.VerticesCount = (uint)geo.Vertices.Length;
			geo.UVs = uvs.ToArray();
			geo.UVsCount = (uint)geo.UVs.Length;
			if (normals.Count > 0) {
				geo.Normals = normals.ToArray();
				geo.Montreal_HasNormals = 1;
			}
			if (colors.Count > 0) {
				if (jadeGao?.Base?.Visual != null) {
					jadeGao.Base.Visual.VertexColors = colors.ToArray();
					jadeGao.Base.Visual.VertexColorsCount = (uint)jadeGao.Base.Visual.VertexColors.Length;
				} else {
					geo.Colors = colors.ToArray();
					geo.Montreal_HasColors = 1;
					geo.ColorsCount = (uint)geo.Colors.Length;
				}
			}
		}
		#endregion

		#region Serialize elements
		public class Element : BinarySerializable {
			public Jade_Code Foceface { get; set; }
			public uint Count { get; set; }
			public MeshElementRef[] Refs { get; set; }
			public Jade_Code Deadbeef { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Foceface = s.Serialize<Jade_Code>(Foceface, name: nameof(Foceface));
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Refs = s.SerializeObjectArray<MeshElementRef>(Refs, Count, name: nameof(Refs));
				Deadbeef = s.Serialize<Jade_Code>(Deadbeef, name: nameof(Deadbeef));
			}
		}

		public class MeshElementRef : BinarySerializable {
			public uint InstanceVIFProgramsCount { get; set; }
			public PS2_DMAChainData[] InstanceVIFPrograms { get; set; }

			public uint DMAChainProgramsCount { get; set; }
			public PS2_DMAChainProgram[] DMAChainPrograms { get; set; }

			public uint GEProgramsCount { get; set; }
			public PSP_GEData[] GEPrograms { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				if (s.GetR1Settings().Platform == Platform.PS2) {
					InstanceVIFProgramsCount = s.Serialize<uint>(InstanceVIFProgramsCount, name: nameof(InstanceVIFProgramsCount));
					InstanceVIFPrograms = s.SerializeObjectArray<PS2_DMAChainData>(InstanceVIFPrograms, InstanceVIFProgramsCount, name: nameof(InstanceVIFPrograms));
					DMAChainProgramsCount = s.Serialize<uint>(DMAChainProgramsCount, name: nameof(DMAChainProgramsCount));
					DMAChainPrograms = s.SerializeObjectArray<PS2_DMAChainProgram>(DMAChainPrograms, DMAChainProgramsCount, name: nameof(DMAChainPrograms));
				} else if (s.GetR1Settings().Platform == Platform.PSP) {
					GEProgramsCount = s.Serialize<uint>(GEProgramsCount, name: nameof(GEProgramsCount));
					GEPrograms = s.SerializeObjectArray<PSP_GEData>(GEPrograms, GEProgramsCount, onPreSerialize: p => p.Pre_IsInstance = true, name: nameof(GEPrograms));
				}
			}
		}
		#endregion
	}
}
