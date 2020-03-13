using System;
using System.Linq;

namespace R1Engine
{
    // TODO: Make this class work for non-PC versions too
    /// <summary>
    /// General event information
    /// </summary>
    public class GeneralEventInfoData : IEquatable<GeneralEventInfoData>
    {
        #region Constructor

        public GeneralEventInfoData(string name, string mapperId, EventWorld? world, int type, int etat, int subEtat, EventFlag? flag, int des, int eta, int offsetBx, int offsetBy, int offsetHy, int followSprite, int hitPoints, int unkGroup, int hitSprite, int followEnabled, string[] connectedEvents, ushort[] labelOffsets, byte[] commands)
        {
            Name = name;
            MapperID = mapperId;
            World = world;
            Type = type;
            Etat = etat;
            SubEtat = subEtat;
            Flag = flag;
            DES = des;
            ETA = eta;
            OffsetBX = offsetBx;
            OffsetBY = offsetBy;
            OffsetHY = offsetHy;
            FollowSprite = followSprite;
            HitPoints = hitPoints;
            UnkGroup = unkGroup;
            HitSprite = hitSprite;
            FollowEnabled = followEnabled;
            ConnectedEvents = connectedEvents ?? new string[0];
            LabelOffsets = labelOffsets ?? new ushort[0];
            Commands = commands ?? new byte[0];
        }

        #endregion

        #region Public Properties

        public string Name { get; }

        public string MapperID { get; }
        
        public EventWorld? World { get; }
        
        public int Type { get; }

        public int Etat { get; }

        public int SubEtat { get; }

        public EventFlag? Flag { get; }

        public int DES { get; }

        public int ETA { get; }

        public int OffsetBX { get; }

        public int OffsetBY { get; }

        public int OffsetHY { get; }

        public int FollowSprite { get; }

        public int HitPoints { get; }

        public int UnkGroup { get; }

        public int HitSprite { get; }

        public int FollowEnabled { get; }

        public string[] ConnectedEvents { get; }

        public ushort[] LabelOffsets { get; }

        public byte[] Commands { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if the other instance is equals to the current one
        /// </summary>
        /// <param name="other">The other instance to compare to the current one</param>
        /// <returns>True if the other instance is equals to the current one, false if not</returns>
        public bool Equals(GeneralEventInfoData other) => other != null &&
                                                          World == other.World &&
                                                          Type == other.Type &&
                                                          Etat == other.Etat &&
                                                          SubEtat == other.SubEtat &&
                                                          DES == other.DES &&
                                                          OffsetBX == other.OffsetBX &&
                                                          OffsetBY == other.OffsetBY &&
                                                          OffsetHY == other.OffsetHY &&
                                                          FollowSprite == other.FollowSprite &&
                                                          HitPoints == other.HitPoints &&
                                                          UnkGroup == other.UnkGroup &&
                                                          HitSprite == other.HitSprite &&
                                                          FollowEnabled == other.FollowEnabled &&
                                                          LabelOffsets.SequenceEqual(other.LabelOffsets) &&
                                                          Commands.SequenceEqual(other.Commands);

        /// <summary>
        /// True if the specified object equals the current instance
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is GeneralEventInfoData id && Equals(id);

        /// <summary>
        /// Gets the object hash code
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = World.GetHashCode();
                hashCode = (hashCode * 397) ^ Type;
                hashCode = (hashCode * 397) ^ Etat;
                hashCode = (hashCode * 397) ^ SubEtat;
                hashCode = (hashCode * 397) ^ Flag.GetHashCode();
                hashCode = (hashCode * 397) ^ DES;
                hashCode = (hashCode * 397) ^ ETA;
                hashCode = (hashCode * 397) ^ OffsetBX;
                hashCode = (hashCode * 397) ^ OffsetBY;
                hashCode = (hashCode * 397) ^ OffsetHY;
                hashCode = (hashCode * 397) ^ FollowSprite;
                hashCode = (hashCode * 397) ^ HitPoints;
                hashCode = (hashCode * 397) ^ UnkGroup;
                hashCode = (hashCode * 397) ^ HitSprite;
                hashCode = (hashCode * 397) ^ FollowEnabled;
                hashCode = (hashCode * 397) ^ (LabelOffsets != null ? LabelOffsets.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Commands != null ? Commands.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion

        #region Static Operators

        /// <summary>
        /// Checks if the two items are the same
        /// </summary>
        /// <param name="a">The first item</param>
        /// <param name="b">The second item</param>
        /// <returns>True if they are the same, false if not</returns>
        public static bool operator ==(GeneralEventInfoData a, GeneralEventInfoData b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        /// <summary>
        /// Checks if the two items are not the same
        /// </summary>
        /// <param name="a">The first item</param>
        /// <param name="b">The second item</param>
        /// <returns>True if they are not the same, false if they are</returns>
        public static bool operator !=(GeneralEventInfoData a, GeneralEventInfoData b)
        {
            return !(a == b);
        }

        #endregion
    }
}