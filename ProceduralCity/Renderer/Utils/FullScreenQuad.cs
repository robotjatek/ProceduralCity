using System;
using System.Collections.Generic;
using OpenTK;
using ProceduralCity.Renderer.Uniform;

namespace ProceduralCity.Renderer.Utils
{
    class FullScreenQuad : IRenderable, IDisposable
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
            Shader = new Shader("vs.vert", "fs.frag");
            Shader.SetUniformValue("tex", new IntUniform
            {
                Value = 0
            });
            UVs = PrimitiveUtils.CreateNDCFullscreenUVs();
            Vertices = PrimitiveUtils.CreateNDCFullscreenGuiVertices();
        }

        public void Dispose()
        {
            Shader.Dispose();
        }
    }
}
