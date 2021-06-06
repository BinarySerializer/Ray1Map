using System;
using System.Collections.Generic;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEX_GlobalList {
		public List<Jade_TextureReference> Textures { get; set; }
		public List<Jade_PaletteReference> Palettes { get; set; }
		public List<Jade_CubeMapReference> CubeMaps { get; set; }

		private Dictionary<Jade_Key, List<Jade_PaletteReference>> KeyPaletteDictionary { get; set; }
		private Dictionary<Jade_Key, List<Jade_TextureReference>> KeyTextureDictionary { get; set; }
		private Dictionary<Jade_Key, List<Jade_CubeMapReference>> KeyCubeMapDictionary { get; set; }
		public Dictionary<Jade_Key, Jade_Reference<STR_FontDescriptor>> FontDescriptors { get; set; } = new Dictionary<Jade_Key, Jade_Reference<STR_FontDescriptor>>(); // Texture key - font descriptor
		public void SortTexturesList_Montreal() {
			Textures.Sort((t1, t2) => t1.Key.Key.CompareTo(t2.Key.Key));
		}
		public void SortPalettesList_Montreal() {
			Palettes.Sort((p1, p2) => p1.Key.Key.CompareTo(p2.Key.Key));
		}
		public void AddTexture(Jade_TextureReference tex) {
			if (tex == null || tex.IsNull) return;
			if (Textures == null) Textures = new List<Jade_TextureReference>();
			if (KeyTextureDictionary == null) KeyTextureDictionary = new Dictionary<Jade_Key, List<Jade_TextureReference>>();
			if (Textures.FindItem(t => t.Key == tex.Key) == null) Textures.Add(tex);
			if (!KeyTextureDictionary.ContainsKey(tex.Key)) KeyTextureDictionary[tex.Key] = new List<Jade_TextureReference>();
			KeyTextureDictionary[tex.Key].Add(tex);
		}
		public bool ContainsTexture(Jade_TextureReference tex) {
			if (tex == null || tex.IsNull) return false;
			if (Textures == null) Textures = new List<Jade_TextureReference>();
			if (KeyTextureDictionary == null) KeyTextureDictionary = new Dictionary<Jade_Key, List<Jade_TextureReference>>();
			if (!KeyTextureDictionary.ContainsKey(tex.Key)) return false;
			return KeyTextureDictionary[tex.Key] != null;
		}
		public void AddPalette(Jade_PaletteReference pal) {
			if (pal == null || pal.IsNull) return;
			if (Palettes == null) Palettes = new List<Jade_PaletteReference>();
			if (KeyPaletteDictionary == null) KeyPaletteDictionary = new Dictionary<Jade_Key, List<Jade_PaletteReference>>();
			if (Palettes.FindItem(t => t.Key == pal.Key) == null) Palettes.Add(pal);
			if (!KeyPaletteDictionary.ContainsKey(pal.Key)) KeyPaletteDictionary[pal.Key] = new List<Jade_PaletteReference>();
			KeyPaletteDictionary[pal.Key].Add(pal);
		}
		public void AddCubeMap(Jade_CubeMapReference map) {
			if (map == null || map.IsNull) return;
			if (CubeMaps == null) CubeMaps = new List<Jade_CubeMapReference>();
			if (KeyCubeMapDictionary == null) KeyCubeMapDictionary = new Dictionary<Jade_Key, List<Jade_CubeMapReference>>();
			if (CubeMaps.FindItem(t => t.Key == map.Key) == null) CubeMaps.Add(map);
			if (!KeyCubeMapDictionary.ContainsKey(map.Key)) KeyCubeMapDictionary[map.Key] = new List<Jade_CubeMapReference>();
			KeyCubeMapDictionary[map.Key].Add(map);
		}

		public void FillInReferences() {
			if (Textures != null) {
				foreach (var t in Textures) {
					var list = KeyTextureDictionary[t.Key];
					for (int i = 1; i < list.Count; i++) {
						list[i].Info = t.Info;
						list[i].Content = t.Content;
					}
				}
			}
			if (Palettes != null) {
				foreach (var p in Palettes) {
					var list = KeyPaletteDictionary[p.Key];
					for (int i = 1; i < list.Count; i++) {
						list[i].Value = p.Value;
					}
				}
			}
			if (CubeMaps != null) {
				foreach (var c in CubeMaps) {
					var list = KeyCubeMapDictionary[c.Key];
					for (int i = 1; i < list.Count; i++) {
						list[i].Value = c.Value;
					}
				}
			}
		}
	}
}
