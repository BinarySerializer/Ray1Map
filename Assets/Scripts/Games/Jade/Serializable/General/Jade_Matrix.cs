using BinarySerializer;
using System;
using UnityEngine;

namespace Ray1Map.Jade {
	public class Jade_Matrix : BinarySerializable {

		// Format: M{Row}{Column}
		public float Ix { get; set; }
		public float Iy { get; set; }
		public float Iz { get; set; }
		public float Sx { get; set; }

		public float Jx { get; set; }
		public float Jy { get; set; }
		public float Jz { get; set; }
		public float Sy { get; set; }

		public float Kx { get; set; }
		public float Ky { get; set; }
		public float Kz { get; set; }
		public float Sz { get; set; }

		public float Tx { get; set; }
		public float Ty { get; set; }
		public float Tz { get; set; }
		public float w { get; set; }

		public TypeFlags Type { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Ix = s.Serialize<float>(Ix, name: nameof(Ix));
			Iy = s.Serialize<float>(Iy, name: nameof(Iy));
			Iz = s.Serialize<float>(Iz, name: nameof(Iz));
			Sx = s.Serialize<float>(Sx, name: nameof(Sx));

			Jx = s.Serialize<float>(Jx, name: nameof(Jx));
			Jy = s.Serialize<float>(Jy, name: nameof(Jy));
			Jz = s.Serialize<float>(Jz, name: nameof(Jz));
			Sy = s.Serialize<float>(Sy, name: nameof(Sy));

			Kx = s.Serialize<float>(Kx, name: nameof(Kx));
			Ky = s.Serialize<float>(Ky, name: nameof(Ky));
			Kz = s.Serialize<float>(Kz, name: nameof(Kz));
			Sz = s.Serialize<float>(Sz, name: nameof(Sz));

			Tx = s.Serialize<float>(Tx, name: nameof(Tx));
			Ty = s.Serialize<float>(Ty, name: nameof(Ty));
			Tz = s.Serialize<float>(Tz, name: nameof(Tz));
			w = s.Serialize<float>(w, name: nameof(w));

			Type = s.Serialize<TypeFlags>(Type, name: nameof(Type));
		}

		#region Unity stuff - todo: make extension methods
		public Matrix4x4 M => new Matrix4x4(
				new Vector4(Ix, Iy, Iz, Sx),
				new Vector4(Jx, Jy, Jz, Sy),
				new Vector4(Kx, Ky, Kz, Sz),
				new Vector4(Tx, Ty, Tz, w));
		public Vector3 GetScale(bool convertAxes = false) {
			if (!Type.HasFlag(TypeFlags.Scale)) return Vector3.one;

			if (convertAxes) {
				return new Vector3(Sx, Sz, Sy);
			} else {
				return new Vector3(Sx, Sy, Sz);
			}
		}
		public Vector3 GetPosition(bool convertAxes = false) {
			if (!Type.HasFlag(TypeFlags.Translation)) return Vector3.zero;

			if (convertAxes) {
				return new Vector3(Tx, Tz, Ty);
			} else {
				return new Vector3(Tx, Ty, Tz);
			}
		}

		public Quaternion GetRotation(bool convertAxes = false) {
			if(!Type.HasFlag(TypeFlags.Rotation)) return Quaternion.identity;

			float m00, m01, m02, m10, m11, m12, m20, m21, m22;
			m00 = Ix;
			m01 = Jx;
			m02 = Kx;
			m10 = Iy;
			m11 = Jy;
			m12 = Ky;
			m20 = Iz;
			m21 = Jz;
			m22 = Kz;

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
			m.SetTRS(Vector3.zero, rot, Vector3.one);
			Ix = m.m00;
			Iy = m.m10;
			Iz = m.m20;
			Jx = m.m01;
			Jy = m.m11;
			Jz = m.m21;
			Kx = m.m02;
			Ky = m.m12;
			Kz = m.m22;
			
			Sx = scale.x;
			Sy = scale.y;
			Sz = scale.z;
			Tx = pos.x;
			Ty = pos.y;
			Tz = pos.z;
			w = 1f;

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
