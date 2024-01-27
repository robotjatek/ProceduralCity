using System.Collections.Generic;

using OpenTK.Mathematics;

using ProceduralCity.Renderer;

namespace ProceduralCity.Utils
{
    static class PrimitiveUtils
    {
        private static Mesh _trafficMesh;

        public static Mesh CreateTrafficMesh(Shader shader)
        {
            _trafficMesh ??= new Mesh(
                    vertices: CreateTrafficLightVertices(),
                    uvs: CreateTrafficLightUVs(),
                    shader: shader);

            return _trafficMesh;
        }

        private static IEnumerable<Vector3> CreateTrafficLightVertices()
        {
            yield return new Vector3(-0.75f, 0.75f, 0);
            yield return new Vector3(0.75f, -0.75f, 0);
            yield return new Vector3(0.75f, 0.75f, 0);

            yield return new Vector3(-0.75f, 0.75f, 0);
            yield return new Vector3(-0.75f, -0.75f, 0);
            yield return new Vector3(0.75f, -0.75f, 0);
        }

        private static IEnumerable<Vector2> CreateTrafficLightUVs()
        {
            yield return new Vector2(0, 1);
            yield return new Vector2(1, 0);
            yield return new Vector2(1, 1);

            yield return new Vector2(0, 1);
            yield return new Vector2(0, 0);
            yield return new Vector2(1, 0);
        }

        /// <summary>
        /// Creates vertices for charaters in a textbox.
        /// Usually should be used in conjuction with CreateCharacterUVs
        /// </summary>
        /// <param name="position">The position of the character in the framebuffer</param>
        /// <param name="width">Width of the character in pixels.</param>
        /// <param name="height">Height of the character in pixels.</param>
        /// <returns>An enumerable of vertices used in character rendering.</returns>
        public static IEnumerable<Vector3> CreateCharacterVertices(Vector2 position, float width, float height)
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

        /// <summary>
        /// Creates UV coordinates for 2D sprites used in text rendering.
        /// Should be used in conjunction with CreateSpriteVertices.
        /// </summary>
        /// <param name="position">The top left corner of the character in the bitmap.</param>
        /// <param name="width">The width of the character (related to the texture width)</param>
        /// <param name="height">The height of the character (related to the texture height)</param>
        /// <returns>An enumerable of the UV coordintes.</returns>
        public static IEnumerable<Vector2> CreateCharacterUVs(Vector2 position, float width, float height)
        {
            // Coordinates are flipped on the Y axis
            return new[]
            {
                new Vector2(position.X, 1.0f - position.Y),
                new Vector2(position.X + width, 1.0f - position.Y),
                new Vector2(position.X, 1.0f - (position.Y + height)),
                new Vector2(position.X, 1.0f - (position.Y + height)),
                new Vector2(position.X + width, 1.0f - position.Y),
                new Vector2(position.X + width, 1.0f - (position.Y + height)),
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

        public static IEnumerable<Vector2> CreateLeftUVs(Vector2 textureStartPosition, float width, float height)
        {
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y + width);
            yield return new Vector2(textureStartPosition.X + height, textureStartPosition.Y + width);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X + height, textureStartPosition.Y + width);
            yield return new Vector2(textureStartPosition.X + height, textureStartPosition.Y);
        }

        public static IEnumerable<Vector2> CreateRightUVs(Vector2 textureStartPosition, float width, float height)
        {
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y + width);
            yield return new Vector2(textureStartPosition.X + height, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y + width);
            yield return new Vector2(textureStartPosition.X + height, textureStartPosition.Y + width);
            yield return new Vector2(textureStartPosition.X + height, textureStartPosition.Y);
        }

        public static IEnumerable<Vector2> CreateFrontUvs(Vector2 textureStartPosition, float width, float height)
        {
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y + height);
            yield return new Vector2(textureStartPosition.X + width, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y + height);
            yield return new Vector2(textureStartPosition.X + width, textureStartPosition.Y + height);
            yield return new Vector2(textureStartPosition.X + width, textureStartPosition.Y);
        }

        public static IEnumerable<Vector2> CreateBackUVs(Vector2 textureStartPosition, float width, float height)
        {
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y + height);
            yield return new Vector2(textureStartPosition.X + width, textureStartPosition.Y + height);
            yield return new Vector2(textureStartPosition.X, textureStartPosition.Y);
            yield return new Vector2(textureStartPosition.X + width, textureStartPosition.Y + height);
            yield return new Vector2(textureStartPosition.X + width, textureStartPosition.Y);
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

        public static IEnumerable<Vector2> CreateCubeUVs(
            Vector2 area,
            float height,
            float windowWidth,
            float windowHeight,
            Vector2[] textureStartPositions,
            float scaleXFrontBack,
            float scaleXLeftRight,
            float scaleWindowHeight)
        {
            // TODO: discretize width and height values when generating buildings
            var UVs = new List<Vector2>();
            UVs.AddRange(
                CreateBackUVs(
                    textureStartPositions[0],
                    windowWidth * area.X / scaleXFrontBack,
                    windowHeight * height / scaleWindowHeight));
            UVs.AddRange(
                CreateFrontUvs(
                    textureStartPositions[1],
                    windowWidth * area.X / scaleXFrontBack,
                    windowHeight * height / scaleWindowHeight));

            UVs.AddRange(
                CreateRightUVs(
                    textureStartPositions[2],
                    windowHeight * height / scaleWindowHeight,
                    windowWidth * area.Y / scaleXLeftRight));

            UVs.AddRange(
                CreateLeftUVs(
                    textureStartPositions[3],
                    windowHeight * height / scaleWindowHeight,
                    windowWidth * area.Y / scaleXLeftRight));

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
