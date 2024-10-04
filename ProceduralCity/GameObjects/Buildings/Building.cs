using System;
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

        public IReadOnlyCollection<Mesh> Meshes => _meshes.AsReadOnly();

        public Building(Vector3 position, Vector2 area, BuildingTextureInfo buildingTextureInfo, Shader shader, float height, RandomService randomService)
        {
            _randomService = randomService;
            _texture = buildingTextureInfo.Texture;
            _shader = shader;

            _meshes.Add(CreateTexturedCube(position, area, height, buildingTextureInfo));
        }

        private Mesh CreateTexturedCube(Vector3 position, Vector2 area, float height, BuildingTextureInfo buildingTextureInfo)
        {
            // TODO: dynamic texture scale
            var textureScale = 4;

            var windowWidth = buildingTextureInfo.WindowWidth / textureScale;
            var windowHeight = buildingTextureInfo.WindowHeight / textureScale;

            // Discretize height to fit window size 
            height = (float)Math.Floor(height / textureScale) * textureScale;

            // Different texture parts for each sides
            Vector2[] textureStartPositions =
            [
                buildingTextureInfo.RandomWindowUV(),
                buildingTextureInfo.RandomWindowUV(),
                buildingTextureInfo.RandomWindowUV(),
                buildingTextureInfo.RandomWindowUV(),
            ];

            var scaleXFrontBackTemp = area.X * windowWidth * 4;
            var scaleXFrontBack = 1;

            var scaleXLeftRightTemp = area.Y * windowWidth * 4;
            var scaleXLeftRight = 1;

            // TODO: scale Y frontback & scaleYleftright -- a window height valójában scaleY és mindkét oldalra külön kell
            var scaleYFrontBack = scaleXFrontBack; 
            var scaleYLeftRight = scaleXLeftRight;

            return new Mesh(
                PrimitiveUtils.CreateCubeVertices(position, area, height),
                PrimitiveUtils.CreateCubeUVs(
                    area,
                    height,
                    windowWidth,
                    windowHeight,
                    textureStartPositions,
                    scaleXFrontBack: scaleXFrontBack,
                    scaleXLeftRight: scaleXLeftRight,
                    scaleYFrontBack,
                    scaleYLeftRight),
                new[] { _texture },
                _shader);
        }
    }
}
