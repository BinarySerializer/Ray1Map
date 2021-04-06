using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinarySerializer;

namespace R1Engine.Jade {
	public class WOR_WorldList : Jade_File {
		public Jade_GenericReference[] Worlds { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			Worlds = s.SerializeObjectArray<Jade_GenericReference>(Worlds, FileSize / 8, name: nameof(Worlds));
			foreach (var world in Worlds) {
				world?.Resolve(immediate: true);
				if (world?.Value != null && world.Value is WOR_World w) {
					var gaos = w.GameObjects?.Value?.GameObjects;
					if (gaos != null) {
						foreach (var gao in gaos) {
							// Resolve references in ANI_pst_Load
							var actionData = gao?.Value?.Visual?.ActionData;
							if (actionData != null) {
								actionData.Shape?.Resolve(immediate: true);
								actionData.GRP?.Resolve(immediate: true);
								if(actionData.ActionKit?.Value == null) actionData.ListTracks?.Resolve(immediate: true);
							}
						}
					}
				}
			}
		}
	}
}
