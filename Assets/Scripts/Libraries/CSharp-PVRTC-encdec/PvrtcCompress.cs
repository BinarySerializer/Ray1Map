using System;

namespace CSharp_PVRTC_EncDec
{
	public static class PvrtcCompress  
	{
		private static (byte[] minColor, byte[] maxColor) GetMinMaxColors(TempByteImageFormat bitmap, int startX, int startY)
		{
			byte[] minColor = CommonColors.GetAllMax(3);
			byte[] maxColor = CommonColors.GetAllMin(3);

			for (int x = startX; x < startX + 4; x++)
			{
				for (int y = startY; y < startY + 4; y++) 
				{
					byte[] currentColor = bitmap.GetPixelChannels(x, y);
					if ( currentColor[0] < minColor[0] ) { minColor[0] = currentColor[0]; }
					if ( currentColor[1] < minColor[1] ) { minColor[1] = currentColor[1]; }
					if ( currentColor[2] < minColor[2] ) { minColor[2] = currentColor[2]; }
					if ( currentColor[0] > maxColor[0] ) { maxColor[0] = currentColor[0]; }
					if ( currentColor[1] > maxColor[1] ) { maxColor[1] = currentColor[1]; }
					if ( currentColor[2] > maxColor[2] ) { maxColor[2] = currentColor[2]; }
				}
			}

			return (minColor, maxColor);
		}

		private static (byte[] minColor, byte[] maxColor) GetMinMaxColorsWithAlpha(TempByteImageFormat bitmap, int startX, int startY)
		{
			byte[] minColor = CommonColors.GetAllMax(4);
			byte[] maxColor = CommonColors.GetAllMin(4);

			for (int x = startX; x < startX + 4; x++)
			{
				for (int y = startY; y < startY + 4; y++)
				{
					byte[] currentColor = bitmap.GetPixelChannels(x, y);
					if ( currentColor[0] < minColor[0] ) { minColor[0] = currentColor[0]; }
					if ( currentColor[1] < minColor[1] ) { minColor[1] = currentColor[1]; }
					if ( currentColor[2] < minColor[2] ) { minColor[2] = currentColor[2]; }
					if ( currentColor[3] < minColor[3] ) { minColor[3] = currentColor[3]; }
					if ( currentColor[0] > maxColor[0] ) { maxColor[0] = currentColor[0]; }
					if ( currentColor[1] > maxColor[1] ) { maxColor[1] = currentColor[1]; }
					if ( currentColor[2] > maxColor[2] ) { maxColor[2] = currentColor[2]; }
					if ( currentColor[3] > maxColor[3] ) { maxColor[3] = currentColor[3]; }
				}
			}
			return (minColor, maxColor);
		}

