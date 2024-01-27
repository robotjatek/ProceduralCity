using System.Collections.Generic;

using OpenTK.Mathematics;

using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Buildings
{
    class Building : IBuilding
    {
        private readonly List<Mesh> _meshes = [];
        private readonly ITexture _texture;
        private readonly Shader _shader;
        private readonly RandomService _randomService;

        public IEnumerable<Mesh> Meshes => _meshes;

        public Building(Vector3 position, Vector2 area, BuildingTextureInfo buildingTextureInfo, Shader shader, float height, RandomService randomService)
        {
            _randomService = randomService;
            _texture = buildingTextureInfo.Texture;
            _shader = shader;

            _meshes.Add(CreateTexturedCube(position, area, height, buildingTextureInfo));
        }

        private Mesh CreateTexturedCube(Vector3 position, Vector2 area, float height, BuildingTextureInfo buildingTextureInfo)
        {
            var windowWidth = buildingTextureInfo.WindowWidth;
            var windowHeight = buildingTextureInfo.WindowHeight;

            Vector2[] textureStartPositions =
            [
                buildingTextureInfo.RandomWindowUV(),
                buildingTextureInfo.RandomWindowUV(),
                buildingTextureInfo.RandomWindowUV(),
                buildingTextureInfo.RandomWindowUV(),
            ];

            var scaleX = 2.5f;
            var scaleWindowHeight = 2.5f;

            return new Mesh(
                PrimitiveUtils.CreateCubeVertices(position, area, height),
                PrimitiveUtils.CreateCubeUVs(
                    area,
                    height,
                    windowWidth,
                    windowHeight,
                    textureStartPositions,
                    scaleXFrontBack: scaleX,
                    scaleXLeftRight: scaleX, // TODO: 
                    scaleWindowHeight),
                new[] { _texture },
                _shader);
        }
    }
}
