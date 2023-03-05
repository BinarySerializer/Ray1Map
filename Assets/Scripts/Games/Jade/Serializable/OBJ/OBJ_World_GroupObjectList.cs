using BinarySerializer;

namespace Ray1Map.Jade
{
    public class OBJ_World_GroupObjectList : Jade_File 
    {
		public override string Export_Extension => "gog";
		public GameObjectRef[] GameObjects { get; set; }
		public bool ResolveObjects { get; set; } = true;

        protected override void SerializeFile(SerializerObject s) 
        {
			var fileEnd = Offset + FileSize;
			GameObjects = s.SerializeObjectArray<GameObjectRef>(GameObjects, FileSize / (Loader.IsBinaryData ? 4 : 8), name: nameof(GameObjects));
            if (ResolveObjects) {
                foreach (var reference in GameObjects) {
                    reference.Resolve();
                }
            }
		}

		public class GameObjectRef : BinarySerializable {
			public Jade_Reference<OBJ_GameObject> Reference { get; set; }
			public Jade_FileType Type { get; set; }
			public bool IsNull => Reference?.IsNull ?? false;
			public OBJ_GameObject Value => Reference?.Value;
			public Jade_Key Key => Reference?.Key;

			public override void SerializeImpl(SerializerObject s) {
				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

				Reference = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Reference, name: nameof(Reference));
				if (!Loader.IsBinaryData) {
					Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
				} else {
					if (!Reference.IsNull) {
						Type = new Jade_FileType() { Extension = ".gao" };
					}
				}
			}

			public void Resolve() {
				Reference?.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.Flag6);
			}
		}

		protected override void OnChangedIsBinaryData() {
			base.OnChangedIsBinaryData();
			if (CurrentIsBinaryData == false) {
				FileSize *= 2;
			} else FileSize /= 2;
		}
	}
}