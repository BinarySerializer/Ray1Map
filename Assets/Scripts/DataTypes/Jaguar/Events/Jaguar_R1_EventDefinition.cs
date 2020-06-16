using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace R1Engine {
	/// <summary>
	/// Event definition data for Rayman 1 (Jaguar)
	/// </summary>
	public class Jaguar_R1_EventDefinition : R1Serializable {
		#region Event Data

		public short StructType1 { get; set; }
		public Pointer Pointer_02 { get; set; } // Points to a struct of size 0x26. just some shorts, no pointers
		public ushort StructType2 { get; set; }

		public Pointer CodePointer { get; set; }
		public Pointer CurrentStatePointer { get; set; }
		public Pointer ComplexDataPointer { get; set; }
		public ushort UShort_10 { get; set; }
		public ushort UShort_12 { get; set; }
		public Pointer ImageDescriptorsPointer { get; set; }

		public uint ImageBufferMemoryPointerPointer { get; set; }
		public uint UInt_1C { get; set; }
		public byte Byte_20 { get; set; }
		public byte Byte_21 { get; set; }
		public byte Byte_22 { get; set; }
		public byte Byte_23 { get; set; }
		public ushort UShort_24 { get; set; }
		public ushort UShort_26 { get; set; }

		public byte[] UnkBytes { get; set; }
		public byte Byte_24 { get; set; }
		public byte Byte_25 { get; set; }
		public byte Byte_26 { get; set; }
		public byte Byte_27 { get; set; }
		public Pointer UnkPointer1 { get; set; }
		public Pointer UnkPointer2 { get; set; }

		#endregion

		#region Parsed from Pointers

		public Common_ImageDescriptor[] ImageDescriptors { get; set; }
		public Jaguar_R1_EventState[] States { get; set; }
		public Jaguar_R1_EventComplexData ComplexData { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Handles the data serialization
		/// </summary>
		/// <param name="s">The serializer object</param>
		public override void SerializeImpl(SerializerObject s) {
			StructType1 = s.Serialize<short>(StructType1, name: nameof(StructType1));
			Pointer_02 = s.SerializePointer(Pointer_02, name: nameof(Pointer_02));
			StructType2 = s.Serialize<ushort>(StructType2, name: nameof(StructType2));
			if (StructType2 == 29) {
				CurrentStatePointer = s.SerializePointer(CurrentStatePointer, name: nameof(CurrentStatePointer));
				ComplexDataPointer = s.SerializePointer(ComplexDataPointer, name: nameof(ComplexDataPointer));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
				ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
				UShort_26 = s.Serialize<ushort>(UShort_26, name: nameof(UShort_26));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
			} else if (StructType2 == 6 || StructType2 == 7 || StructType2 == 30 || StructType2 == 31) {
				CurrentStatePointer = s.SerializePointer(CurrentStatePointer, name: nameof(CurrentStatePointer));
				ComplexDataPointer = s.SerializePointer(ComplexDataPointer, name: nameof(ComplexDataPointer));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
				ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
				UShort_26 = s.Serialize<ushort>(UShort_26, name: nameof(UShort_26));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				Byte_24 = s.Serialize<byte>(Byte_24, name: nameof(Byte_24));
				Byte_25 = s.Serialize<byte>(Byte_25, name: nameof(Byte_25));
				Byte_26 = s.Serialize<byte>(Byte_26, name: nameof(Byte_26));
				Byte_27 = s.Serialize<byte>(Byte_27, name: nameof(Byte_27));
			} else if (StructType2 == 23 || StructType2 == 11 || StructType2 == 2) {
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1c, name: nameof(UnkBytes));
			} else if (StructType2 == 36 || StructType2 == 37 || StructType2 == 56) {
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1C, name: nameof(UnkBytes));
			} else if (StructType2 == 111) {
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x8, name: nameof(UnkBytes));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0xA, name: nameof(UnkBytes));
			} else if (StructType2 == 112 || StructType2 == 113 || StructType2 == 114) {
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x1a, name: nameof(UnkBytes));
			} else if (StructType2 == 25) {
				UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
				UnkPointer2 = s.SerializePointer(UnkPointer2, name: nameof(UnkPointer2));
				CodePointer = s.SerializePointer(CodePointer, name: nameof(CodePointer));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UnkBytes = s.SerializeArray<byte>(UnkBytes, 0x10, name: nameof(UnkBytes));
			} else {
				CurrentStatePointer = s.SerializePointer(CurrentStatePointer, name: nameof(CurrentStatePointer));
				UnkPointer1 = s.SerializePointer(UnkPointer1, name: nameof(UnkPointer1));
				UShort_10 = s.Serialize<ushort>(UShort_10, name: nameof(UShort_10));
				UShort_12 = s.Serialize<ushort>(UShort_12, name: nameof(UShort_12));
				ImageDescriptorsPointer = s.SerializePointer(ImageDescriptorsPointer, name: nameof(ImageDescriptorsPointer));
				ImageBufferMemoryPointerPointer = s.Serialize<uint>(ImageBufferMemoryPointerPointer, name: nameof(ImageBufferMemoryPointerPointer));
				UInt_1C = s.Serialize<uint>(UInt_1C, name: nameof(UInt_1C));
				Byte_20 = s.Serialize<byte>(Byte_20, name: nameof(Byte_20));
				Byte_21 = s.Serialize<byte>(Byte_21, name: nameof(Byte_21));
				Byte_22 = s.Serialize<byte>(Byte_22, name: nameof(Byte_22));
				Byte_23 = s.Serialize<byte>(Byte_23, name: nameof(Byte_23));
				UShort_24 = s.Serialize<ushort>(UShort_24, name: nameof(UShort_24));
				UShort_26 = s.Serialize<ushort>(UShort_26, name: nameof(UShort_26));
			}

			// Serialize event states
			if (CurrentStatePointer != null) {
				// TODO: Find way to get the length
				// First look back to find current state index
				uint currentStateIndex = 1;
				while (true) {
					Pointer CheckPtr0 = null, CheckPtr1 = null;
					bool success = true;
					s.DoAt(CurrentStatePointer - currentStateIndex * 0xC + 0x2, () => {
						try {
							CheckPtr0 = s.SerializePointer(CheckPtr0, name: nameof(CheckPtr0));
							CheckPtr1 = s.SerializePointer(CheckPtr1, name: nameof(CheckPtr1));
						} catch (Exception) {
							success = false;
						}
					});
					if (!success
					|| (CheckPtr0 != null && CheckPtr0.file != CurrentStatePointer.file)
					|| (CheckPtr1 != null && CheckPtr1.file != CurrentStatePointer.file)) {
						currentStateIndex--;
						break;
					}
					currentStateIndex++;
				}
				// Serialize from first state index
				s.DoAt(CurrentStatePointer - currentStateIndex * 0xC, () => {
					var temp = new List<Jaguar_R1_EventState>();

					var index = 0;
					while (true) {

						if (s.CurrentPointer.AbsoluteOffset > CurrentStatePointer.AbsoluteOffset) {
							Pointer CheckPtr0 = null, CheckPtr1 = null;
							bool success = true;
							s.DoAt(s.CurrentPointer + 0x2, () => {
								try {
									CheckPtr0 = s.SerializePointer(CheckPtr0, name: nameof(CheckPtr0));
									CheckPtr1 = s.SerializePointer(CheckPtr1, name: nameof(CheckPtr1));
								} catch (Exception) {
									success = false;
								}
							});
							if (!success
							|| (CheckPtr0 != null && CheckPtr0.file != CurrentStatePointer.file)
							|| (CheckPtr1 != null && CheckPtr1.file != CurrentStatePointer.file)) {
								break;
							}
							/*
							byte[] CheckBytes = default;
							s.DoAt(s.CurrentPointer + 0xA, () => {
								CheckBytes = s.SerializeArray<byte>(CheckBytes, 2, name: nameof(CheckBytes));
							});
							if (CheckBytes[0] != 0 || CheckBytes[1] != 0xFF)
							{
								break;
							}*/
						}
						var i = s.SerializeObject<Jaguar_R1_EventState>(default, name: $"{nameof(States)}[{index}]");

						temp.Add(i);

						index++;
					}

					States = temp.ToArray();
				});
			} else if (ComplexDataPointer != null) {
				if (!(StructType2 == 30 && StructType1 == 5)) { // Different struct in this case, that only has states with code pointers
					s.DoAt(ComplexDataPointer, () => {
						ComplexData = s.SerializeObject<Jaguar_R1_EventComplexData>(ComplexData, onPreSerialize: g => g.StructType = StructType2, name: nameof(ComplexData));
					});
				}
			}

			// Serialize image descriptors based on the state animations
			s.DoAt(ImageDescriptorsPointer, () => {
				// TODO: This doesn't seem to work consistently at all - fallback to previous method for now
				if (States != null && States.Length > 0) {
					int maxImageIndex = States
						.Where(x => x?.Animation != null)
						.SelectMany(x => x.Animation.Layers)
						.Max(x => UShort_12 == 5 ? BitHelpers.ExtractBits(x.ImageIndex, 7, 0) : x.ImageIndex);
					ImageDescriptors = s.SerializeObjectArray<Common_ImageDescriptor>(ImageDescriptors, maxImageIndex + 1, name: nameof(ImageDescriptors));
					//Debug.Log(ImageDescriptors.Length);
				} else {
					var temp = new List<Common_ImageDescriptor>();

					var index = 0;
					while (true) {
						var i = s.SerializeObject<Common_ImageDescriptor>(default, name: $"{nameof(ImageDescriptors)}[{index}]");

						if (temp.Any() && i.Index != 0xFF && i.ImageBufferOffset < temp.Last().ImageBufferOffset)
							break;

						temp.Add(i);

						index++;
					}

					ImageDescriptors = temp.ToArray();
				}
			});
		}

		#endregion
	}
}