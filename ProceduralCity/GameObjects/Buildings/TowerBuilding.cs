using System.Collections.Generic;
using OpenTK.Mathematics;

using ProceduralCity.Extensions;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Buildings
{
    class TowerBuilding : IBuilding
    {
        private readonly List<Mesh> _meshes = [];
        private readonly Shader _shader;
        private readonly ITexture _texture;
        private readonly RandomService _randomService;

        public IEnumerable<Mesh> Meshes => _meshes;

        public TowerBuilding(Vector3 position, Vector2 area, BuildingTextureInfo texture, Shader shader, float height, IBillboardBuilder billboardBuilder, RandomService randomService)
        {
            _texture = texture.Texture;
            _shader = shader;
            _randomService = randomService;

            var blockToppingArea = new Vector2(area.X + 3, area.Y + 3);
            var lastPosition = new Vector3(position);
            var billboardPosition = new Vector3();

            while (height - 8 > 0)
            {
                var blockToppingPosition = new Vector3(lastPosition.X - 1.5f, lastPosition.Y, lastPosition.Z - 1.5f);
                blockToppingPosition.Y += 5;
                billboardPosition = blockToppingPosition;

                _meshes.Add(CreateTexturedCube(lastPosition, area, 5, texture));
                _meshes.Add(CreateUntexturedCube(blockToppingPosition, blockToppingArea, 3));

                lastPosition.Y += 8;
                height -= 8;
            }

            CreateBillboardIfEliglible(billboardBuilder, billboardPosition, blockToppingArea);
        }

        private void CreateBillboardIfEliglible(IBillboardBuilder builder, Vector3 position, Vector2 area)
        {
            if (builder.HasBillboards())
            {
                var northSouthSideLength = area.X;
                var eastWestSideLength = area.Y;
                if (northSouthSideLength > eastWestSideLength)
                {
                    if (northSouthSideLength >= builder.CalculateBillboardWidth(3))
                    {
                        var billboard = _randomService.FlipCoin() ?
                            builder.CreateNorthFacingBillboard(position, area, 3) :
                            builder.CreateSouthFacingBillboard(position, area, 3);

                        _meshes.AddRange(billboard.Meshes);
                    }
                }
                else
                {
                    if (eastWestSideLength >= builder.CalculateBillboardWidth(3))
                    {
                        var billboard = _randomService.FlipCoin() ?
                            builder.CreateWestFacingBillboard(position, area, 3) :
                            builder.CreateEastFacingBillboard(position, area, 3);

                        _meshes.AddRange(billboard.Meshes);
                    }
                }
            }
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

            var scaleWindowHeight = height / 2; // Two floors per section
            var scaleXFrontBackTemp = area.X * windowWidth;
            var scaleXFrontBack = scaleXFrontBackTemp.Clamp(1, scaleXFrontBackTemp); // Make sure that the window does not overflow on the sides by scaling it to an integer value

            var scaleXLeftRightTemp = area.Y * windowWidth;
            var scaleXLeftRight = scaleXLeftRightTemp.Clamp(1, scaleXLeftRightTemp);

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
                    scaleWindowHeight),
                new[] { _texture },
                _shader);
        }

        private Mesh CreateUntexturedCube(Vector3 position, Vector2 area, float height)
        {
            return new Mesh(
                PrimitiveUtils.CreateCubeVertices(position, area, height),
                PrimitiveUtils.CreateZeroCubeUVs(),
                new[] { _texture },
                _shader);
        }
    }
}
