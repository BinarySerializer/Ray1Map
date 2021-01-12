namespace R1Engine
{
    public class GBACrash_Crash2_Manager : GBACrash_BaseManager
    {
        public override LevInfo[] LevInfos => Levels;
        public override int LocTableCount => 191;
        public override int AnimSetsCount => 49;

        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(0, 0, "Island Intro"), 
            new LevInfo(1, 0, "Prints of Persia"), 
            new LevInfo(1, LevInfo.Type.Bonus, "Prints of Persia"), 
            new LevInfo(2, 0, "Lagoony Tunes"), 
            new LevInfo(3, 0, "Globe Trottin'"), 
            new LevInfo(4, 0, "Pharaoh's Funhouse"), 
            new LevInfo(4, LevInfo.Type.Bonus, "Pharaoh's Funhouse"), 
            new LevInfo(5, 0, "Runaway Rug"), 
            new LevInfo(5, LevInfo.Type.Bonus, "Runaway Rug"), 
            new LevInfo(6, 0, "Tiki Torture"), 
            new LevInfo(6, LevInfo.Type.Bonus, "Tiki Torture"), 
            new LevInfo(7, 0, "Hoppin' Coffins"), 
            new LevInfo(7, LevInfo.Type.Bonus, "Hoppin' Coffins"), 
            new LevInfo(8, 0, "Barrel Roll"), 
            new LevInfo(9, 0, "Flockful of Seagulls"), 
            new LevInfo(10, 0, "Magma Mania"), 
            new LevInfo(10, LevInfo.Type.Bonus, "Magma Mania"), 
            new LevInfo(11, 0, "Run from the Sun"), 
            new LevInfo(12, 0, "Now it's Istanbul"), 
            new LevInfo(12, LevInfo.Type.Bonus, "Now it's Istanbul"), 
            new LevInfo(13, 0, "Mister Lava Lava"), 
            new LevInfo(13, LevInfo.Type.Bonus, "Mister Lava Lava"), 
            new LevInfo(14, 0, "Water Logged"), 
            new LevInfo(15, 0, "Slip-n-slidin' Sphinx"), 
            new LevInfo(15, LevInfo.Type.Bonus, "Slip-n-slidin' Sphinx"), 
            new LevInfo(16, 0, "Rocks can Roll"), 
            new LevInfo(17, 0, "Rock the Casaba"), 
            new LevInfo(17, LevInfo.Type.Bonus, "Rock the Casaba"), 
            new LevInfo(18, 0, "Eruption Disruption"), 
            new LevInfo(18, LevInfo.Type.Bonus, "Eruption Disruption"), 
            new LevInfo(19, 0, "Spaced Out"), 
            new LevInfo(20, 0, "King too Uncommon"), 
            new LevInfo(20, LevInfo.Type.Bonus, "King too Uncommon"), 
            new LevInfo(21, 0, "Wild Nile Ride"), 
            new LevInfo(22, 0, "101 Arabian Kites"), 
            new LevInfo(23, 0, "Fire Walker"), 
            new LevInfo(24, 0, "Evil Crunch"), 
            new LevInfo(25, 0, "Evil Coco"), 
            new LevInfo(26, 0, "Fake Crash"), 
            new LevInfo(27, 0, "N. Trance - Part 1"), 
            new LevInfo(27, 1, "N. Trance - Part 2"), 
            new LevInfo(28, 0, "N. Tropy - Part 1"), 
            new LevInfo(28, 1, "N. Tropy - Part 2"), 
            new LevInfo(28, 2, "N. Tropy - Part 3"), 
        };
    }
}