using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity
{
    static class PrimitiveUtils
    {
        public static IEnumerable<Vector3> CreateSpriteVertices(Vector2 position, float width, float height)
        {
            return new[]
            {
                new Vector3(position.X, position.Y, 0.0f),
                new Vector3(position.X + width, position.Y, 0.0f),
                new Vector3(position.X, position.Y + height, 0.0f),
                new Vector3(position.X, position.Y + height, 0.0f),
                new Vector3(position.X + width, position.Y, 0.0f),
                new Vector3(position.X + width, position.Y + height, 0.0f)
            };
        }

        public static IEnumerable<Vector2> CreateSpriteUVs(Vector2 position, float width, float height)
        {
            return new[]
            {
                new Vector2(position.X, position.Y),
                new Vector2(position.X + width, position.Y),
                new Vector2(position.X, position.Y + height),
                new Vector2(position.X, position.Y + height),
                new Vector2(position.X + width, position.Y),
                new Vector2(position.X + width, position.Y + height),
            };
        }
    }
}
