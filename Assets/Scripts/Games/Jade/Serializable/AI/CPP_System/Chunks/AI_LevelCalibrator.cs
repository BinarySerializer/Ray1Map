using BinarySerializer;

namespace Ray1Map.Jade {
	public class AI_LevelCalibrator : AI_Chunk {
		public uint LevelsCount { get; set; }
		public uint CalibratorsCount { get; set; }
		public Calibrator[] Calibrators { get; set; }

		public override void SerializeImpl(SerializerObject s) {
			base.SerializeImpl(s);
			LevelsCount = s.Serialize<uint>(LevelsCount, name: nameof(LevelsCount));
			CalibratorsCount = s.Serialize<uint>(CalibratorsCount, name: nameof(CalibratorsCount));
			if (LevelsCount != 0)
				Calibrators = s.SerializeObjectArray<Calibrator>(Calibrators, CalibratorsCount, c => c.Pre_LevelCalibrator = this, name: nameof(Calibrators));
		}
		public class Calibrator : BinarySerializable {
			public AI_LevelCalibrator Pre_LevelCalibrator { get; set; }
			public AI_SystemStringID Name { get; set; }
			public float[] LevelValues { get; set; }

			public override void SerializeImpl(SerializerObject s) {
				Name = s.SerializeObject<AI_SystemStringID>(Name, n => n.Pre_System = Pre_LevelCalibrator.Pre_System, name: nameof(Name));
				LevelValues = s.SerializeArray<float>(LevelValues, Pre_LevelCalibrator.LevelsCount, name: nameof(LevelValues));
			}
		}
	}
}
