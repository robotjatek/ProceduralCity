using System;
using System.Collections.Generic;

using OpenTK.Mathematics;

using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Buildings;

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

        var textureScale = _randomService.Next(2, 10);
        var windowWidth = buildingTextureInfo.WindowWidth;
        var windowHeight = buildingTextureInfo.WindowHeight;

        // Discretize height to fit window size
        height = (float)Math.Floor(height / textureScale) * textureScale;
        // Discretize area to fit window size
        area.X = (float)Math.Floor(area.X / textureScale) * textureScale;
        area.Y = (float)Math.Floor(area.Y / textureScale) * textureScale;

        // Different texture parts for each sides
        Vector2[] textureStartPositions =
        [
            buildingTextureInfo.RandomWindowUV(),
            buildingTextureInfo.RandomWindowUV(),
            buildingTextureInfo.RandomWindowUV(),
            buildingTextureInfo.RandomWindowUV(),
        ];

        return new Mesh(
            PrimitiveUtils.CreateCubeVertices(position, area, height),
            PrimitiveUtils.CreateCubeUVs(
                area,
                height,
                windowWidth,
                windowHeight,
                textureStartPositions,
                scaleXFrontBack: textureScale,
                scaleXLeftRight: textureScale,
                textureScale,
                textureScale),
            [_texture],
            _shader);
    }
}
