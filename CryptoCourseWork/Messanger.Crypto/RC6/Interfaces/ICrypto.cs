namespace Messanger.Crypto.RC6.Interfaces
{
    public interface ICrypto
    {        
        public byte[] Encrypt(byte[] block);
        
        public byte[] Decrypt(byte[] block);
    }
}