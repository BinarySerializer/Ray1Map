using System;
using System.Linq;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEXT_OneText : BinarySerializable {
		public bool HasSound { get; set; }

		public Jade_Key IdKey { get; set; }
		public Jade_Key SoundKey { get; set; }
		public Jade_Key ObjKey { get; set; }
		public uint OffsetInBuffer { get; set; }
		public ushort Priority { get; set; }
		public ushort Version { get; set; }

		public byte FacialIdx { get; set; }
		public byte LipsIdx { get; set; }
		public byte AnimIdx { get; set; }
		public byte DumIdx { get; set; }

		public uint EditorStringLength { get; set; }
		public string IDString { get; set; }
		public string EditorString { get; set; }

		public string String { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			IdKey = s.SerializeObject<Jade_Key>(IdKey, name: nameof(IdKey));
			if(HasSound || !Loader.IsBinaryData) SoundKey = s.SerializeObject<Jade_Key>(SoundKey, name: nameof(SoundKey));
			ObjKey = s.SerializeObject<Jade_Key>(ObjKey, name: nameof(ObjKey));
			OffsetInBuffer = s.Serialize<uint>(OffsetInBuffer, name: nameof(OffsetInBuffer));
			s.SerializeBitValues<int>(bitFunc => {
				Priority = (ushort)bitFunc(Priority, 16, name: nameof(Priority));
				Version = (ushort)bitFunc(Version, 16, name: nameof(Version));
			});
			if (Version >= 1) {
				FacialIdx = s.Serialize<byte>(FacialIdx, name: nameof(FacialIdx));
				LipsIdx = s.Serialize<byte>(LipsIdx, name: nameof(LipsIdx));
				AnimIdx = s.Serialize<byte>(AnimIdx, name: nameof(AnimIdx));
				DumIdx = s.Serialize<byte>(DumIdx, name: nameof(DumIdx));
			}
			EditorStringLength = s.Serialize<uint>(EditorStringLength, name: nameof(EditorStringLength));
			if (!Loader.IsBinaryData) {
				IDString = s.SerializeString(IDString, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(IDString));
				EditorString = s.SerializeString(EditorString, length: EditorStringLength, encoding: Jade_BaseManager.Encoding, name: nameof(EditorString));
			}
		}

		public void SerializeString(SerializerObject s, Pointer bufferPointer) {
			s.DoAt(bufferPointer + OffsetInBuffer, () => {
				String = s.SerializeString(String, encoding: Jade_BaseManager.Encoding, name: nameof(String));
			});
		}
	}
}
