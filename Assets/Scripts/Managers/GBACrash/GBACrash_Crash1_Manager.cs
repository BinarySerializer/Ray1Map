namespace R1Engine
{
    public class GBACrash_Crash1_Manager : GBACrash_BaseManager
    {
        public override LevInfo[] LevInfos => Levels;
        public override int LocTableCount => 70;

        // 25 levels
        public static LevInfo[] Levels = new LevInfo[]
        {
            new LevInfo(0, 0, "Jungle Jam"),
            new LevInfo(0, LevInfo.Type.Bonus, "Jungle Jam"),
            new LevInfo(1, 0, "Shipwrecked"),
            new LevInfo(2, 0, "Temple of Boom"),
            new LevInfo(2, LevInfo.Type.Bonus, "Temple of Boom"),
            new LevInfo(2, LevInfo.Type.Challenge, "Temple of Boom"),
            new LevInfo(3, 0, "Frostbite Cavern - Part 1"),
            new LevInfo(3, 1, "Frostbite Cavern - Part 2"),
            new LevInfo(3, 2, "Frostbite Cavern - Part 3"),
            new LevInfo(3, LevInfo.Type.Bonus, "Frostbite Cavern"),
            new LevInfo(4, 0, "Just in Slime"),
            new LevInfo(4, LevInfo.Type.Bonus, "Just in Slime"),
            new LevInfo(5, 0, "Snow Crash - Part 1"),
            new LevInfo(5, 1, "Snow Crash - Part 2"),
            new LevInfo(5, 2, "Snow Crash - Part 3"),
            new LevInfo(5, LevInfo.Type.Bonus, "Snow Crash"),
            new LevInfo(5, LevInfo.Type.Challenge, "Snow Crash"),
            new LevInfo(6, 0, "Rocket Racket"),
            new LevInfo(7, 0, "Just Hangin'"),
            new LevInfo(7, LevInfo.Type.Bonus, "Just Hangin'"),
            new LevInfo(7, LevInfo.Type.Challenge, "Just Hangin'"),
            new LevInfo(8, 0, "Shark Attack"),
            new LevInfo(9, 0, "Ruined"),
            new LevInfo(9, LevInfo.Type.Bonus, "Ruined"),
            new LevInfo(10, 0, "Snow Job - Part 1"),
            new LevInfo(10, 1, "Snow Job - Part 2"),
            new LevInfo(10, 2, "Snow Job - Part 3"),
            new LevInfo(10, LevInfo.Type.Bonus, "Snow Job"),
            new LevInfo(11, 0, "Ace of Space"),
            new LevInfo(11, LevInfo.Type.Bonus, "Ace of Space"),
            new LevInfo(11, LevInfo.Type.Challenge, "Ace of Space"),
            new LevInfo(12, 0, "Sunken City"),
            new LevInfo(13, 0, "Down the Hole"),
            new LevInfo(13, LevInfo.Type.Bonus, "Down the Hole"),
            new LevInfo(14, 0, "Blimp Bonanza"),
            new LevInfo(15, 0, "Star to Finish"),
            new LevInfo(15, LevInfo.Type.Bonus, "Star to Finish"),
            new LevInfo(16, 0, "Air Supply"),
            new LevInfo(17, 0, "No-fly Zone"),
            new LevInfo(18, 0, "Drip, drip, drip"),
            new LevInfo(18, LevInfo.Type.Bonus, "Drip, drip, drip"),
            new LevInfo(19, 0, "Final Countdown"),
            new LevInfo(19, LevInfo.Type.Bonus, "Final Countdown"),
            new LevInfo(20, 0, "Dingodile"),
            new LevInfo(21, 0, "N. Gin"),
            new LevInfo(22, 0, "Tiny"),
            new LevInfo(23, 0, "Neo Cortex"),
            new LevInfo(24, 0, "Mega-mix"),
        };
    }
}