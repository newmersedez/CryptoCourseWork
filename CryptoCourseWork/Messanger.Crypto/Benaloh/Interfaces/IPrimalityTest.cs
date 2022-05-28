using System.Numerics;

namespace Messanger.Crypto.Benaloh.Interfaces
{
    public enum PrimalityTestMode
    {
        Fermat,
        SolovayStrassen,
        MillerRabin
    }
    
    internal interface IPrimalityTest
    {
        public bool SimplicityTest(BigInteger n, double minProbability);
    }
}