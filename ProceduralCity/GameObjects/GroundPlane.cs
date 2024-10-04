using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;

using ProceduralCity.Renderer;
using ProceduralCity.Utils;

namespace ProceduralCity.GameObjects
{
    class GroundPlane : IRenderable
    {
        private readonly List<Mesh> _meshes = [];

        public IReadOnlyCollection<Mesh> Meshes => _meshes.AsReadOnly();

        public GroundPlane(Vector3 position, Vector2 size, Shader shader)
        {
            _meshes.Add(new Mesh(
                PrimitiveUtils.CreateTopVertices(position, size, 0),
                Enumerable.Empty<Vector2>(), // untextured flat colored plane
                shader));
        }
    }
}
