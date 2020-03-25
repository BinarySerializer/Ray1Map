namespace R1Engine
{
    /// <summary>
    /// ETA data for PC
    /// </summary>
    public class PC_ETA : R1Serializable
    {
        /// <summary>
        /// The event states, order by Etat and SubEtat
        /// </summary>
        public Common_EventState[][] States { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            States = s.SerializeArraySize<Common_EventState[], byte>(States, name: "States");

            for (int i = 0; i < States.Length; i++)
            {
                States[i] = s.SerializeArraySize<Common_EventState, byte>(States[i], name: "States[" + i + "]");
                States[i] = s.SerializeObjectArray<Common_EventState>(States[i], States[i].Length, name: "States[" + i + "]");
            }
        }
    }
}