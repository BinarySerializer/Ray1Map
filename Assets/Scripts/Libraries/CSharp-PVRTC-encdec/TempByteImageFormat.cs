
namespace CSharp_PVRTC_EncDec
{

	/// <summary>
	/// Temp byte based image format. 0 is zero color, 255 is max color
	/// </summary>
	public sealed class TempByteImageFormat : IImageFormat
	{
		/// <summary>
		/// Width of bitmap
		/// </summary>
		public readonly int width;

		/// <summary>
		/// Height of bitmap
		/// </summary>
		public readonly int height;

		private readonly byte[,,] content;

		/// <summary>
		/// How many color channels per pixel
		/// </summary>
		public readonly int channelsPerPixel;

		/// <summary>
		/// Constructor for temp byte image format
		/// </summary>
		/// <param name="input">Input bitmap as three dimensional (widht, height, channels per pixel) byte array</param>
		public TempByteImageFormat(byte[,,] input)
		{
			this.content = input;
			this.width = input.GetLength(0);
			this.height = input.GetLength(1);
			this.channelsPerPixel = input.GetLength(2);
		}

		public TempByteImageFormat(int w, int h, int channels)
		{
			this.width = w;
			this.height = h;
			this.channelsPerPixel = channels;
			this.content = new byte[w, h, channels];
		}

		/// <summary>
		/// Constructor for temp byte image format
		/// </summary>
		/// <param name="input">Existing TempByteImageFormat</param>
		public TempByteImageFormat(TempByteImageFormat input) : this(input.content)
		{

		}

		/// <summary>
		/// Get width of bitmap
		/// </summary>
		/// <returns>Width in pixels</returns>
		public int GetWidth()
		{
			return this.width;
		}    
		
		/// <summary>
		/// Get height of bitmap
		/// </summary>
		/// <returns>Height in pixels</returns>
		public int GetHeight()
		{
			return this.height;
		}

		public int GetChannelsPerPixel()
		{
			return this.channelsPerPixel;
		}

		/// <summary>
		/// Set pixel channels of certain coordinate
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="newValues">New values as object array</param>
		public void SetPixelChannels(int x, int y, byte[] newValues)
		{
			for (int i = 0; i < this.channelsPerPixel; i++)
			{
				this.content[x, y, i] = newValues[i];
			}
		}

		/// <summary>
		/// Get pixel channels of certain coordinate
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>Values as object array</returns>
		public byte[] GetPixelChannels(int x, int y)
		{
			byte[] returnArray = new byte[this.channelsPerPixel];

			for (int i = 0; i < this.channelsPerPixel; i++)
			{
				returnArray[i] = this.content[x, y, i];
			}

			return returnArray;
		}

		public byte GetPixelChannel(int x, int y, int i) {
			return content[x, y, i];
		}
	}
}