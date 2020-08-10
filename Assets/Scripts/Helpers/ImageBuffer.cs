using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;

namespace R1Engine {
	public class ImageBuffer {
		private byte[] imageBuffer;
		public void AddData(byte[] data) {
			if (imageBuffer == null) {
				imageBuffer = new byte[data.Length];
			} else {
				Array.Resize(ref imageBuffer, imageBuffer.Length + data.Length);
			}
			Array.Copy(data, 0, imageBuffer, imageBuffer.Length - data.Length, data.Length);
		}

		public byte GetPixel8(uint index) {
			return imageBuffer[index];
		}
	}
}
