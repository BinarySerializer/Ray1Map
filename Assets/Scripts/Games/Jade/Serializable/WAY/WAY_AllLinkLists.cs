using BinarySerializer;

namespace Ray1Map.Jade {
	public class WAY_AllLinkLists : Jade_File {
		public override string Export_Extension => "lnk";
		public override bool HasHeaderBFFile => true;

		public ushort Count { get; set; }
		public ushort UseLongFormat { get; set; }
		public LinkList[] LinkLists { get; set; }
		protected override void SerializeFile(SerializerObject s) {
			s.DoBits<uint>(b => {
				Count = (ushort)b.SerializeBits<int>(Count, 16, name: nameof(Count));
				UseLongFormat = (ushort)b.SerializeBits<int>(UseLongFormat, 16, name: nameof(UseLongFormat));
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
					.Resolve(flags: LOA_Loader.ReferenceFlags.MustExist | LOA_Loader.ReferenceFlags.Flag6);
				Count = s.Serialize<uint>(Count, name: nameof(Count));
				Links = s.SerializeObjectArray<Link>(Links, Count, onPreSerialize: l => l.UseLongFormat = UseLongFormat, name: nameof(Links));
			}

			public class Link : BinarySerializable {
				public ushort UseLongFormat { get; set; } // Set in onPreSerialize

				public uint Capacities { get; set; }
				public Jade_Reference<OBJ_GameObject> Next { get; set; }
				public byte Design { get; set; }
				public short Design2 { get; set; }

				public override void SerializeImpl(SerializerObject s) {
					if (UseLongFormat != 0) {
						Capacities = s.Serialize<uint>(Capacities, name: nameof(Capacities));
					} else {
						Capacities = s.Serialize<ushort>((ushort)Capacities, name: nameof(Capacities));
					}
					Next = s.SerializeObject<Jade_Reference<OBJ_GameObject>>(Next, name: nameof(Next))?.Resolve();
					Design = s.Serialize<byte>(Design, name: nameof(Design));
					Design2 = s.Serialize<short>(Design2, name: nameof(Design2));
				}
			}
		}
	}
}
