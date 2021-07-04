using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class GAO_ModifierRotationPaste : MDF_Modifier {
        public uint Version { get; set; }
        public Jade_Reference<OBJ_GameObject> GameObject { get; set; }

        public override void SerializeImpl(SerializerObject s) {
            Version = s.Serialize<uint>(Version, name: nameof(Version));;
			GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
		}
	}
}
