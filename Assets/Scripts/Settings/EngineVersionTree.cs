using BinarySerializer;

namespace R1Engine
{
    public class EngineVersionTree : VersionTree<EngineVersion>
    {
        public static EngineVersionTree Create(GameSettings settings)
        {
            return settings.MajorEngineVersion switch
            {
                MajorEngineVersion.Jade => Create_Jade(settings),
                _ => null
            };
        }

        public static EngineVersionTree Create_Jade(GameSettings settings) {
            EngineVersionTree tree = new EngineVersionTree() {
                Root = new Node(EngineVersion.Jade_Engine).SetChildren(
                    new Node(EngineVersion.Jade_Montpellier).SetChildren(
                        new Node(EngineVersion.Jade_BGE).SetChildren(
                            new Node(EngineVersion.Jade_BGE_HD),
                            new Node(EngineVersion.Jade_KingKong).SetChildren(
                                new Node(EngineVersion.Jade_KingKong_Xenon),
                                new Node(EngineVersion.Jade_RRR).SetChildren(
                                    new Node(EngineVersion.Jade_RRR2),
                                    new Node(EngineVersion.Jade_Phoenix).SetChildren(
                                        new Node(EngineVersion.Jade_Horsez2).SetChildren(
                                            new Node(EngineVersion.Jade_PetzHorseClub).SetChildren(
                                                new Node(EngineVersion.Jade_MovieGames)
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    ),
                    new Node(EngineVersion.Jade_Montreal).SetChildren(
                        new Node(EngineVersion.Jade_PoP_SoT_20030819).SetChildren(
                            new Node(EngineVersion.Jade_PoP_SoT).SetChildren(
                                new Node(EngineVersion.Jade_PoP_WW).SetChildren(
                                    new Node(EngineVersion.Jade_PhoenixRayman4).SetChildren(
                                        new Node(EngineVersion.Jade_Horsez)
                                    ),
                                    new Node(EngineVersion.Jade_PoP_T2T).SetChildren(
                                        new Node(EngineVersion.Jade_MyWordCoach),
                                        new Node(EngineVersion.Jade_PoP_TFS),
                                        new Node(EngineVersion.Jade_RRRTVParty),
                                        new Node(EngineVersion.Jade_CloudyWithAChanceOfMeatballs).SetChildren(
                                            new Node(EngineVersion.Jade_NCIS)
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            };
            tree.Init();
            tree.Current = tree.FindVersion(settings.EngineVersion);
            
            return tree;
        }
    }
}