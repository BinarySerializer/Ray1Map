using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class WAY_AllNetworks : Jade_File {
		public uint NetworksCount { get; set; }
		public Jade_Reference<WAY_Network>[] Networks { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			NetworksCount = s.Serialize<uint>(NetworksCount, name: nameof(NetworksCount));
			Networks = s.SerializeObjectArray<Jade_Reference<WAY_Network>>(Networks, NetworksCount, name: nameof(Networks));
			foreach (var reference in Networks) {
				reference.Resolve();
			}
		}
	}
}
