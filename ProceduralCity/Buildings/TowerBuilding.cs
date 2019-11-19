using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Generators;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Buildings
{
    class TowerBuilding : IBuilding
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();

        private readonly List<Vector2> _UVs = new List<Vector2>();

        public IEnumerable<Vector3> Vertices
        {
            get
            {
                return _vertices;
            }
        }

        public IEnumerable<Vector2> UVs
        {
            get
            {
                return _UVs;
            }
        }

        public ITexture Texture { get; private set; }

        public Shader Shader { get; private set; }

        public bool HasBillboard { get; private set; } = true;

        public Billboard Billboard { get; private set; }

        public TowerBuilding(Vector3 position, Vector2 area, Texture texture, Shader shader, float height, IBillboardBuilder billboardBuilder)
        {
            Texture = texture;
            Shader = shader;

            var blockToppingArea = new Vector2(area.X + 3, area.Y + 3);
            var lastPosition = new Vector3(position);
            var billboardPosition = new Vector3();

            while (height - 8 > 0)
            {
                var blockToppingPosition = new Vector3(lastPosition.X - 1.5f, lastPosition.Y, lastPosition.Z - 1.5f);
                blockToppingPosition.Y += 5;
                billboardPosition = blockToppingPosition;

                CreateTexturedCube(lastPosition, area, 5);
                CreateUntexturedCube(blockToppingPosition, blockToppingArea, 3);

                lastPosition.Y += 8;
                height -= 8;
            }

            Billboard = GenerateBillboard(billboardBuilder, billboardPosition, blockToppingArea);
        }

        private Billboard GenerateBillboard(IBillboardBuilder builder, Vector3 position, Vector2 area)
        {
            var northSouthSideLength = area.X;
            var eastWestSideLength = area.Y;
            if (northSouthSideLength > eastWestSideLength)
            {
                if (northSouthSideLength < builder.CalculateBillboardWidth(3))
                {
                    HasBillboard = false;
                    return null;
                }

                return CoinFlip.Flip() ?
                    builder.CreateNorthFacingBillboard(position, area, 3) :
                    builder.CreateSouthFacingBillboard(position, area, 3);
            }
            else
            {
                if (eastWestSideLength < builder.CalculateBillboardWidth(3))
                {
                    HasBillboard = false;
                    return null;
                }

                return CoinFlip.Flip() ?
                    builder.CreateWestFacingBillboard(position, area, 3) :
                    builder.CreateEastFacingBillboard(position, area, 3);
            }
        }

        private void CreateTexturedCube(Vector3 position, Vector2 area, float height)
        {
            _UVs.AddRange(PrimitiveUtils.CreateCubeUVs());
            _vertices.AddRange(PrimitiveUtils.CreateCubeVertices(position, area, height));
        }

        private void CreateUntexturedCube(Vector3 position, Vector2 area, float height)
        {
            _UVs.AddRange(PrimitiveUtils.CreateZeroCubeUVs());
            _vertices.AddRange(PrimitiveUtils.CreateCubeVertices(position, area, height));
        }
    }
}
