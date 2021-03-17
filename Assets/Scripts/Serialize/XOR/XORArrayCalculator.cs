namespace R1Engine
{
	/// <summary>
	/// Class for XOR operations with a multi-byte key
	/// </summary>
	public class XORArrayCalculator : IXORCalculator {
		public byte[] Key { get; set; }
		public int CurrentKeyByte { get; set; }

		public XORArrayCalculator(byte[] key, int currentByte = 0) {
			Key = key;
			CurrentKeyByte = currentByte % key.Length;
		}
		public byte XORByte(byte b) {
			var key = Key[CurrentKeyByte];
			CurrentKeyByte = (CurrentKeyByte+1) % Key.Length;
			return (byte)(b ^ key);
		}
	}
}