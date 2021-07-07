namespace CSharp_PVRTC_EncDec
{

	public readonly struct Vector4Int {

		public readonly int x;
		public readonly int y;
		public readonly int z;
		public readonly int w;

		public Vector4Int(int xx, int yy, int zz, int ww)
		{
			this.x = xx;
			this.y = yy;
			this.z = zz;
			this.w = ww;
		}

		public static Vector4Int operator *(Vector4Int a, int b)
		{
			return new Vector4Int (a.x * b, a.y * b, a.z * b, a.w * b);
		}

		public static Vector4Int operator +(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int (a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
		}

		public static Vector4Int operator -(Vector4Int a, Vector4Int b)
		{
			return new Vector4Int (a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
		}

		public static int operator %(Vector4Int a, Vector4Int b)
		{
			return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
		}
	}
}