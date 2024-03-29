﻿using BinarySerializer;
using Cysharp.Threading.Tasks;
using System.IO;
using System.Linq;
using Ray1Map.DWARF1;

namespace Ray1Map
{
    public class Jade_KingKong_GC_Manager : Jade_Montpellier_BaseManager {
		// Levels
		public override LevelInfo[] LevelInfos => new LevelInfo[] {
			new LevelInfo(0x0002EA59, "ROOT/EngineDatas/06 Levels/_main/_main_bootup", "_main_bootup.wol"),
			new LevelInfo(0x000129BE, "ROOT/EngineDatas/06 Levels/_main/_main_credits", "_main_credits.wol"),
			new LevelInfo(0x0002D989, "ROOT/EngineDatas/06 Levels/_main/_main_E3EndScreeen", "_main_E3EndScreeen.wol"),
			new LevelInfo(0x0000C452, "ROOT/EngineDatas/06 Levels/_main/_main_fix", "_main_fix.wol"),
			new LevelInfo(0x0000C456, "ROOT/EngineDatas/06 Levels/_main/_main_logo", "_main_logo.wol"),
			new LevelInfo(0x0000C45A, "ROOT/EngineDatas/06 Levels/_main/_main_menu", "_main_menu.wol"),
			new LevelInfo(0x0002DE8F, "ROOT/EngineDatas/06 Levels/_main/_main_pad", "_main_pad.wol"),
			new LevelInfo(0x00009BAF, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkCreature", "Level_Bonus_ArtworkCreature.wol"),
			new LevelInfo(0x00009B97, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkEnv2", "Level_Bonus_ArtworkEnv2.wol"),
			new LevelInfo(0x00009BA3, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkEnv3", "Level_Bonus_ArtworkEnv3.wol"),
			new LevelInfo(0x00009BBB, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkKong1", "Level_Bonus_ArtworkKong1.wol"),
			new LevelInfo(0x00009B65, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkKong2", "Level_Bonus_ArtworkKong2.wol"),
			new LevelInfo(0x00009B7E, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkKong3", "Level_Bonus_ArtworkKong3.wol"),
			new LevelInfo(0x00009B8B, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_ArtworkKong4", "Level_Bonus_ArtworkKong4.wol"),
			new LevelInfo(0x000222D4, "ROOT/EngineDatas/06 Levels/_main/Level_Bonus_Jack", "Level_Bonus_Jack.wol"),
			new LevelInfo(0x00008943, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/01A_Venture", "01A_Venture.wol"),
			new LevelInfo(0x00006177, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/01B_Skull_Island", "01B_Skull_Island.wol"),
			new LevelInfo(0x00009BB9, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/02A_Tombs", "02A_Tombs.wol"),
			new LevelInfo(0x00011976, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/02C_Ann_s_capture_Part_1", "02C_Ann_s_capture_Part_1.wol"),
			new LevelInfo(0x000145B1, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/02C_Ann_s_capture_Part_2", "02C_Ann_s_capture_Part_2.wol"),
			new LevelInfo(0x0000975D, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03A_Escape_from_natives", "03A_Escape_from_natives.wol"),
			new LevelInfo(0x00004F21, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03A_Escape_from_natives_return", "03A_Escape_from_natives_retour.wol"),
			new LevelInfo(0x000134E8, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03B_On_Kongs_Tracks", "03B_On_Kongs_Tracks.wol"),
			new LevelInfo(0x000060E9, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03B_On_Kongs_Tracks_retour", "03B_On_Kongs_Tracks_retour.wol"),
			new LevelInfo(0x00008887, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03C_Hayes_is_back", "03C_Hayes_is_back.wol"),
			new LevelInfo(0x000057D6, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03C_Hayes_is_back_return", "03C_Hayes_is_back_retour.wol"),
			new LevelInfo(0x00001B17, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03E_Chased_by_The_Trex", "03E_Chased_by_The_Trex.wol"),
			new LevelInfo(0x00001683, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/03F_Ann_first_escape", "03F_Ann_first_escape.wol"),
			new LevelInfo(0x00002001, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/04A_Brontosaurs", "04A_Brontosaurs.wol"),
			new LevelInfo(0x00005EFA, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/04A_Brontosaurs/04A_Brontosaurs_part1", "04A_Brontosaurs_part1.wol"),
			new LevelInfo(0x00001793, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/04A_Brontosaurs_part2", "04A_Brontosaurs_part2.wol"),
			new LevelInfo(0x00002144, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/04B_To_the_raft", "04B_To_the_raft.wol"),
			new LevelInfo(0x0000B3B3, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/05A_On_the_raft_Part_1", "05A_On_the_raft_Part_1.wol"),
			new LevelInfo(0x0000BECA, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/05A_On_the_Raft_Part_2", "05A_On_the_Raft_Part_2.wol"),
			new LevelInfo(0x00001E3B, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/07B_Protection_Swamps", "07B_Protection_Swamps.wol"),
			new LevelInfo(0x000047D3, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/07B_Protection_Swamps_Part_2", "07B_Protection_Swamps_Part_2.wol"),
			new LevelInfo(0x00004D69, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/09B_Pit", "9B_Pit.wol"),
			new LevelInfo(0x000054C2, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/11A_Ann_alone", "11A_Ann_alone.wol"),
			new LevelInfo(0x000099AC, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/12A_In_the_temple", "12A_In_the_Temple.wol"),
			new LevelInfo(0x0000373A, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/12B_Blocked", "12B_Blocked.wol"),
			new LevelInfo(0x00009D63, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/14A_Call_Kong", "14A_Call_Kong.wol"),
			new LevelInfo(0x00012CF8, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/15A_Follow_The_Plane", "15A_Follow_The_Plane.wol"),
			new LevelInfo(0x000003EB, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/17A_Jack_Gets_To_The_Lair", "17A_Jack_Gets_To_The_Lair.wol"),
			new LevelInfo(0x000016AF, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/17A_Jack_Gets_To_The_Lair_Bis", "17A_Jack_Gets_To_The_Lair_Bis.wol"),
			new LevelInfo(0x00001B6F, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/18A_Down_from_the_lair", "18A_Down_from_the_lair.wol"),
			new LevelInfo(0x0000C697, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Jack/19B_Kong_is_inconscious", "19B_Kong_is_inconscious.wol"),
			new LevelInfo(0x0000EBEF, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/03D_Kong_On_His_Killing_Ground", "03D_Kong_On_His_Killing_Ground.wol"),
			new LevelInfo(0x0001EABC, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/05C_Kong_vs_first_Trex", "05C_Kong_vs_first_Trex.wol"),
			new LevelInfo(0x0000F858, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/07D_Kong_Saves_Ann", "07D_Kong_Saves_Ann.wol"),
			new LevelInfo(0x0000018C, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/10B_3_T-rex", "10B_3_T-rex.wol"),
			new LevelInfo(0x00000213, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/14B_Kong_to_the_rescue", "14B.wol"),
			new LevelInfo(0x00000C5A, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/17C_Fight_in_the_lair", "17C_Fight_in_the_lair.wol"),
			new LevelInfo(0x000048EF, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/19A_Kong_in_the_village", "19A_Kong_in_the_village.wol"),
			new LevelInfo(0x0000CD24, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/20A_New_York_Kongs_death", "20A_New_York_Kongs_death.wol"),
			new LevelInfo(0x0001FEDB, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/20A_New_York_theater", "20A_NY_theater.wol"),
			new LevelInfo(0x00000105, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/20C_Plane_In_New-York", "20C_Plane_In_New-York.wol"),
			new LevelInfo(0x0001468A, "ROOT/EngineDatas/06 Levels/PRODUCTION_Levels_Kong/21S_Back_to_KongIsland", "21S_Back_to_KongIsland.wol"),
		};

        // Version properties
		public override string[] BFFiles => new string[] {
			"KingKongTheGame.bf"
		};

		// Game actions
		public override GameAction[] GetGameActions(GameSettings settings) {
			return base.GetGameActions(settings).Concat(new GameAction[]
			{
				new GameAction("Export Struct Info", false, false, (input, output) => ExportStructInfo(settings, output)), // TODO: make it export later
			}).ToArray();
		}

		public virtual string ExecutablePath => "jadegc_ia2cr.elf";
		public virtual uint DebugSegmentAddress => 0x002d2e48;
		public virtual uint DebugSegmentLength => 0x1a9c30;

		public async UniTask ExportStructInfo(GameSettings settings, string outputDir) {
			using (var context = new Ray1MapContext(settings)) {
				// Load the files
				await context.AddLinearFileAsync(ExecutablePath);
				var s = context.Deserializer;

				byte[] bytes = null;
				s.DoAt(context.FilePointer(ExecutablePath) + DebugSegmentAddress, () => {
					bytes = s.SerializeArray<byte>(bytes, DebugSegmentLength, name: nameof(bytes));
				});
				const string key = "DebugSegment";
				MemoryStream ms = new MemoryStream(bytes);
				StreamFile sf = context.AddStreamFile(key, ms, endianness: Endian.Big, allowLocalPointers: true);

				DWARF_Segment Segment = null;
				Segment = FileFactory.Read<DWARF_Segment>(context, key);
			}
		}

	}
}
