using BinarySerializer;
using Cysharp.Threading.Tasks;
using Ray1Map.Jade;
using static Ray1Map.Jade_BaseManager;

namespace Ray1Map {
	public class Jade_GameActions {
		public Jade_BaseManager Manager { get; set; }

		protected async UniTask<LOA_Loader> InitJadeAsync(Context context, bool initAI = true, bool initTextures = false, bool initSound = false) => await Manager.InitJadeAsync(context, initAI: initAI, initTextures: initTextures, initSound: initSound);
		protected async UniTask LoadFilesAsync(Context context) => await Manager.LoadFilesAsync(context);
		protected async UniTask<LOA_Loader> LoadJadeAsync(Context context, Jade_Key worldKey, LoadFlags loadFlags) => await Manager.LoadJadeAsync(context, worldKey, loadFlags);
		protected async UniTask<BIG_BigFile> LoadBF(Context context, string bfPath) => await Manager.LoadBF(context, bfPath);
		protected string[] BFFiles => Manager.BFFiles;
		protected LevelInfo[] LevelInfos => Manager.LevelInfos;
		protected bool HasUnbinarizedData => Manager.HasUnbinarizedData;



		protected string TexturesGearBFPath => Manager.TexturesGearBFPath;
		protected string GeometryBFPath => Manager.GeometryBFPath;
		protected string SoundGearBFPath => Manager.SoundGearBFPath;
		protected async UniTask<GEAR_BigFile> LoadGearBF(Context context, string bfPath) => await Manager.LoadGearBF(context, bfPath);


		public Jade_GameActions(Jade_BaseManager manager) {
			Manager = manager;
		}
	}
}
