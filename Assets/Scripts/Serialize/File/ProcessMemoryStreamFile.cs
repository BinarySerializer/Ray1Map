namespace R1Engine.Serialize
{
    public class ProcessMemoryStreamFile : StreamFile
    {
        public ProcessMemoryStreamFile(string name, ProcessMemoryStream stream, Context context) : base(name, stream, context)
        { }

        public override Pointer StartPointer => new Pointer((uint)baseAddress, this);

        public override Pointer GetPointer(uint serializedValue, Pointer anchor = null)
        {
            uint anchorOffset = anchor?.AbsoluteOffset ?? 0;

            //if (serializedValue + anchorOffset >= baseAddress && serializedValue + anchorOffset < baseAddress + length)
                return new Pointer(serializedValue, this, anchor: anchor);

            //return null;
        }
    }
}