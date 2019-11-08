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

        public Billboard(Texture texture)
        {
            Vertices = PrimitiveUtils.CreateSpriteVertices(new Vector2(100, 100), 200, 200);
            UVs = PrimitiveUtils.CreateGuiUVs();
            Shader = new Shader("vs.vert", "fs.frag");  //TODO: dispose shader
            Texture = texture;
        }
    }
}
