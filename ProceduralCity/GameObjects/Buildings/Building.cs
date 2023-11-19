using System.Collections.Generic;
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

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public Building(Vector3 position, Vector2 area, Texture texture, Shader shader, float height)
        {
            _texture = texture;
            _shader = shader;

            _meshes.Add(CreateTexturedCube(position, area, height));
        }

        private Mesh CreateTexturedCube(Vector3 position, Vector2 area, float height)
        {
            return new Mesh(
                PrimitiveUtils.CreateCubeVertices(position, area, height),
                PrimitiveUtils.CreateCubeUVs(),
                new[] { _texture },
                _shader);
        }
    }
}
