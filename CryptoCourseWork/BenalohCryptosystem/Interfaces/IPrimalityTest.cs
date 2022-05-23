using System.Numerics;

namespace BenalohCryptosystem
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