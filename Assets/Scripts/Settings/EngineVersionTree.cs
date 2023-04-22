using BinarySerializer;

namespace Ray1Map
{
    public class EngineVersionTree : VersionTree<EngineVersion>
    {
        public EngineVersionTree(Node root) : base(root) { }

        public static EngineVersionTree Create(GameSettings settings)
        {
            return settings.MajorEngineVersion switch
            {
                MajorEngineVersion.Jade => Create_Jade(settings),
                _ => null
            };
        }

        public static EngineVersionTree Create_Jade(GameSettings settings) {
            EngineVersionTree tree = new EngineVersionTree(
                root: new Node(EngineVersion.Jade_Engine).SetChildren(
                    new Node(EngineVersion.Jade_Montpellier).SetChildren(
                        new Node(EngineVersion.Jade_BGE).SetChildren(
                            new Node(EngineVersion.Jade_BGE_HD),
                            new Node(EngineVersion.Jade_KingKong).SetChildren(
                                new Node(EngineVersion.Jade_KingKong_Xenon),
                                new Node(EngineVersion.Jade_RRRPrototype).SetChildren(
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
                        )
                    ),
                    new Node(EngineVersion.Jade_Montreal).SetChildren(
                        new Node(EngineVersion.Jade_PoP_SoT_20030723).SetChildren(
                            new Node(EngineVersion.Jade_PoP_SoT_20030819).SetChildren(
                                new Node(EngineVersion.Jade_PoP_SoT).SetChildren(
                                    new Node(EngineVersion.Jade_PoP_WW_20040920).SetChildren(
                                        new Node(EngineVersion.Jade_PoP_WW_20041024).SetChildren(
                                            new Node(EngineVersion.Jade_PoP_WW).SetChildren(
                                                new Node(EngineVersion.Jade_PhoenixRayman4).SetChildren(
                                                    new Node(EngineVersion.Jade_Horsez)
                                                ),
                                                new Node(EngineVersion.Jade_Beowulf),
                                                new Node(EngineVersion.Jade_PoP_T2T_20051002).SetChildren(
                                                    new Node(EngineVersion.Jade_PoP_T2T).SetChildren( // BIG: 0x26
														new Node(EngineVersion.Jade_TMNT).SetChildren( // BIG: 0x2A
															new Node(EngineVersion.Jade_MyWordCoach), // BIG: 0x2A
															new Node(EngineVersion.Jade_PostTMNT).SetChildren(
                                                                new Node(EngineVersion.Jade_Fox).SetChildren( // BIG: 0x2A
																	new Node(EngineVersion.Jade_Naruto1RiseOfANinja).SetChildren(
																		new Node(EngineVersion.Jade_Naruto2TheBrokenBond)
                                                                    )
                                                                ),
                                                                new Node(EngineVersion.Jade_CPP).SetChildren( // bf name format: [game]_bin_[platform].bf
																    new Node(EngineVersion.Jade_RRRTVParty).SetChildren( // BIG: 0x2C
																	    new Node(EngineVersion.Jade_JustDance)
																    ),
																    new Node(EngineVersion.Jade_PoP_TFS).SetChildren( // BIG: 0x2A
																	    new Node(EngineVersion.Jade_SeanWhiteSkateboarding) // BIG: 0x2A (but BF64!)
																    ),
																    new Node(EngineVersion.Jade_Avatar) // BIG: 0x2A
                                                                ),
                                                                new Node(EngineVersion.Jade_CloudyWithAChanceOfMeatballs).SetChildren( // BIG: 0x2B
                                                                    new Node(EngineVersion.Jade_NCIS)
                                                                )
                                                            )
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                ));
            tree.Init();
            tree.Current = tree.FindVersion(settings.EngineVersion);
            
            return tree;
        }
    }
}