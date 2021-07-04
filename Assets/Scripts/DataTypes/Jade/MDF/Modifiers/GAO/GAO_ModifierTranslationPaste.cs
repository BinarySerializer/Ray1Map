using BinarySerializer;

namespace R1Engine.Jade {
	public class GAO_ModifierTranslationPaste : MDF_Modifier {
		public uint UInt_00 { get; set; }
		public Jade_Reference<OBJ_GameObject> ObjectToPasteFrom { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
			ObjectToPasteFrom = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(ObjectToPasteFrom, name: nameof(ObjectToPasteFrom))?.Resolve();
		}
	}
}
