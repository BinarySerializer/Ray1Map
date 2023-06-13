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

        public static EngineVersionTree Create_Jade(GameSettings settings) 
        {
            EngineVersionTree tree = new(
                new Node(EngineVersion.Jade_Engine) {
                    new(EngineVersion.Jade_Montpellier) {
                        new(EngineVersion.Jade_BGE) {
                            new(EngineVersion.Jade_BGE_HD),
                            new(EngineVersion.Jade_KingKong) {
                                new(EngineVersion.Jade_KingKong_Xenon),
                                new(EngineVersion.Jade_RRRPrototype) {
                                    new(EngineVersion.Jade_RRR) {
                                        new(EngineVersion.Jade_RRR2),
                                        new(EngineVersion.Jade_Phoenix) {
                                            new(EngineVersion.Jade_Horsez2) {
                                                new(EngineVersion.Jade_PetzHorseClub) {
                                                    new(EngineVersion.Jade_MovieGames)
                                                },
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                    new(EngineVersion.Jade_Montreal) {
                        new(EngineVersion.Jade_PoP_SoT_20030723) {
                            new(EngineVersion.Jade_PoP_SoT_20030819) {
                                new(EngineVersion.Jade_PoP_SoT) {
                                    new(EngineVersion.Jade_PoP_WW_20040920) {
                                        new(EngineVersion.Jade_PoP_WW_20041024) {
                                            new(EngineVersion.Jade_PoP_WW) {
                                                new(EngineVersion.Jade_PhoenixRayman4) {
                                                    new(EngineVersion.Jade_Horsez)
                                                },
                                                new(EngineVersion.Jade_Beowulf),
                                                new(EngineVersion.Jade_PoP_T2T_20051002) {
                                                    new(EngineVersion.Jade_PoP_T2T) { // BIG: 0x26
                                                        new(EngineVersion.Jade_TMNT) { // BIG: 0x2A
                                                            new(EngineVersion.Jade_MyWordCoach), // BIG: 0x2A
                                                            new(EngineVersion.Jade_PostTMNT) {
                                                                new(EngineVersion.Jade_CPP) { // bf name format: [game]_bin_[platform].bf
                                                                    new(EngineVersion.Jade_RRRTVParty) { // BIG: 0x2C
                                                                        new(EngineVersion.Jade_JustDance)
                                                                    },
                                                                    new(EngineVersion.Jade_PoP_TFS) { // BIG: 0x2A
                                                                        new(EngineVersion.Jade_Avatar) { // BIG: 0x2A
                                                                            new(EngineVersion.Jade_SeanWhiteSkateboarding) // BIG: 0x2A (but BF64!)
                                                                        },
                                                                        new(EngineVersion.Jade_PoP_TFS_PSP) // BIG: 0x2A
                                                                    },
                                                                    new(EngineVersion.Jade_Fox) { // BIG: 0x2A
                                                                        new(EngineVersion.Jade_Naruto1RiseOfANinja) { // BIG : 0x2A
                                                                            new(EngineVersion.Jade_Naruto2TheBrokenBond)
                                                                        },
                                                                    }
                                                                },
                                                                new(EngineVersion.Jade_CloudyWithAChanceOfMeatballs) { // BIG: 0x2B
                                                                    new(EngineVersion.Jade_NCIS)
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                });
            tree.Init();
            tree.Current = tree.FindVersion(settings.EngineVersion);
            
            return tree;
        }
    }
}