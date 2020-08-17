namespace R1Engine
{
    /// <summary>
    /// ETA data for PC
    /// </summary>
    public class R1_PC_ETA : R1Serializable
    {
        /// <summary>
        /// The event states, order by Etat and SubEtat
        /// </summary>
        public R1_EventState[][] States { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            States = s.SerializeArraySize<R1_EventState[], byte>(States, name: nameof(States));

            for (int i = 0; i < States.Length; i++)
            {
                States[i] = s.SerializeArraySize<R1_EventState, byte>(States[i], name: nameof(States) + "[" + i + "]");
                States[i] = s.SerializeObjectArray<R1_EventState>(States[i], States[i].Length, name: nameof(States) + "[" + i + "]");
            }
        }
    }
}