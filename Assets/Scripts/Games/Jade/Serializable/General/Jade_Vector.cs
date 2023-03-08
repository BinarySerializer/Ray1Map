using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class Jade_Vector : BinarySerializable, IEquatable<Jade_Vector>, ISerializerShortLog {
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			X = s.Serialize<float>(X, name: nameof(X));
			Y = s.Serialize<float>(Y, name: nameof(Y));
			Z = s.Serialize<float>(Z, name: nameof(Z));
		}

		public string ShortLog => ToString();
		public override string ToString() => $"Vector({X}, {Y}, {Z})";

		public Jade_Vector() { }
		public Jade_Vector(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		#region Constants

		public static Jade_Vector Zero => new Jade_Vector();
		public static Jade_Vector One => new Jade_Vector(1, 1, 1);
		
		#endregion

		#region Operators

		public static Jade_Vector operator +(Jade_Vector v) => v;
		public static Jade_Vector operator -(Jade_Vector v) => new Jade_Vector(-v.X, -v.Y, -v.Z);
		public static Jade_Vector operator +(Jade_Vector a, Jade_Vector b)
			=> new Jade_Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		public static Jade_Vector operator -(Jade_Vector a, Jade_Vector b)
			=> a + (-b);
		public static Jade_Vector operator *(Jade_Vector a, Jade_Vector b)
			=> new Jade_Vector(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		public static Jade_Vector operator *(Jade_Vector a, double b)
			=> new Jade_Vector((float)(a.X * b), (float)(a.Y * b), (float)(a.Z * b));
		public static Jade_Vector operator /(Jade_Vector a, double b) {
			if (b == 0)
				throw new DivideByZeroException();

			return a * (1f/b);
		}

		#endregion

		#region Math

		public double Magnitude => Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));
		public Jade_Vector Normalize() {
			var magnitude = Magnitude;
			if (magnitude == 0f)
				return Jade_Vector.Zero;
			return this / magnitude;
		}
		public static Jade_Vector CrossProduct(Jade_Vector a, Jade_Vector b) => new Jade_Vector(
			(a.Y * b.Z) - (a.Z * b.Y),
			(a.Z * b.X) - (a.X * b.Z),
			(a.X * b.Y) - (a.Y * b.X)
			);
		public static float Distance(Jade_Vector a, Jade_Vector b) => (float)((b - a).Magnitude);

		#endregion

		#region Equality

		public bool Equals(Jade_Vector other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return X == other.X && Y == other.Y && Z == other.Z;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj))
				return false;
			if (ReferenceEquals(this, obj))
				return true;
			if (obj.GetType() != GetType())
				return false;

			return Equals((Jade_Vector)obj);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				hashCode = (hashCode * 397) ^ Z.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(Jade_Vector a, Jade_Vector b) {
			if (ReferenceEquals(a, b))
				return true;
			if (ReferenceEquals(null, a))
				return false;
			if (ReferenceEquals(null, b))
				return false;
			return a.Equals(b);
		}
		public static bool operator !=(Jade_Vector a, Jade_Vector b) => !(a == b);

		#endregion
	}
}
