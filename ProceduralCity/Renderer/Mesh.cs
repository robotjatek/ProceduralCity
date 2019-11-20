using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer
{
    class Mesh
    {
        public IEnumerable<Vector3> Vertices { get; private set; }
        public IEnumerable<Vector2> UVs { get; private set; }
        public ITexture Texture { get; private set; }
        public Shader Shader { get; private set; }

        public Mesh(IEnumerable<Vector3> vertices, IEnumerable<Vector2> uvs, ITexture texture, Shader shader)
        {
            Vertices = vertices;
            UVs = uvs;
            Texture = texture;
            Shader = shader;
        }
    }
}
