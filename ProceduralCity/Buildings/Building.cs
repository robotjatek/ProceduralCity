using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.Buildings
{
    class Building : IBuilding
    {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _UVs = new List<Vector2>();

        public ITexture Texture
        {
            get; private set;
        }

        public Shader Shader
        {
            get; private set;
        }

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

        public bool HasBillboard { get; private set; } = false;

        public Billboard Billboard => throw new System.NotImplementedException();

        public Building(Vector3 position, Vector2 area, Texture texture, Shader shader, float height)
        {
            Texture = texture;
            Shader = shader;

            CreateTexturedCube(position, area, height);
        }

        private void CreateTexturedCube(Vector3 position, Vector2 area, float height)
        {
            _UVs.AddRange(PrimitiveUtils.CreateCubeUVs());
            _vertices.AddRange(PrimitiveUtils.CreateCubeVertices(position, area, height));
        }
    }
}
