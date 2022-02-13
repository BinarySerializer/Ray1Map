using BinarySerializer;

namespace Ray1Map.Jade
{
    public class COL_Cob : Jade_File {
        public override bool HasHeaderBFFile => true;
        public override string Export_Extension => "cob";

        public Jade_Reference<COL_GameMaterial> Material { get; set; }
        public COL_ZoneShape Type { get; set; } // 1, 2, 3 or 5
        public byte Flags { get; set; }
        public uint FacesCount { get; set; }

        public COL_Box Shape_Box { get; set; } // Type 1
        public COL_Sphere Shape_Sphere { get; set; } // Type 2
        public COL_Cylinder Shape_Cylinder { get; set; } // Type 3
        public COL_IndexedTriangles Shape_IndexedTriangles { get; set; } // Type 5

        public COL_EventObject EventObject { get; set; }

        protected override void SerializeFile(SerializerObject s) {
            Material = s.SerializeObject<Jade_Reference<COL_GameMaterial>>(Material, name: nameof(Material))?.Resolve();
            Type = s.Serialize<COL_ZoneShape>(Type, name: nameof(Type));
            Flags = s.Serialize<byte>(Flags, name: nameof(Flags));

            if (Type != COL_ZoneShape.Box && Type != COL_ZoneShape.Sphere && Type != COL_ZoneShape.Cylinder
                && Type != COL_ZoneShape.Triangles && Type != COL_ZoneShape.Unknown7)
                return;

            if (Type != COL_ZoneShape.Triangles && Type != COL_ZoneShape.Unknown7)
                FacesCount = s.Serialize<uint>(FacesCount, name: nameof(FacesCount));

            switch (Type)
            {
                case COL_ZoneShape.Box:
                    Shape_Box = s.SerializeObject<COL_Box>(Shape_Box, name: nameof(Shape_Box));
                    break;

                case COL_ZoneShape.Sphere:
                    Shape_Sphere = s.SerializeObject<COL_Sphere>(Shape_Sphere, name: nameof(Shape_Sphere));
                    break;

                case COL_ZoneShape.Cylinder:
                    Shape_Cylinder = s.SerializeObject<COL_Cylinder>(Shape_Cylinder, name: nameof(Shape_Cylinder));
                    break;

                case COL_ZoneShape.Triangles:
                    Shape_IndexedTriangles = s.SerializeObject<COL_IndexedTriangles>(Shape_IndexedTriangles,
                        onPreSerialize: it => it.Flags = Flags,
                        name: nameof(Shape_IndexedTriangles));

                    break;
            }

            if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal)) {
                EventObject = s.SerializeObject<COL_EventObject>(EventObject, name: nameof(EventObject));
            }
		}
    }
}