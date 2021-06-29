using BinarySerializer;

namespace R1Engine.Jade
{
    public class OBJ_World_GroupObjectList : Jade_File 
    {
		public GroupObject[] GroupObjects { get; set; }
        public bool ResolveObjects { get; set; } = true;

		public override void SerializeImpl(SerializerObject s) 
        {
			var fileEnd = Offset + FileSize;
			GroupObjects = s.SerializeObjectArrayUntil(GroupObjects, x => s.CurrentAbsoluteOffset >= fileEnd.AbsoluteOffset, onPreSerialize: go => go.List = this, name: nameof(GroupObjects));
		}

        public class GroupObject : BinarySerializable
        {
            public OBJ_World_GroupObjectList List { get; set; }

            public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
            public uint Uint_04_Editor { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject));
                if(List.ResolveObjects) GameObject?.Resolve();
                if (!Loader.IsBinaryData) Uint_04_Editor = s.Serialize<uint>(Uint_04_Editor, name: nameof(Uint_04_Editor));
            }
        }
    }
}