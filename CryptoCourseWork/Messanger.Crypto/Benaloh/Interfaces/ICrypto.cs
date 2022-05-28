using System.Numerics;

namespace Messanger.Crypto.Benaloh.Interfaces
{
    internal interface ICrypto
    {        
        public BigInteger Encrypt(BigInteger message);
        
        public BigInteger Decrypt(BigInteger message);

        public void GenerateKeys(BigInteger message);
    }
}