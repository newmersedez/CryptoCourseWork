namespace Messanger.Crypto.RC6.Interfaces
{
    internal interface IKeysGenerator
    {
        public uint[] GenerateRoundKeys(byte[] key, uint length);
    }
}