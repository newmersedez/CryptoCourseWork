namespace RC6.Interfaces
{
    public interface ICrypto
    {        
        public byte[] Encrypt(byte[] block);
        
        public byte[] Decrypt(byte[] block);
        
        // public void GenerateRoundKeys(byte[] key);       доделать потом, потому что в приложении есть смена ключа
    }
}