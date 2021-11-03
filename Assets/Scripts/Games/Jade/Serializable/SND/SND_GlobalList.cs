using System;
using System.Collections.Generic;
using BinarySerializer;

namespace Ray1Map.Jade {
	public class SND_GlobalList {
		public List<SND_Wave> Waves { get; set; }

		public void AddWave(SND_Wave wav) {
			if (wav == null) return;
			if (Waves == null) Waves = new List<SND_Wave>();
			if (Waves.FindItem(w => w.Key == wav.Key) == null) Waves.Add(wav);
		}
	}
}
