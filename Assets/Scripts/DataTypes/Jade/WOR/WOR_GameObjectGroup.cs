using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WOR_GameObjectGroup : Jade_File {
		public GameObjectRef[] GameObjects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GameObjects = s.SerializeObjectArray<GameObjectRef>(GameObjects, FileSize / (Loader.IsBinaryData ? 4 : 8), name: nameof(GameObjects));
			foreach (var reference in GameObjects) {
				reference.Resolve();
			}
		}

        public class GameObjectRef : BinarySerializable {
            public Jade_GenericReference ReferenceEditor { get; set; }
            public Jade_Reference<OBJ_GameObject> Reference { get; set; }
            public bool IsNull => Reference?.IsNull ?? ReferenceEditor?.IsNull ?? false;
            public OBJ_GameObject Value {
                get {
                    if(Reference != null) return Reference?.Value;
                    if(ReferenceEditor != null) return (OBJ_GameObject)ReferenceEditor?.Value;
                    return null;
                }
            }
            public Jade_Key Key {
                get {
                    if (Reference != null) return Reference?.Key;
                    if (ReferenceEditor != null) return ReferenceEditor?.Key;
                    return null;
                }
            }

            public override void SerializeImpl(SerializerObject s) {
                LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

                if (!Loader.IsBinaryData) {
                    ReferenceEditor = s.SerializeObject<Jade_GenericReference>(ReferenceEditor, name: nameof(ReferenceEditor));
                } else {
                    Reference = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Reference, name: nameof(Reference));
                }
            }

            public void Resolve() {
                if (Reference != null) {
                    Reference?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
                } else {
                    ReferenceEditor?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
                }
            }
        }
    }
}
