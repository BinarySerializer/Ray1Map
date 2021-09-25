using BinarySerializer;
using System;
using UnityEngine;

namespace R1Engine.Jade {
	public class Jade_Matrix : BinarySerializable {

		// Format: M{Row}{Column}
		public float M00 { get; set; }
		public float M10 { get; set; }
		public float M20 { get; set; }
		public float M30 { get; set; }

		public float M01 { get; set; }
		public float M11 { get; set; }
		public float M21 { get; set; }
		public float M31 { get; set; }

		public float M02 { get; set; }
		public float M12 { get; set; }
		public float M22 { get; set; }
		public float M32 { get; set; }

		public float M03 { get; set; }
		public float M13 { get; set; }
		public float M23 { get; set; }
		public float M33 { get; set; }

		public TypeFlags Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			M00 = s.Serialize<float>(M00, name: nameof(M00));
			M10 = s.Serialize<float>(M10, name: nameof(M10));
			M20 = s.Serialize<float>(M20, name: nameof(M20));
			M30 = s.Serialize<float>(M30, name: nameof(M30));

			M01 = s.Serialize<float>(M01, name: nameof(M01));
			M11 = s.Serialize<float>(M11, name: nameof(M11));
			M21 = s.Serialize<float>(M21, name: nameof(M21));
			M31 = s.Serialize<float>(M31, name: nameof(M31));

			M02 = s.Serialize<float>(M02, name: nameof(M02));
			M12 = s.Serialize<float>(M12, name: nameof(M12));
			M22 = s.Serialize<float>(M22, name: nameof(M22));
			M32 = s.Serialize<float>(M32, name: nameof(M32));

			M03 = s.Serialize<float>(M03, name: nameof(M03));
			M13 = s.Serialize<float>(M13, name: nameof(M13));
			M23 = s.Serialize<float>(M23, name: nameof(M23));
			M33 = s.Serialize<float>(M33, name: nameof(M33));

			Type = s.Serialize<TypeFlags>(Type, name: nameof(Type));
		}

		#region Unity stuff - todo: make extension methods
		public Matrix4x4 M => new Matrix4x4(
				new Vector4(M00, M10, M20, M30),
				new Vector4(M01, M11, M21, M31),
				new Vector4(M02, M12, M22, M32),
				new Vector4(M03, M13, M23, M33));
		public Vector3 GetScale(bool convertAxes = false) {
			if (!Type.HasFlag(TypeFlags.Scale)) return Vector3.one;

			Matrix4x4 m = M;
			if (convertAxes) {
				return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(2).magnitude, m.GetColumn(1).magnitude);
			} else {
				return new Vector3(m.GetColumn(0).magnitude, m.GetColumn(1).magnitude, m.GetColumn(2).magnitude);
			}
		}
		public Vector3 GetPosition(bool convertAxes = false) {
			if (!Type.HasFlag(TypeFlags.Translation)) return Vector3.zero;

			if (convertAxes) {
				return new Vector3(M03, M23, M13);
			} else {
				return new Vector3(M03, M13, M23);
			}
		}

		public Quaternion GetRotation(bool convertAxes = false) {
			if(!Type.HasFlag(TypeFlags.Rotation)) return Quaternion.identity;

			float m00, m01, m02, m10, m11, m12, m20, m21, m22;
			m00 = M00;
			m01 = M01;
			m02 = M02;
			m10 = M10;
			m11 = M11;
			m12 = M12;
			m20 = M20;
			m21 = M21;
			m22 = M22;

			float tr = m00 + m11 + m22;
			Quaternion q = new Quaternion();
			float t;
			if (m22 < 0) {
				if (m00 > m11) {
					t = 1 + m00 - m11 - m22;
					q = new Quaternion(t, m01 + m10, m20 + m02, m12 - m21);
				} else {
					t = 1 - m00 + m11 - m22;
					q = new Quaternion(m01 + m10, t, m12 + m21, m20 - m02);
				}
			} else {
				if (m00 < -m11) {
					t = 1 - m00 - m11 + m22;
					q = new Quaternion(m20 + m02, m12 + m21, t, m01 - m10);
				} else {
					t = 1 + m00 + m11 + m22;
					q = new Quaternion(m12 - m21, m20 - m02, m01 - m10, t);
				}
			}
			float factor = (0.5f / Mathf.Sqrt(t));
			q.x = q.x * factor;
			q.y = q.y * factor;
			q.z = q.z * factor;
			q.w = q.w * -factor;

			if (convertAxes) {
				q = new Quaternion(q.x, q.z, q.y, -q.w);
			}

			return q;
		}


		// For writing
		public void SetTRS(Vector3 pos, Quaternion rot, Vector3 scale, TypeFlags? newType = null, bool convertAxes = false) {
			if (convertAxes) {
				Vector3 tempRot = rot.eulerAngles;
				tempRot = new Vector3(tempRot.z, tempRot.x, -tempRot.y);
				rot = Quaternion.Euler(tempRot);
				scale = new Vector3(scale.x, scale.z, scale.y);
				pos = new Vector3(pos.x, pos.z, pos.y);
			}
			var m = M;
			m.SetTRS(pos, rot, scale);
			M00 = m.m00;
			M01 = m.m01;
			M02 = m.m02;
			M03 = m.m03;
			M10 = m.m10;
			M11 = m.m11;
			M12 = m.m12;
			M13 = m.m13;
			M20 = m.m20;
			M21 = m.m21;
			M22 = m.m22;
			M23 = m.m23;
			M30 = m.m30;
			M31 = m.m31;
			M32 = m.m32;
			M33 = m.m33;
			if (newType.HasValue) Type = newType.Value;

		}
		#endregion
	}

	[Flags]
	public enum TypeFlags : int {
		None = 0,
		_1 = 1 << 0,
		Translation = 1 << 1,
		Rotation = 1 << 2,
		Scale = 1 << 3,
	}
}
