using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class WAY_AllNetworks : Jade_File {
		public override string Export_Extension => "net";
		public uint NetworksCount { get; set; }
		public Entry[] Networks { get; set; }

		protected override void SerializeFile(SerializerObject s) {
			if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montpellier)) {
				NetworksCount = s.Serialize<uint>(NetworksCount, name: nameof(NetworksCount));
			} else {
				NetworksCount = FileSize / (uint)(Loader.IsBinaryData ? 4 : 8);
			}
			Networks = s.SerializeObjectArray<Entry>(Networks, NetworksCount, name: nameof(Networks));
			foreach (var reference in Networks) {
				reference.Resolve();
			}
		}

		public class Entry : BinarySerializable {
			public Jade_Reference<WAY_Network> Network { get; set; }
			public uint Editor_UInt_00 { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Network = s.SerializeObject<Jade_Reference<WAY_Network>>(Network, name: nameof(Network));

				LOA_Loader Loader = Context.GetStoredObject<LOA_Loader>(Jade_BaseManager.LoaderKey);
				if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_Montreal) && !Loader.IsBinaryData) {
					Editor_UInt_00 = s.Serialize<uint>(Editor_UInt_00, name: nameof(Editor_UInt_00));
				}
			}

			public void Resolve() {
				Network.Resolve();
			}
		}
	}
}
