using System;
using BinarySerializer;

namespace R1Engine.Jade
{
    public class COL_EventObject : BinarySerializable {
        public uint Count { get; set; }
        public GEObject[] EventObjects { get; set; }

        public override void SerializeImpl(SerializerObject s) {
			Count = s.Serialize<uint>(Count, name: nameof(Count));
			EventObjects = s.SerializeObjectArray<GEObject>(EventObjects, Count, name: nameof(EventObjects));
		}

		public class GEObject : BinarySerializable {
            public uint Type { get; set; }
            public GELine Line { get; set; }
			public override void SerializeImpl(SerializerObject s) {
				Type = s.Serialize<uint>(Type, name: nameof(Type));
                if (Type == 1 || Type == 2) {
					Line = s.SerializeObject<GELine>(Line, name: nameof(Line));
				}
			}
		}

		public class GELine : BinarySerializable {
            public GEOSubType_T2T SubType { get; set; }
            public GEOSubType_TFS SubTypeTFS { get; set; }
            public Jade_Vector[] Points { get; set; }
            public Jade_Vector Normal { get; set; }
            public uint UInt_00 { get; set; }
            public uint SoundMaterial { get; set; }

			public override void SerializeImpl(SerializerObject s) {
                if (s.GetR1Settings().EngineVersionTree.HasParent(EngineVersion.Jade_PoP_TFS)) {
					SubTypeTFS = s.Serialize<GEOSubType_TFS>(SubTypeTFS, name: nameof(SubTypeTFS));
				} else {
                    SubType = s.Serialize<GEOSubType_T2T>(SubType, name: nameof(SubType));
                }
				Points = s.SerializeObjectArray<Jade_Vector>(Points, 2, name: nameof(Points));
				Normal = s.SerializeObject<Jade_Vector>(Normal, name: nameof(Normal));
				UInt_00 = s.Serialize<uint>(UInt_00, name: nameof(UInt_00));
				if (SubType.HasFlag(GEOSubType_T2T.GEO_LoadSndAIMaterial) || SubTypeTFS.HasFlag(GEOSubType_TFS.GEO_LoadSndAIMaterial)) {
                    SoundMaterial = s.Serialize<uint>(SoundMaterial, name: nameof(SoundMaterial));
                }

			}

            [Flags]
            public enum GEOSubType_T2T : uint {
                None                        = 0,
                LedgeGrabWall               = 1 << 0,
                LedgeGrabWallEdge           = 1 << 1,
                LedgeGrabFree               = 1 << 2,
                LedgeGrabFreeEdge           = 1 << 3,
                Ladder                      = 1 << 4,
                Column                      = 1 << 5,
                Rope                        = 1 << 6,
                HandlePole                  = 1 << 7,
                HandleBox                   = 1 << 8,
                AutoOrientHelper            = 1 << 9,
                BeamWall                    = 1 << 10,
                FlatRope                    = 1 << 11,
                SwingPole                   = 1 << 12,
                LedgeGrabWallNoClimb        = 1 << 13,
                LedgeGrabFreeNoClimb        = 1 << 14,
                LedgeGrabFreeSwing          = 1 << 15,
                LedgeGrabFreeEdgeSwing      = 1 << 16,
                LedgeGrabFreeNoClimbSwing   = 1 << 17,
                StairRail                   = 1 << 18,
                BeamFree                    = 1 << 19,
                HandleBoxNoClimb            = 1 << 20,
                WaterSource                 = 1 << 21,
                BeamWallNoClimb             = 1 << 22,
                TearableDrape               = 1 << 23,
                WallingRope                 = 1 << 24,
                CombatColumn                = 1 << 25,
                WallSlide                   = 1 << 26,
                ChainSwing                  = 1 << 27,
                LEdge_DaggerPlate           = 1 << 28,
                LEdge_ChainHook             = 1 << 29,
                NotUsed02                   = 1 << 30,
                GEO_LoadSndAIMaterial       = (uint)1 << 31
            }

            
            [Flags]
            public enum GEOSubType_TFS : uint {
                None                        = 0,
                LedgeGrabWall               = 1 << 0,
                LedgeGrabWallEdge           = 1 << 1,
                LedgeGrabFree               = 1 << 2,
                LedgeGrabFreeEdge           = 1 << 3,
                Goo                         = 1 << 4,
                Column                      = 1 << 5,
                Pillar                      = 1 << 6,
                Bubble                      = 1 << 7,
                HandleBox                   = 1 << 8,
                LedgeGrabWallVines          = 1 << 9,
                BeamWall                    = 1 << 10,
                Vines                       = 1 << 11,
                SwingPole                   = 1 << 12,
                LedgeGrabWallNoClimb        = 1 << 13,
                LedgeGrabFreeNoClimb        = 1 << 14,
                CrackGrabWall               = 1 << 15,
                LedgeGrabWallEdgeVines      = 1 << 16,
                AutoClimb                   = 1 << 17,
                StairRail                   = 1 << 18,
                BeamFree                    = 1 << 19,
                HandleBoxNoClimb            = 1 << 20,
                WaterSource                 = 1 << 21,
                BeamWallNoClimb             = 1 << 22,
                LedgeGrabWallDrop           = 1 << 23,
                LedgeGrabFreeDrop           = 1 << 24,
                CombatColumn                = 1 << 25,
                WallSlide                   = 1 << 26,
                RingSwitch                  = 1 << 27,
                Grip                        = 1 << 28,
                GEO_Disabled                = 1 << 29,
                GEO_AutomaticLine           = 1 << 30,
                GEO_LoadSndAIMaterial       = (uint)1 << 31
            }
		}
	}
}