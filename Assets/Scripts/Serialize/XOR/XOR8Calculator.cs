namespace R1Engine
{
	/// <summary>
	/// Class for basic XOR operations with a single byte key
	/// </summary>
	public class XOR8Calculator : IXORCalculator {
		public byte Key { get; set; }
		public XOR8Calculator(byte key) {
			Key = key;
		}
		public byte XORByte(byte b) {
			return (byte)(b ^ Key);
		}
	}
}