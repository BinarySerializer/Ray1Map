using System;

namespace CSharp_PVRTC_EncDec
{
	public static class PvrtcDecompress 
	{
		/// <summary>
		/// Decode 4 bit RGB texture from byte array
		/// </summary>
		/// <remarks>Assumes that input texture is square! (width == height)</remarks>
		/// <param name="data">Byte array that contains encoded texture data</param>
		/// <param name="width">Width of texture in pixels</param>
		/// <returns>TempByteImageFormat</returns>
		public static TempByteImageFormat DecodeRgb4Bpp(byte[] data, int width)
		{
			int size = width;
			int blocks = size / 4;
			int blockMask = blocks-1;

			TempByteImageFormat returnValue = new TempByteImageFormat(size, size, 3);

			PvrtcPacket[] packets = new PvrtcPacket[blocks * blocks];
			byte[] eightBytes = new byte[8];
			for (int i = 0; i < packets.Length; i++)
			{
				packets[i] = new PvrtcPacket();
				Buffer.BlockCopy(data, i*8, eightBytes, 0, 8);
				packets[i].InitFromBytes(eightBytes);
			}

			int currentFactorIndex = 0;
			
			for(int y = 0; y < blocks; ++y)
			{
				for(int x = 0; x < blocks; ++x)
				{
					currentFactorIndex = 0;

					PvrtcPacket packet = packets[MortonTable.GetMortonNumber(x, y)];
					
					uint mod = packet.GetModulationData();
					
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

							byte[] currentWeights = PvrtcPacket.WEIGHTS[4*packet.GetPunchthroughAlpha() + mod&3];

							
							byte red = (byte)((ca.x * currentWeights[0] + cb.x * currentWeights[1]) >> 7);
							byte green = (byte)((ca.y * currentWeights[0] + cb.y * currentWeights[1]) >> 7);
							byte blue = (byte)((ca.z * currentWeights[0] + cb.z * currentWeights[1]) >> 7);

							returnValue.SetPixelChannels((px+x*4), (py+y*4), new byte[] { red, green, blue });
							mod >>= 2;
							currentFactorIndex++;
						}
					}
				}
			}
			
			return returnValue;
		}

		/// <summary>
		/// Decode 4 bit RGBA texture from byte array
		/// </summary>
		/// <remarks>Assumes that input texture is square! (width == height)</remarks>
		/// <param name="data">Byte array that contains encoded texture data</param>
		/// <param name="width">Width of texture in pixels</param>
		/// <returns>TempByteImageFormat</returns>
		public static TempByteImageFormat DecodeRgba4Bpp(byte[] data, int width)
		{
			int size = width;
			int blocks = size / 4;
			int blockMask = blocks-1;

			TempByteImageFormat returnValue = new TempByteImageFormat(size, size, 4);

			PvrtcPacket[] packets = new PvrtcPacket[blocks * blocks];
			byte[] eightBytes = new byte[8];
			for (int i = 0; i < packets.Length; i++)
			{
				packets[i] = new PvrtcPacket();
				Buffer.BlockCopy(data, i*8, eightBytes, 0, 8);
				packets[i].InitFromBytes(eightBytes);
			}
			
			int currentFactorIndex = 0;
			
			for(int y = 0; y < blocks; ++y)
			{
				for(int x = 0; x < blocks; ++x)
				{
					currentFactorIndex = 0;
					
					PvrtcPacket packet = packets[MortonTable.GetMortonNumber(x, y)];
					
					uint mod = packet.GetModulationData();
					
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
							
							Vector4Int ca = p0.GetColorRgbaA() * currentFactors[0] +
											p1.GetColorRgbaA() * currentFactors[1] +
											p2.GetColorRgbaA() * currentFactors[2] +
											p3.GetColorRgbaA() * currentFactors[3];
							
							Vector4Int cb = p0.GetColorRgbaB() * currentFactors[0] +
											p1.GetColorRgbaB() * currentFactors[1] +
											p2.GetColorRgbaB() * currentFactors[2] +
											p3.GetColorRgbaB() * currentFactors[3];

							byte[] currentWeights = PvrtcPacket.WEIGHTS[4*packet.GetPunchthroughAlpha() + mod&3];

							byte red = (byte)((ca.x * currentWeights[0] + cb.x * currentWeights[1]) >> 7);
							byte green = (byte)((ca.y * currentWeights[0] + cb.y * currentWeights[1]) >> 7);
							byte blue = (byte)((ca.z * currentWeights[0] + cb.z * currentWeights[1]) >> 7);
							byte alpha = (byte)((ca.w * currentWeights[2] + cb.w * currentWeights[3]) >> 7);
							
							returnValue.SetPixelChannels((px+x*4), (py+y*4), new byte[] { red, green, blue, alpha });
							mod >>= 2;
							currentFactorIndex++;
						}
					}
				}
			}

			return returnValue;
		}
	}
}