		/// <summary>
		/// Encodes RGBA texture into byte array
		/// </summary>
		/// <remarks>Texture must be square and power of two dimensions</remarks>
		/// <param name="bitmap">TempByteImageFormat</param>
		/// <returns>Byte array</returns>
		public static byte[] EncodeRgba4Bpp(TempByteImageFormat bitmap)
		{
			if (bitmap.height != bitmap.width) 
			{
				throw new ArgumentException("Texture isn't square!");
			}

			if (!((bitmap.height & (bitmap.height - 1)) == 0)) 
			{
				throw new ArgumentException("Texture resolution must be 2^N!");
			}

			int size = bitmap.width;
			int blocks = size / 4;
			int blockMask = blocks-1;
			
			PvrtcPacket[] packets = new PvrtcPacket[blocks * blocks];
			for (int i = 0; i < packets.Length; i++)
			{
				packets[i] = new PvrtcPacket();
			}
			
			for (int y = 0; y < blocks; ++y)
			{
				for (int x = 0; x < blocks; ++x)
				{
					(byte[] minColor, byte[] maxColor) = GetMinMaxColorsWithAlpha(bitmap, 4*x, 4*y);				

					PvrtcPacket packet = packets[MortonTable.GetMortonNumber(x, y)];
					packet.SetPunchthroughAlpha(false);
					packet.SetColorA(minColor[0], minColor[1], minColor[2], minColor[3]);
					packet.SetColorB(maxColor[0], maxColor[1], maxColor[2], maxColor[3]);
				}
			}

			int currentFactorIndex = 0;
			
			for (int y = 0; y < blocks; ++y)
			{
				for (int x = 0; x < blocks; ++x)
				{
					currentFactorIndex = 0;
					
					uint modulationData = 0;
					
					for(int py = 0; py < 4; ++py)
					{
						int yOffset = (py < 2) ? -1 : 0;
						int y0 = (y + yOffset) & blockMask;
						int y1 = (y0+1) & blockMask;
						
						for(int px = 0; px < 4; ++px)
						{
							int xOffset = (px < 2) ? -1 : 0;
							int x0 = (x + xOffset) & blockMask;
							int x1 = (x0+1) & blockMask;
							
							PvrtcPacket p0 = packets[MortonTable.GetMortonNumber(x0, y0)];
							PvrtcPacket p1 = packets[MortonTable.GetMortonNumber(x1, y0)];
							PvrtcPacket p2 = packets[MortonTable.GetMortonNumber(x0, y1)];
							PvrtcPacket p3 = packets[MortonTable.GetMortonNumber(x1, y1)];

							byte[] currentFactors = PvrtcPacket.BILINEAR_FACTORS[currentFactorIndex];
							
							Vector4Int ca = 	p0.GetColorRgbaA() * currentFactors[0] +
												p1.GetColorRgbaA() * currentFactors[1] +
												p2.GetColorRgbaA() * currentFactors[2] +
												p3.GetColorRgbaA() * currentFactors[3];
							
							Vector4Int cb = 	p0.GetColorRgbaB() * currentFactors[0] +
												p1.GetColorRgbaB() * currentFactors[1] +
												p2.GetColorRgbaB() * currentFactors[2] +
												p3.GetColorRgbaB() * currentFactors[3];
							
							byte[] pixel = bitmap.GetPixelChannels(4*x + px, 4*y + py);
							Vector4Int d = cb - ca;
							Vector4Int p = new Vector4Int(pixel[0]*16, pixel[1]*16, pixel[2]*16, pixel[3]*16);
							Vector4Int v = p - ca;
							
							// PVRTC uses weightings of 0, 3/8, 5/8 and 1
							// The boundaries for these are 3/16, 1/2 (=8/16), 13/16
							int projection = (v % d) * 16; //Mathf.RoundToInt(Vector4.Dot(v, d)) * 16;
							int lengthSquared = d % d; //Mathf.RoundToInt(Vector4.Dot(d,d));
							if(projection > 3*lengthSquared) modulationData++;
							if(projection > 8*lengthSquared) modulationData++;
							if(projection > 13*lengthSquared) modulationData++;
							
							modulationData = RotateRight(modulationData, 2);
							
							currentFactorIndex++;
						}
					}
					
					PvrtcPacket packet = packets[MortonTable.GetMortonNumber(x, y)];
					packet.SetModulationData(modulationData);
				}
			}

			byte[] returnValue = new byte[size * size /2];

			// Create final byte array from PVRTC packets
			for (int i = 0; i < packets.Length; i++)
			{
				byte[] tempArray = packets[i].GetAsByteArray();
				Buffer.BlockCopy(tempArray, 0, returnValue, 8*i, 8);
			}
			
			return returnValue;
		}

