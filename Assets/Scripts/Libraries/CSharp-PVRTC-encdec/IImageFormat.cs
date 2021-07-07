namespace CSharp_PVRTC_EncDec
{
	/// <summary>
	/// Interface for our custom image formats
	/// </summary>
	public interface IImageFormat
	{
		/// <summary>
		/// Get width
		/// </summary>
		/// <returns>Width of image</returns>
		int GetWidth();

		/// <summary>
		/// Get height
		/// </summary>
		/// <returns>Height of image</returns>
		int GetHeight();

		/// <summary>
		/// Set pixel channels of certain coordinate
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="newValues">New values as byte array</param>
		void SetPixelChannels(int x, int y, byte[] newValues);

		/// <summary>
		/// Get pixel channels of certain coordinate
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>Values as byte array</returns>
		byte[] GetPixelChannels(int x, int y);
	}
}