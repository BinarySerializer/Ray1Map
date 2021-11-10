using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.Jade {
	public class GEO_GaoVisu_PS2 : BinarySerializable {
		public uint ElementsCount { get; set; }
		public Element[] Elements { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
			Elements = s.SerializeObjectArray<Element>(Elements, ElementsCount, name: nameof(Elements));
		}

		public GameObject ExecuteChainPrograms(GEO_GeoObject_PS2 obj, int elementIndex, int subElementIndex) {
			Element visuElement = (Elements.Length > elementIndex) ? Elements[elementIndex] : null;
			GEO_GeoObject_PS2.ElementDataBuffer.ElementData objElement = (obj.ElementData.ElementDatas.Length > elementIndex ) ? obj.ElementData.ElementDatas[elementIndex] : null;
			MeshElementRef visuSubElement = ((visuElement?.Refs?.Length ?? 0) > subElementIndex) ? visuElement.Refs[subElementIndex] : null;
			GEO_GeoObject_PS2.ElementDataBuffer.MeshElement objSubElement = ((objElement?.MeshElements?.Length ?? 0) > subElementIndex) ? objElement.MeshElements[subElementIndex] : null;

			GameObject gao = new GameObject($"{Offset}");

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
			// TODO: Write each ChainData element with Negative ID as a VIF program
			if (chainPrograms.Any() ) {
				chainPrograms.Sort((x,y) => x.ID.CompareTo(y.ID));
				foreach (var prog in chainPrograms) {
					if (prog.Commands.Length > 0) {
						using (MemoryStream ms = new MemoryStream()) {
							using (Writer w = new Writer(ms, isLittleEndian: true, leaveOpen: true)) {
								foreach (var c in prog.Commands) {
									c.Transfer(w, chainData);
								}
							}

							PS2_GeometryCommand[] VIFProgram = null;
							using (Context c = new Context("", serializerLog: new ParentContextLog(Context.Log))) {
								ms.Position = 0;
								var file = c.AddStreamFile($"ChainProgram__{Offset}__{elementIndex}-{subElementIndex}", ms);
								var s = c.Deserializer;
								s.DoAt(file.StartPointer, () => {
									// Parse ms into a VIF program here
									VIFProgram = s.SerializeObjectArrayUntil<PS2_GeometryCommand>(VIFProgram, g => s.CurrentAbsoluteOffset >= file.Length, name: nameof(VIFProgram));
								});
							}
							// TODO: Execute VIF program
							if (VIFProgram != null) {
								PS2_GeometryCommand vertices = null;
								PS2_GeometryCommand uvs = null;
								foreach (var cmd in VIFProgram) {
									if (cmd.Type == PS2_GeometryCommand.CommandType.Vertices) vertices = cmd;
									if (cmd.Type == PS2_GeometryCommand.CommandType.UVs) uvs = cmd;
									if (cmd.Type == PS2_GeometryCommand.CommandType.TransferData) {
										if (vertices != null) {
											var verts = vertices.V3_VL32;
											var uv = uvs.UV3;
											List<Vector3> unityVerts = new List<Vector3>();

											Vector3 getVector3(PS2_Vector3_32 v) {
												return new Vector3(v.X, v.Z, v.Y);
											}
											int numTriangle = 0;
											for(int i = 0; i < verts.Length; i++) {
												if(uv[i].Z == 1) continue;
												if (numTriangle % 2 == 1) {
													unityVerts.Add(getVector3(verts[i - 2]));
													unityVerts.Add(getVector3(verts[i - 1]));
													unityVerts.Add(getVector3(verts[i - 0]));
													unityVerts.Add(getVector3(verts[i - 1]));
													unityVerts.Add(getVector3(verts[i - 2]));
													unityVerts.Add(getVector3(verts[i - 0]));
												} else {
													unityVerts.Add(getVector3(verts[i - 1]));
													unityVerts.Add(getVector3(verts[i - 2]));
													unityVerts.Add(getVector3(verts[i - 0]));
													unityVerts.Add(getVector3(verts[i - 2]));
													unityVerts.Add(getVector3(verts[i - 1]));
													unityVerts.Add(getVector3(verts[i - 0]));
												}
												numTriangle++;
											}
											int[] tris = Enumerable.Range(0, numTriangle * 6).ToArray();

											Mesh m = new Mesh();
											m.vertices = unityVerts.ToArray();
											m.triangles = tris;
											m.RecalculateNormals();

											GameObject g_geo_e = new GameObject($"Element");
											g_geo_e.transform.SetParent(gao.transform, false);
											g_geo_e.layer = LayerMask.NameToLayer("3D Collision");
											MeshFilter mf = g_geo_e.AddComponent<MeshFilter>();
											mf.mesh = m;
											MeshRenderer mr = g_geo_e.AddComponent<MeshRenderer>();
											mr.material = Controller.obj.levelController.controllerTilemap.isometricCollisionMaterial;

											vertices = null;
										}
									}
								}
							}
						}
					}
				}
			} else {
				/*if (chainData.Any()) {
					throw new Exception($"GAO Visu {Offset} with object {obj} for element indices {elementIndex}-{subElementIndex} doesn't have any chain programs!");
				}*/
			}
			return gao;
		}

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

			public override void SerializeImpl(SerializerObject s) {
				InstanceVIFProgramsCount = s.Serialize<uint>(InstanceVIFProgramsCount, name: nameof(InstanceVIFProgramsCount));
				InstanceVIFPrograms = s.SerializeObjectArray<PS2_DMAChainData>(InstanceVIFPrograms, InstanceVIFProgramsCount, onPreSerialize: p => p.Pre_IsInstance = true, name: nameof(InstanceVIFPrograms));
				if (s.GetR1Settings().Platform == Platform.PS2) {
					DMAChainProgramsCount = s.Serialize<uint>(DMAChainProgramsCount, name: nameof(DMAChainProgramsCount));
					DMAChainPrograms = s.SerializeObjectArray<PS2_DMAChainProgram>(DMAChainPrograms, DMAChainProgramsCount, name: nameof(DMAChainPrograms));
				}
			}
		}
	}
}