		/// <summary>
		/// Encodes RGB texture into byte array
		/// </summary>
		/// <remarks>Texture must be square and power of two dimensions</remarks>
		/// <param name="bitmap">TempByteImageFormat</param>
		/// <returns>Byte array</returns>
		public static byte[] EncodeRgb4Bpp(TempByteImageFormat bitmap)
		{
			if (bitmap.height != bitmap.width) 
			{
				throw new ArgumentException("Texture isn't square!");
			}

			if (!((bitmap.height & (bitmap.height - 1)) == 0)) 
			{
				throw new ArgumentException("Texture resolution must be 2^N!");
			}

			int size = bitmap.width;
			int blocks = size / 4;
			int blockMask = blocks-1;
			
			PvrtcPacket[] packets = new PvrtcPacket[blocks * blocks];
			for (int i = 0; i < packets.Length; i++)
			{
				packets[i] = new PvrtcPacket();
			}

			for(int y = 0; y < blocks; ++y)
			{
				for(int x = 0; x < blocks; ++x)
				{
					(byte[] minColor, byte[] maxColor) = GetMinMaxColors(bitmap, 4*x, 4*y);

					PvrtcPacket packet = packets[MortonTable.GetMortonNumber(x, y)];
					packet.SetPunchthroughAlpha(false);
					packet.SetColorA(minColor[0], minColor[1], minColor[2]);
					packet.SetColorB(maxColor[0], maxColor[1], maxColor[2]);
				}
			}

			int currentFactorIndex = 0;
			
			for(int y = 0; y < blocks; ++y)
			{
				for(int x = 0; x < blocks; ++x)
				{
					currentFactorIndex = 0;
					
					uint modulationData = 0;
					
					for(int py = 0; py < 4; ++py)
					{
						int yOffset = (py < 2) ? -1 : 0;
						int y0 = (y + yOffset) & blockMask;
						int y1 = (y0+1) & blockMask;
						
						for(int px = 0; px < 4; ++px)
						{
							int xOffset = (px < 2) ? -1 : 0;
							int x0 = (x + xOffset) & blockMask;
							int x1 = (x0+1) & blockMask;
							
							PvrtcPacket p0 = packets[MortonTable.GetMortonNumber(x0, y0)];
							PvrtcPacket p1 = packets[MortonTable.GetMortonNumber(x1, y0)];
							PvrtcPacket p2 = packets[MortonTable.GetMortonNumber(x0, y1)];
							PvrtcPacket p3 = packets[MortonTable.GetMortonNumber(x1, y1)];

							byte[] currentFactors = PvrtcPacket.BILINEAR_FACTORS[currentFactorIndex];

							Vector3Int ca = 	p0.GetColorRgbA() * currentFactors[0] +
												p1.GetColorRgbA() * currentFactors[1] +
												p2.GetColorRgbA() * currentFactors[2] +
												p3.GetColorRgbA() * currentFactors[3];
							
							Vector3Int cb = 	p0.GetColorRgbB() * currentFactors[0] +
												p1.GetColorRgbB() * currentFactors[1] +
												p2.GetColorRgbB() * currentFactors[2] +
												p3.GetColorRgbB() * currentFactors[3];
							
							byte[] pixel = bitmap.GetPixelChannels(4*x + px, 4*y + py);

							Vector3Int d = cb - ca;
							Vector3Int p = new Vector3Int(pixel[0]*16, pixel[1]*16, pixel[2]*16);
							Vector3Int v = p - ca;
							
							// PVRTC uses weightings of 0, 3/8, 5/8 and 1
							// The boundaries for these are 3/16, 1/2 (=8/16), 13/16
							int projection = (v % d) * 16; // Mathf.RoundToInt(Vector3.Dot(v, d)) * 16;
							int lengthSquared = d % d;//Mathf.RoundToInt(Vector3.Dot(d,d));
							if(projection > 3*lengthSquared) modulationData++;
							if(projection > 8*lengthSquared) modulationData++;
							if(projection > 13*lengthSquared) modulationData++;

							modulationData = RotateRight(modulationData, 2);

							currentFactorIndex++;
						}
					}
					
					PvrtcPacket packet = packets[MortonTable.GetMortonNumber(x, y)];
					packet.SetModulationData(modulationData);
				}
			}

			byte[] returnValue = new byte[size * size /2];

			// Create final byte array from PVRTC packets
			for (int i = 0; i < packets.Length; i++)
			{
				byte[] tempArray = packets[i].GetAsByteArray();
				Buffer.BlockCopy(tempArray, 0, returnValue, 8*i, 8);
			}

			return returnValue;
		}

		private static uint RotateRight(uint value, int count)
		{
			return (value >> count) | (value << (32 - count));
		}
	}
}