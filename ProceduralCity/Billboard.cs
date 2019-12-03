using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    class Billboard : IRenderable
    {
        private readonly List<Mesh> _meshes = new List<Mesh>();

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
