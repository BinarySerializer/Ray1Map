﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BinarySerializer;

namespace R1Engine.Jade {
	public class OBJ_GameObject_Anim : BinarySerializable {
		public uint Type { get; set; } // Set in on PreSerialize

		public Jade_Reference<GEO_Object> GeometricObject { get; set; }
		public Jade_Reference<GEO_Object> Material { get; set; }
		public int Int_7A { get; set; }
		public uint UInt_7E { get; set; } // Seem to be flags of some sort

		public OBJ_GameObject_GeometricData_Xenon Xenon { get; set; }

		// RLI
		public uint Code { get; set; }
		public Jade_Reference<OBJ_GameObjectRLI> RLI { get; set; }
		public uint[] RLI_UInts { get; set; }

		public OBJ_GameObject_GeometricData_Xenon2 Xenon2 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GeometricObject = s.SerializeObject<Jade_Reference<GEO_Object>>(GeometricObject, name: nameof(GeometricObject))?.Resolve(onPostSerialize: (_, f) => {
				if (f.RenderObject.Type != GRO_Type.GEO
				&& f.RenderObject.Type != GRO_Type.PAG
				&& f.RenderObject.Type != GRO_Type.GEO_StaticLOD
				&& f.RenderObject.Type != GRO_Type.CAM
				&& f.RenderObject.Type != GRO_Type.STR
				&& f.RenderObject.Type != GRO_Type.Unknown) {
					throw new Exception($"{f.Key}: Expected GEO, got {f.RenderObject.Type}");
				}
			});
			Material = s.SerializeObject<Jade_Reference<GEO_Object>>(Material, name: nameof(Material))?.Resolve(onPostSerialize: (_,f) => {
				if(f.RenderObject.Type != GRO_Type.MAT_MSM
				&& f.RenderObject.Type != GRO_Type.MAT_MTT
				&& f.RenderObject.Type != GRO_Type.MAT_SIN) {
					throw new Exception($"{f.Key}: Expected material, got {f.RenderObject.Type}");
				}
			});
			Int_7A = s.Serialize<int>(Int_7A, name: nameof(Int_7A));
			UInt_7E = s.Serialize<uint>(UInt_7E, name: nameof(UInt_7E));

			if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon) {
				Xenon = s.SerializeObject<OBJ_GameObject_GeometricData_Xenon>(Xenon, onPreSerialize: x => x.Type = Type, name: nameof(Xenon));
			}

			// RLI
			Code = s.Serialize<uint>(Code, name: nameof(Code));
			if (Code == (uint)Jade_Code.RLI) {
				RLI = s.SerializeObject<Jade_Reference<OBJ_GameObjectRLI>>(RLI, name: nameof(RLI))?.Resolve();
			} else {
				RLI_UInts = s.SerializeArray<uint>(RLI_UInts, Code, name: nameof(RLI_UInts));
			}

			if (s.GetR1Settings().Jade_Version == Jade_Version.Xenon) {
				Xenon2 = s.SerializeObject<OBJ_GameObject_GeometricData_Xenon2>(Xenon2, onPreSerialize: x => x.Type = Type, name: nameof(Xenon2));
			}
		}
	}
}