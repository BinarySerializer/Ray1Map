using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.XMW {
	public class XMW_Command : BinarySerializable {
		public Command CommandType { get; set; }
		public uint ParamUInt { get; set; }
		public ushort ParamUShort { get; set; }

		public ushort Type { get; set; }
		public uint Address { get; set; }
		public uint StructSize { get; set; }

		public byte StructPosition_Byte_00 { get; set; }
		public uint StructPosition_Position { get; set; }
		public byte StructPosition_Byte_05 { get; set; }

		public ushort StructPointer_UShort_00 { get; set; }
		public byte StructPointer_Byte_02 { get; set; }
		public Pointer StructPointer_Pointer { get; set; }
		
		public string String { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			CommandType = s.Serialize<Command>(CommandType, name: nameof(CommandType));
			switch (CommandType) {
				case Command.Unk23:
					break;
				case Command.Unk06:
				case Command.Unk36:
					ParamUInt = s.Serialize<uint>(ParamUInt, name: nameof(ParamUInt));
					break;
				case Command.StructSize:
					StructSize = s.Serialize<uint>(StructSize, name: nameof(StructSize));
					break;
				case Command.StartAddress:
				case Command.EndAddress:
					Address = s.Serialize<uint>(Address, name: nameof(Address));
					break;
				case Command.Name:
				case Command.Compiler:
					String = s.SerializeString(String, name: nameof(String));
					break;
				case Command.Type:
					Type = s.Serialize<ushort>(Type, name: nameof(Type));
					break;
				case Command.StructPosition:
					StructPosition_Byte_00 = s.Serialize<byte>(StructPosition_Byte_00, name: nameof(StructPosition_Byte_00));
					StructPosition_Position = s.Serialize<uint>(StructPosition_Position, name: nameof(StructPosition_Position));
					StructPosition_Byte_05 = s.Serialize<byte>(StructPosition_Byte_05, name: nameof(StructPosition_Byte_05));
					break;
				case Command.TypeStructPointer:
					StructPointer_UShort_00 = s.Serialize<ushort>(StructPointer_UShort_00, name: nameof(StructPointer_UShort_00));
					StructPointer_Byte_02 = s.Serialize<byte>(StructPointer_Byte_02, name: nameof(StructPointer_Byte_02));
					StructPointer_Pointer = s.SerializePointer(StructPointer_Pointer, name: nameof(StructPointer_Pointer));
					break;
				case Command.TypeStruct:
					StructPointer_Pointer = s.SerializePointer(StructPointer_Pointer, name: nameof(StructPointer_Pointer));
					break;
				default:
					throw new NotImplementedException();
			}
		}

		public enum Command : ushort {
			StructPosition = 0x0006,
			Unk23 = 0x0023,
			Name = 0x0038,
			Type = 0x0055,
			TypeStruct = 0x0072,
			TypeStructPointer = 0x0083,
			StructSize = 0x00B6,
			Unk06 = 0x0106,
			StartAddress = 0x0111,
			EndAddress = 0x0121,
			Unk36 = 0x0136,
			Compiler = 0x0258,
		}
	}
}
