using System;

namespace ProceduralCity.Extensions
{
    static class MathExtensions
    {
        public static float Clamp(this float value, float min = 0.0f, float max = 1.0f)
        {
            return Math.Max(Math.Min(value, max), min);
        }
    }
}
