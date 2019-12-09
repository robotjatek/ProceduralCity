using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Utils
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

        public static IEnumerable<Vector2> CreateGuiUVs()
        {
            return new[]
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(1, 0),
            };
        }

        public static IEnumerable<Vector3> CreateNDCFullscreenGuiVertices()
        {
            return new[]
            {
                new Vector3(-1, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, 1, 0),
                new Vector3(1, -1, 0),
                new Vector3(-1, -1, 0)
            };
        }

        public static IEnumerable<Vector2> CreateNDCFullscreenUVs()
        {
            return new[]
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
            };
        }

        public static IEnumerable<Vector3> CreateBottomVertices(Vector3 position, Vector2 area)
        {
            yield return new Vector3(position.X + area.X, position.Y, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y, position.Z);
            yield return new Vector3(position.X, position.Y, position.Z);
            yield return new Vector3(position.X + area.X, position.Y, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y, position.Z);
            yield return new Vector3(position.X, position.Y, position.Z + area.Y);
        }

        public static IEnumerable<Vector3> CreateTopVertices(Vector3 position, Vector2 area, float height)
        {
            yield return new Vector3(position.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y + height, position.Z);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y + height, position.Z);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z);
        }

        public static IEnumerable<Vector3> CreateLeftVertices(Vector3 position, Vector2 area, float height)
        {
            yield return new Vector3(position.X, position.Y, position.Z);
            yield return new Vector3(position.X, position.Y + height, position.Z);
            yield return new Vector3(position.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y, position.Z);
            yield return new Vector3(position.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y, position.Z + area.Y);
        }

        public static IEnumerable<Vector3> CreateRightVertices(Vector3 position, Vector2 area, float height)
        {
            yield return new Vector3(position.X + area.X, position.Y, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y, position.Z);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z);
            yield return new Vector3(position.X + area.X, position.Y, position.Z);
        }

        public static IEnumerable<Vector3> CreateFrontVertices(Vector3 position, Vector2 area, float height)
        {
            yield return new Vector3(position.X, position.Y, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y, position.Z + area.Y);
            yield return new Vector3(position.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z + area.Y);
            yield return new Vector3(position.X + area.X, position.Y, position.Z + area.Y);
        }

        public static IEnumerable<Vector3> CreateBacksideVertices(Vector3 position, Vector2 area, float height)
        {
            yield return new Vector3(position.X + area.X, position.Y, position.Z);
            yield return new Vector3(position.X + area.X, position.Y + height, position.Z);
            yield return new Vector3(position.X, position.Y + height, position.Z);
            yield return new Vector3(position.X + area.X, position.Y, position.Z);
            yield return new Vector3(position.X, position.Y + height, position.Z);
            yield return new Vector3(position.X, position.Y, position.Z);
        }

        public static IEnumerable<Vector2> CreateBottomUVs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
        }

        public static IEnumerable<Vector2> CreateZeroTopUVs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
        }

        public static IEnumerable<Vector2> CreateTopUVs(float height, float width)
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, height);
            yield return new Vector2(width, 0);
            yield return new Vector2(width, 0);
            yield return new Vector2(0, height);
            yield return new Vector2(height, width);
        }

        public static IEnumerable<Vector2> CreateLeftUVs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 1);
            yield return new Vector2(0, 0);
            yield return new Vector2(1, 1);
            yield return new Vector2(1, 0);
        }

        public static IEnumerable<Vector2> CreateRightUVs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 0);
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 1);
            yield return new Vector2(1, 0);
        }

        public static IEnumerable<Vector2> CreateFrontUvs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 0);
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 1);
            yield return new Vector2(1, 0);
        }

        public static IEnumerable<Vector2> CreateBackUVs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 1);
            yield return new Vector2(0, 0);
            yield return new Vector2(1, 1);
            yield return new Vector2(1, 0);
        }

        public static IEnumerable<Vector3> CreateCubeVertices(Vector3 position, Vector2 area, float height)
        {
            var vertices = new List<Vector3>();
            vertices.AddRange(CreateBacksideVertices(position, area, height));
            vertices.AddRange(CreateFrontVertices(position, area, height));
            vertices.AddRange(CreateRightVertices(position, area, height));
            vertices.AddRange(CreateLeftVertices(position, area, height));
            vertices.AddRange(CreateTopVertices(position, area, height));
            vertices.AddRange(CreateBottomVertices(position, area));
            return vertices;
        }

        public static IEnumerable<Vector2> CreateCubeUVs()
        {
            var UVs = new List<Vector2>();
            UVs.AddRange(CreateBackUVs());
            UVs.AddRange(CreateFrontUvs());
            UVs.AddRange(CreateRightUVs());
            UVs.AddRange(CreateLeftUVs());
            UVs.AddRange(CreateZeroTopUVs());
            UVs.AddRange(CreateBottomUVs());
            return UVs;
        }

        public static IEnumerable<Vector2> CreateZeroCubeUVs()
        {
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);

            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);

            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);

            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);

            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);

            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
            yield return new Vector2(0, 0);
        }
    }
}
