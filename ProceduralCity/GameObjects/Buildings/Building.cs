﻿using System.Collections.Generic;

using OpenTK.Mathematics;

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

        public Building(Vector3 position, Vector2 area, Texture texture, Shader shader, float height, RandomService randomService)
        {
            _randomService = randomService;
            _texture = texture;
            _shader = shader;

            _meshes.Add(CreateTexturedCube(position, area, height));
        }

        private Mesh CreateTexturedCube(Vector3 position, Vector2 area, float height)
        {
            var numWindowsX = 128;
            var numWindowsY = 128;
            var windowWidth = 1f / numWindowsX;
            var windowHeight = 1f / numWindowsY;
            var windowX = _randomService.Next(0, numWindowsX);
            var windowY = _randomService.Next(0, numWindowsY);

            var startX = (float)windowX / numWindowsX;
            var startY = (float)windowY / numWindowsY;
            var textureStartPosition = new Vector2(startX, startY);
            var scaleX = 2.5f;
            var scaleY = 2.5f;

            return new Mesh(
                PrimitiveUtils.CreateCubeVertices(position, area, height),
                PrimitiveUtils.CreateCubeUVs(
                    area,
                    height,
                    windowWidth,
                    windowHeight,
                    textureStartPosition,
                    scaleXFrontBack: scaleX,
                    scaleXLeftRight: scaleX, // TODO: 
                    scaleY),
                new[] { _texture },
                _shader);
        }
    }
}
