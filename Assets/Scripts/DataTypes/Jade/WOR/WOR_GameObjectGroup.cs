using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace R1Engine.Jade {
	public class WOR_GameObjectGroup : Jade_File {
		public Jade_Reference<OBJ_GameObject>[] GameObjects { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			GameObjects = s.SerializeObjectArray<Jade_Reference<OBJ_GameObject>>(GameObjects, FileSize / 4, name: nameof(GameObjects));
			foreach (var reference in GameObjects) {
				reference.Resolve();
			}
		}
	}
}
