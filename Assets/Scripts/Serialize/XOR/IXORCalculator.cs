namespace R1Engine
{
    /// <summary>
    /// Base interface for any class that XORs bytes in a way
    /// </summary>
    public interface IXORCalculator
    {
        byte XORByte(byte b);
    }
}