using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer;

namespace ProceduralCity
{
    class Character : IRenderable
    {
        public IEnumerable<Vector3> Vertices { get; private set; }

        public IEnumerable<Vector2> UVs { get; private set; }

        public Texture Texture { get; private set; }

        public Shader Shader { get; private set; }

        public Character(Shader shader, Texture texture)
        {
            Texture = texture;
            Shader = shader;
        }
    }
}
