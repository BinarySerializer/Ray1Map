using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class COL_IndexedTriangles : BinarySerializable{
        public byte Flags { get; set; } // Set in OnPreSerialize

        public uint VerticesCount { get; set; }
        public Jade_Vector[] Vertices { get; set; }
        public uint FacesCount { get; set; }
        public Jade_Vector[] FaceNormals { get; set; }
        public uint UVsCount { get; set; }
        public UV[] UVs { get; set; } // NCIS only
        public uint ElementsCount { get; set; }
        public COL_ElementIndexedTriangles[] Elements { get; set; }
        public COL_OK3 OK3 { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

            VerticesCount = s.Serialize<uint>(VerticesCount, name: nameof(VerticesCount));
            Vertices = s.SerializeObjectArray<Jade_Vector>(Vertices, VerticesCount, name: nameof(Vertices));

            FacesCount = s.Serialize<uint>(FacesCount, name: nameof(FacesCount));
            FaceNormals = s.SerializeObjectArray<Jade_Vector>(FaceNormals, FacesCount, name: nameof(FaceNormals));

            ElementsCount = s.Serialize<uint>(ElementsCount, name: nameof(ElementsCount));
            Elements = s.SerializeObjectArray<COL_ElementIndexedTriangles>(Elements, ElementsCount, name: nameof(Elements));

            if (((Flags & 0x80) != 0 || s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) && Loader.IsBinaryData)
            {
                foreach (var element in Elements) {
                    element.SerializeProx(s);
                }

                if ((Flags & 8) == 8) {
                    OK3 = s.SerializeObject<COL_OK3>(OK3, name: nameof(OK3));
                }
            }
        }

        public class COL_ElementIndexedTriangles : BinarySerializable
        {
            public ushort TrianglesCount { get; set; }
            public byte Design { get; set; }
            public byte Flag { get; set; }
            public ushort Flag_Montreal { get; set; }
            public uint MaterialID { get; set; }
            public uint PhoenixMovieGames_Design10_UInt { get; set; }
            public Triangle[] Triangles { get; set; }
            public Triangle[] Prox { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                TrianglesCount = s.Serialize<ushort>(TrianglesCount, name: nameof(TrianglesCount));
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
					Flag_Montreal = s.Serialize<ushort>(Flag_Montreal, name: nameof(Flag_Montreal));
				} else {
                    Design = s.Serialize<byte>(Design, name: nameof(Design));
                    Flag = s.Serialize<byte>(Flag, name: nameof(Flag));
                }
                MaterialID = s.Serialize<uint>(MaterialID, name: nameof(MaterialID));
                if(s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PetzHorseClub) && Design == 10)
					PhoenixMovieGames_Design10_UInt = s.Serialize<uint>(PhoenixMovieGames_Design10_UInt, name: nameof(PhoenixMovieGames_Design10_UInt));
				Triangles = s.SerializeObjectArray<Triangle>(Triangles, TrianglesCount, name: nameof(Triangles));
            }

            public void SerializeProx(SerializerObject s) {
                Prox = s.SerializeObjectArray<Triangle>(Prox, TrianglesCount, name: nameof(Prox));
            }
        }

        public class Triangle : BinarySerializable
        {
            public short Index0 { get; set; }
            public short Index1 { get; set; }
            public short Index2 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Index0 = s.Serialize<short>(Index0, name: nameof(Index0));
                Index1 = s.Serialize<short>(Index1, name: nameof(Index1));
                Index2 = s.Serialize<short>(Index2, name: nameof(Index2));
            }
        }
        public class UV : BinarySerializable {
            public float U { get; set; }
            public float V { get; set; }

            public override void SerializeImpl(SerializerObject s) {
                U = s.Serialize<float>(U, name: nameof(U));
                V = s.Serialize<float>(V, name: nameof(V));
            }
        }
    }
}