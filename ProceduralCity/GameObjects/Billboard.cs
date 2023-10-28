using System.Collections.Generic;
using OpenTK.Mathematics;

using ProceduralCity.Renderer;

namespace ProceduralCity.GameObjects
{
    class Billboard : IRenderable
    {
        private readonly List<Mesh> _meshes = new();

        public IEnumerable<Mesh> Meshes
        {
            get
            {
                return _meshes;
            }
        }

        public Billboard(Texture texture, Shader shader, IEnumerable<Vector3> vertices, IEnumerable<Vector2> uvs)
        {
            _meshes.Add(new Mesh(vertices, uvs, new[] { texture }, shader));
        }
    }
}
