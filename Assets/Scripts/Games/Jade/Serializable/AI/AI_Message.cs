using BinarySerializer;
using System;

namespace Ray1Map.Jade {
	public class AI_Message : BinarySerializable {
		public Jade_Reference<OBJ_GameObject> Sender { get; set; }
		public Jade_Reference<OBJ_GameObject> GAO1 { get; set; }
		public Jade_Reference<OBJ_GameObject> GAO2 { get; set; }
		public Jade_Reference<OBJ_GameObject> GAO3 { get; set; }
		public Jade_Reference<OBJ_GameObject> GAO4 { get; set; }
		public Jade_Reference<OBJ_GameObject> GAO5 { get; set; }
		public Jade_Vector Vector1 { get; set; }
		public Jade_Vector Vector2 { get; set; }
		public Jade_Vector Vector3 { get; set; }
		public Jade_Vector Vector4 { get; set; }
		public Jade_Vector Vector5 { get; set; }
		public int Int1 { get; set; }
		public int Int2 { get; set; }
		public int Int3 { get; set; }
		public int Int4 { get; set; }
		public int Int5 { get; set; }
		public int ID { get; set; }

		public float BGE_Float_0 { get; set; }
		public float BGE_Float_1 { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			if (!s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_KingKong)) {
				GAO1 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO1, name: nameof(GAO1));
				GAO2 = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO2, name: nameof(GAO2));
				Vector1 = s.SerializeObject<Jade_Vector>(Vector1, name: nameof(Vector1));
				Vector2 = s.SerializeObject<Jade_Vector>(Vector2, name: nameof(Vector2));
				BGE_Float_0 = s.Serialize<float>(BGE_Float_0, name: nameof(BGE_Float_0));
				BGE_Float_1 = s.Serialize<float>(BGE_Float_1, name: nameof(BGE_Float_1));
				Int1 = s.Serialize<int>(Int1, name: nameof(Int1));
				Int2 = s.Serialize<int>(Int2, name: nameof(Int2));
				ID = s.Serialize<int>(ID, name: nameof(ID));
			} else {
				Sender = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Sender, name: nameof(Sender));
				GAO1   = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO1, name: nameof(GAO1));
				GAO2   = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO2, name: nameof(GAO2));
				GAO3   = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO3, name: nameof(GAO3));
				GAO4   = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO4, name: nameof(GAO4));
				GAO5   = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GAO5, name: nameof(GAO5));
				Vector1 = s.SerializeObject<Jade_Vector>(Vector1, name: nameof(Vector1));
				Vector2 = s.SerializeObject<Jade_Vector>(Vector2, name: nameof(Vector2));
				Vector3 = s.SerializeObject<Jade_Vector>(Vector3, name: nameof(Vector3));
				Vector4 = s.SerializeObject<Jade_Vector>(Vector4, name: nameof(Vector4));
				Vector5 = s.SerializeObject<Jade_Vector>(Vector5, name: nameof(Vector5));
				Int1 = s.Serialize<int>(Int1, name: nameof(Int1));
				Int2 = s.Serialize<int>(Int2, name: nameof(Int2));
				Int3 = s.Serialize<int>(Int3, name: nameof(Int3));
				Int4 = s.Serialize<int>(Int4, name: nameof(Int4));
				Int5 = s.Serialize<int>(Int5, name: nameof(Int5));
				ID = s.Serialize<int>(ID, name: nameof(ID));
			}
		}

		public override string ToString() {
			return $"Message({Sender}, {GAO1}, {GAO2}, {Int1}, {Int2})";
		}
	}
}
