using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WOR_GameObjectGroup : Jade_File {
		public GameObjectRef[] GameObjects { get; set; }
        public override string Export_Extension => "gal";

        public override void SerializeImpl(SerializerObject s) {
			GameObjects = s.SerializeObjectArray<GameObjectRef>(GameObjects, FileSize / (Loader.IsBinaryData ? 4 : 8), name: nameof(GameObjects));
			foreach (var reference in GameObjects) {
				reference.Resolve();
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
                if (!Loader.IsBinaryData) Type = s.SerializeObject<Jade_FileType>(Type, name: nameof(Type));
            }

            public void Resolve() {
                Reference?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
            }
        }
    }
}
