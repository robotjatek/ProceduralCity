using System;

namespace ProceduralCity.Utils
{
    class CoinFlip
    {
        private readonly static Random _random = new Random();

        public static bool Flip()
        {
            return _random.Next(0, 10) % 2 == 0;
        }
    }
}
