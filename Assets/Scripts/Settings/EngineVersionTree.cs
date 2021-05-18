using System.Collections.Generic;

namespace R1Engine {
    public class EngineVersionTree {
        public Node Root { get; set; }
        public Node Current { get; set; }

        public void Init() {
            Root.PropagateParents();
        }

        public bool HasParent(EngineVersion version) => Current.HasParent(version);
        public Node FindEngineVersion(EngineVersion version) => Root.FindEngineVersion(version);

        public class Node {
            public EngineVersion Version { get; set; }

            public Node[] Children { get; set; }

            protected HashSet<EngineVersion> ParentVersions { get; set; } = new HashSet<EngineVersion>();

            public bool HasParent(EngineVersion version) => Version == version || ParentVersions.Contains(version);
            public Node FindEngineVersion(EngineVersion version) {
                if(Version == version) return this;
                if(Children == null) return null;
                foreach (var child in Children) {
                    var result = child.FindEngineVersion(version);
                    if(result != null) return result;
                }
                return null;
            }

            public void PropagateParents() {
                if (Children == null) return;
                foreach (var child in Children) {
                    foreach (var pv in ParentVersions)
                        child.ParentVersions.Add(pv);
                    child.ParentVersions.Add(Version);
                    child.PropagateParents();
                }
            }

            public Node(EngineVersion version) {
                Version = version;
            }

            public Node SetChildren(params Node[] nodes) {
                Children = nodes;
                return this;
            }
        }

        public static EngineVersionTree Create(GameSettings settings) {
            switch (settings.MajorEngineVersion) {
                case MajorEngineVersion.Jade: return Create_Jade(settings);
                default: return null;
            }
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
                        new Node(EngineVersion.Jade_PoP_SoT).SetChildren(
                            new Node(EngineVersion.Jade_PoP_WW).SetChildren(
                                new Node(EngineVersion.Jade_PhoenixRayman4).SetChildren(
                                    new Node(EngineVersion.Jade_Horsez)
                                ),
                                new Node(EngineVersion.Jade_PoP_T2T).SetChildren(
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
            };
            tree.Init();
            tree.Current = tree.FindEngineVersion(settings.EngineVersion);
            
            return tree;
        }
    }
}