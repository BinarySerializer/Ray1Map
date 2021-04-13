using System;

namespace R1Engine
{
    [Flags]
    public enum R1_FinBossLevel : ushort
    {
        None = 0,

        Bzzit = 1 << 0,
        Moskito = 1 << 1,
        MrSax = 1 << 2,
        MrStone = 1 << 3,
        VikingMama = 1 << 4,
        SpaceMama = 1 << 5,
        MrSkops = 1 << 6,
        MrDark = 1 << 7, // fin_dark, fin_du_jeu

        Flag_08 = 1 << 8,
        HelpedJoe1 = 1 << 9, // This indicates if Eat at Joe's has been completed
        HelpedJoe2 = 1 << 10, // joe_exp_probleme, indicates if the light switch has been activated
        HelpedMusician = 1 << 11,
    }
}