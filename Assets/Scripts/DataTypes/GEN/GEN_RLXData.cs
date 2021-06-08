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
		public byte UnknownBytesCount { get; set; }
		public byte[] UnknownBytes { get; set; }

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
					UnknownBytesCount = s.Serialize<byte>(UnknownBytesCount, name: nameof(UnknownBytesCount));
					UnknownBytes = s.SerializeArray<byte>(UnknownBytes, UnknownBytesCount, name: nameof(UnknownBytes));
				}
				DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
				Data = s.DoEncoded(new GEN_RLXEncoder(this), () => s.SerializeArray<byte>(Data, s.CurrentLength, name: nameof(Data)));
				if (RLXType == 2) {
					Texture2D tex = ToTexture2D();
					Util.ByteArrayToFile($"{Context.BasePath}/temp_images/{Offset.File.FilePath}_{Offset.StringFileOffset}.png", tex.EncodeToPNG());
					//Util.ByteArrayToFile($"{Context.BasePath}/temp_ext/{Offset.File.FilePath}_{Offset.StringFileOffset}.bin", Data);
				}
			} else {
				DataLength = FileSize.Value - (uint)(s.CurrentPointer - Offset);
				Data = s.SerializeArray<byte>(Data, DataLength, name: nameof(Data));
			}
		}

		public Texture2D ToTexture2D() {
			Texture2D tex = TextureHelpers.CreateTexture2D(Width, Height);
			var pal = Util.CreateDummyPalette(256, false);
			var data = Data.Select(d => pal[d].GetColor()).ToArray();
			tex.SetPixels(data);
			tex.Apply();
			return tex;
		}
	}
}