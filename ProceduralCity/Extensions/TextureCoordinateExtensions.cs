using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

namespace ProceduralCity.Extensions
{
    static class TextureCoordinateExtensions
    {

        /// <summary>Flips texture coordinates vertically.</summary>
        /// <param name="vectors">The vectors to flip.</param>
        /// <returns>Returns a new vector with flipped X-axis</returns>
        public static IEnumerable<Vector2> FlipTextureCoordinatesVertically(this IEnumerable<Vector2> vectors)
        {
            return vectors.Select(v => new Vector2(-v.X + 1, v.Y));
        }

        public static IEnumerable<Vector2> FlipTextureCoordinatesHorizontally(this IEnumerable<Vector2> vectors)
        {
            return vectors.Select(v => new Vector2(v.X, -v.Y + 1));
        }
    }
}
