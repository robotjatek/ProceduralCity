using System.Collections.Generic;
using OpenTK;

namespace ProceduralCity.Renderer.Utils
{
    class FullScreenQuad : IRenderable
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

        public FullScreenQuad(Texture texture)
        {
            Texture = texture;
            Shader = new Shader("vs.vert", "fs.frag"); //TODO: dispose
            UVs = PrimitiveUtils.CreateNDCFullscreenUVs();
            Vertices = PrimitiveUtils.CreateNDCFullscreenGuiVertices();
        }
    }
}
