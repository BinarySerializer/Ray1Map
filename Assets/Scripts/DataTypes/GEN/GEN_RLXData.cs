using System.Linq;
using System.Text;
using BinarySerializer;
using UnityEngine;

namespace R1Engine {
	public class GEN_RLXData : BinarySerializable {
		public byte RLXType { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public ushort X { get; set; }
		public ushort Y { get; set; }
		public uint? FileSize { get; set; }
		public byte LookupTableCount { get; set; }
		public byte[] LookupTable { get; set; }

		public uint DataLength { get; set; }
		public byte[] Data { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			RLXType = s.Serialize<byte>(RLXType, name: nameof(RLXType));
			Width = s.Serialize<ushort>(Width, name: nameof(Width));
			Height = s.Serialize<ushort>(Height, name: nameof(Height));
			X = s.Serialize<ushort>(X, name: nameof(X));
			Y = s.Serialize<ushort>(Y, name: nameof(Y));
			if (RLXType < 7) {
				if (RLXType > 1) {
					LookupTableCount = s.Serialize<byte>(LookupTableCount, name: nameof(LookupTableCount));
					LookupTable = s.SerializeArray<byte>(LookupTable, LookupTableCount, name: nameof(LookupTable));
				}
				DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
				Data = s.DoEncoded(new GEN_RLXEncoder(this), () => s.SerializeArray<byte>(Data, s.CurrentLength, name: nameof(Data)));
				/*if (RLXType == 2) {
					Texture2D tex = ToTexture2D();
					Util.ByteArrayToFile($"{Context.BasePath}/temp_images/{Offset.File.FilePath}_{Offset.StringFileOffset}.png", tex.EncodeToPNG());
					//Util.ByteArrayToFile($"{Context.BasePath}/temp_ext/{Offset.File.FilePath}_{Offset.StringFileOffset}.bin", Data);
				}*/
			} else {
				DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
				Data = s.SerializeArray<byte>(Data, DataLength, name: nameof(Data));
			}
		}

		public Texture2D ToTexture2D(BaseColor[] palette) {
			var pal = palette.Select(p => {
				Color c = p.GetColor();
				c.a = 1f;
				return c;
			}).ToArray();
			var tex = Util.ToTileSetTexture(Data, pal, Util.TileEncoding.Linear_8bpp, 1, true, wrap: Width);
			return tex;
		}
	}
}