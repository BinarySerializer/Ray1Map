using BinarySerializer;

namespace Ray1Map.Jade
{
    public class OBJ_World_GroupObjectList : Jade_File 
    {
		public override string Export_Extension => "gog";
		public GroupObject[] GroupObjects { get; set; }
        public bool ResolveObjects { get; set; } = true;

        protected override void SerializeFile(SerializerObject s) 
        {
			var fileEnd = Offset + FileSize;
			GroupObjects = s.SerializeObjectArrayUntil(GroupObjects, x => s.CurrentAbsoluteOffset >= fileEnd.AbsoluteOffset, onPreSerialize: go => go.List = this, name: nameof(GroupObjects));
		}

        public class GroupObject : BinarySerializable
        {
            public OBJ_World_GroupObjectList List { get; set; }

            public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
            public Jade_FileType Type { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
                if (!List.Loader.IsBinaryData) {
                    Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
                } else {
                    if (!GameObject.IsNull) Type = new Jade_FileType() { Extension = ".gao" };
                }
                if (List.ResolveObjects) GameObject?.Resolve();
            }
        }
    }
}