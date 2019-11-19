using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    class Billboard : IRenderable
    {
        public IEnumerable<Vector3> Vertices
        {
            get;
            private set;
        }

        public IEnumerable<Vector2> UVs
        {
            get;
            private set;
        }

        public ITexture Texture
        {
            get;
            private set;
        }

        public Shader Shader
        {
            get;
            private set;
        }

        public Billboard(Texture texture, Shader shader, IEnumerable<Vector3> vertices, IEnumerable<Vector2> uvs)
        {
            Vertices = vertices;
            UVs = uvs;
            Shader = shader;
            Texture = texture;
        }
    }
}
