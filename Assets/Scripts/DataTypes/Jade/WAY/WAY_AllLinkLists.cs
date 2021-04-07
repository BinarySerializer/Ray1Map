using System;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WAY_AllLinkLists : Jade_File {
		public ushort Count { get; set; }
		public ushort UseLongFormat { get; set; }
		public LinkList[] LinkLists { get; set; }
		public override void SerializeImpl(SerializerObject s) {
			s.SerializeBitValues<uint>(bitFunc => {
				Count = (ushort)bitFunc(Count, 16, name: nameof(Count));
				UseLongFormat = (ushort)bitFunc(UseLongFormat, 16, name: nameof(UseLongFormat));
			});
			LinkLists = s.SerializeObjectArray<LinkList>(LinkLists, Count, onPreSerialize: ll => ll.UseLongFormat = UseLongFormat, name: nameof(LinkLists));
		}

		public class LinkList : BinarySerializable {
			public ushort UseLongFormat { get; set; } // Set in onPreSerialize

			public Jade_Reference<WAY_Network> Network { get; set; }
			public uint Count { get; set; }
			public Link[] Links { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Network = s.SerializeObject<Jade_Reference<WAY_Network>>(Network, name: nameof(Network))?
					.Resolve(flags: LOA_Loader.ReferenceFlags.Log | LOA_Loader.ReferenceFlags.Flag6);
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Links = s.SerializeObjectArray<Link>(Links, Count, onPreSerialize: l => l.UseLongFormat = UseLongFormat, name: nameof(Links));
			}

			public class Link : BinarySerializable {
				public ushort UseLongFormat { get; set; } // Set in onPreSerialize

				public uint Unknown { get; set; }
				public Jade_Reference<OBJ_GameObject> GameObject { get; set; }
				public byte Byte_08 { get; set; }
				public short Short_09 { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					if (UseLongFormat != 0) {
						Unknown = s.Serialize<uint>(Unknown, name: nameof(Unknown));
					} else {
						Unknown = s.Serialize<ushort>((ushort)Unknown, name: nameof(Unknown));
					}
					GameObject = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(GameObject, name: nameof(GameObject))?.Resolve();
					Byte_08 = s.Serialize<byte>(Byte_08, name: nameof(Byte_08));
					Short_09 = s.Serialize<short>(Short_09, name: nameof(Short_09));
				}
			}
		}
	}
}
