using System;
using System.Collections.Generic;
using System.Linq;
using BinarySerializer;

namespace R1Engine.Jade {
	public class TEXT_OneText : BinarySerializable {
		public bool HasSound { get; set; }

		public Jade_Key IdKey { get; set; }
		public Jade_Reference<SND_Wave> Sound { get; set; }
		public Jade_Key ObjKey { get; set; }
		public int OffsetInBuffer { get; set; }
		public ushort Priority { get; set; }
		public ushort Version { get; set; }

		public byte FacialIdx { get; set; }
		public byte LipsIdx { get; set; }
		public byte AnimIdx { get; set; }
		public byte DumIdx { get; set; }

		public uint EditorStringLength { get; set; }
		public string IDString { get; set; }
		public string EditorString { get; set; }

		public string Text { get; set; }
		//public string ProcessedText { get; set; }
		//public byte[] TextBytes { get; set; }


		public override void SerializeImpl(SerializerObject s) {
			LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);

			if (!HasSound || !Loader.IsBinaryData) IdKey = s.SerializeObject<Jade_Key>(IdKey, name: nameof(IdKey));
			if (HasSound || !Loader.IsBinaryData) Sound = s.SerializeObject<Jade_Reference<SND_Wave>>(Sound, name: nameof(Sound));
			if (!HasSound || !Loader.IsBinaryData) {
				ObjKey = s.SerializeObject<Jade_Key>(ObjKey, name: nameof(ObjKey));
				OffsetInBuffer = s.Serialize<int>(OffsetInBuffer, name: nameof(OffsetInBuffer));
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
			}
			EditorStringLength = s.Serialize<uint>(EditorStringLength, name: nameof(EditorStringLength));
			if (!Loader.IsBinaryData) {
				IDString = s.SerializeString(IDString, length: 0x40, encoding: Jade_BaseManager.Encoding, name: nameof(IDString));
				EditorString = s.SerializeString(EditorString, length: EditorStringLength, encoding: Jade_BaseManager.Encoding, name: nameof(EditorString));
			}
			if (HasSound) {
				Sound?.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.KeepReferencesCount);
			}
		}

		public void SerializeString(SerializerObject s, Pointer bufferPointer) {
			if (OffsetInBuffer >= 0) {
				s.DoAt(bufferPointer + OffsetInBuffer, () => {
					System.Text.Encoding encoding = Jade_BaseManager.Encoding;
					Text = s.SerializeString(Text, encoding: encoding, name: nameof(Text));
					/*ProcessedText = Text;
					if (!string.IsNullOrEmpty(Text) && Text.Contains("\\u")) {
						s.DoAt(bufferPointer + OffsetInBuffer, () => {
							TextBytes = s.SerializeArray<byte>(TextBytes, Text.Length, name: nameof(TextBytes));
						});
						ProcessText();
						s.Log(ProcessedText);
					}*/
				});
			}
		}

		/*public void ProcessText() {
			int curByte = 0;
			int curByteCharDifference = 0;
			bool unicode = false;
			while (curByte < Text.Length) {
				if (Text[curByte] == '\\') {
					curByte += 2;
					void readFormatCode() {
						while (curByte < Text.Length) {
							curByte++;
							if (Text[curByte - 1] == '\\') break;
						}
					}

					switch (Text[curByte - 1]) {
						case 'A':
						case 'D':
						case 'J':
						case 'P':
						case 'a':
						case 'b':
						case 'c':
						case 'h':
						case 'j':
						case 'm':
						case 'p':
						case 'w':
						case 'x':
						case 'y':
							readFormatCode();
							break;
						case 'd':
							if(curByte < Text.Length) curByte++;
							break;
						case 'f':
							if (curByte < Text.Length) {
								curByte++;
								switch (Text[curByte - 1]) {
									case 'c':
									case 'i':
										readFormatCode();
										break;
								}
							}
							break;
						case 'u':
							unicode = true;
							break;
					}
				} else {
					if (unicode) {
						byte row = TextBytes[curByte];
						byte byte2 = TextBytes[curByte + 1];
						var charIndexInFontDescriptor = byte2 + ((row - 0x20) * 200) - 0x20;
						//var bytes = new byte[] { row, byte2 };
						//var newString = System.Text.Encoding.Unicode.GetString(bytes);
						var newString = $"{charIndexInFontDescriptor},";
						ProcessedText = ProcessedText.Remove(curByte - curByteCharDifference,2).Insert(curByte - curByteCharDifference, newString);
						curByteCharDifference += 2 - newString.Length;
						curByte += 2;
					} else {
						curByte++;
					}
				}
			}
		}*/

	}
}
