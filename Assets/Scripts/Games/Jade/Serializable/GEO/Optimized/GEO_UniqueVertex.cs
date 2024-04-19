using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.Jade
{
    public class GEO_UniqueVertex : IEquatable<GEO_UniqueVertex> {
		public Jade_Vector Vertex { get; set; }
		public Jade_Vector Normal { get; set; }
		public SerializableColor Color { get; set; }

		public int[] Bones { get; set; }
		public float[] Weights { get; set; }

		#region Equality

		bool ArraysEqual<T>(T[] a1, T[] a2) {
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			if (a1.Length != a2.Length)
				return false;

			var comparer = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++) {
				if (!comparer.Equals(a1[i], a2[i])) return false;
			}
			return true;
		}

		bool ColorsEqual(BaseColor a1, BaseColor a2) {
			if (ReferenceEquals(a1, a2))
				return true;

			if (a1 == null || a2 == null)
				return false;

			return a1.Equals(a2);
		}

		public override bool Equals(object other) {
			if (other is GEO_UniqueVertex uv)
				return Equals(uv);
			else
				return false;
		}

		public bool Equals(GEO_UniqueVertex other) {
			if (other == null)
				return false;
			if (other.Vertex != Vertex || other.Normal != Normal || !other.Color.Equals(Color)
                || !ArraysEqual(other.Bones, Bones) || !ArraysEqual(other.Weights, Weights))
				return false;
			return true;
		}

		public override int GetHashCode() => (Vertex, Normal, Color, Bones, Weights).GetHashCode();

		public static bool operator ==(GEO_UniqueVertex term1, GEO_UniqueVertex term2) {
			if ((object)term1 == null)
				return (object)term2 == null;

			return term1.Equals(term2);
		}

		public static bool operator !=(GEO_UniqueVertex term1, GEO_UniqueVertex term2) => !(term1 == term2);

		#endregion
	}
}
