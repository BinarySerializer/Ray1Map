namespace R1Engine
{
    /// <summary>
    /// ETA data for PC
    /// </summary>
    public class PC_Eta : R1Serializable
    {
        /// <summary>
        /// The right speed
        /// </summary>
        public sbyte RightSpeed { get; set; }
        
        /// <summary>
        /// The left speed
        /// </summary>
        public sbyte LeftSpeed { get; set; }
        
        /// <summary>
        /// The animation index
        /// </summary>
        public byte AnimationIndex { get; set; }
        
        /// <summary>
        /// The etat value
        /// </summary>
        public byte Etat { get; set; }
        
        /// <summary>
        /// The sub-etat value
        /// </summary>
        public byte SubEtat { get; set; }
        
        /// <summary>
        /// The amount of frames to skip in the animation each second, or 0 for it to not animate
        /// </summary>
        public byte AnimationSpeed { get; set; }
        
        /// <summary>
        /// The sound index
        /// </summary>
        public byte SoundIndex { get; set; }
        
        /// <summary>
        /// The interaction type
        /// </summary>
        public byte InteractionType { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            RightSpeed = s.Serialize(RightSpeed, name: "RightSpeed");
            LeftSpeed = s.Serialize(LeftSpeed, name: "LeftSpeed");
            AnimationIndex = s.Serialize(AnimationIndex, name: "AnimationIndex");
            Etat = s.Serialize(Etat, name: "Etat");
            SubEtat = s.Serialize(SubEtat, name: "SubEtat");
            AnimationSpeed = s.Serialize(AnimationSpeed, name: "AnimationSpeed");
            SoundIndex = s.Serialize(SoundIndex, name: "SoundIndex");
            InteractionType = s.Serialize(InteractionType, name: "InteractionType");
        }
    }
}