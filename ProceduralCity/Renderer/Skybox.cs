using System;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer
{
    class Skybox : IDisposable
    {
        private readonly int _vaoId;
        private readonly int _vboId;
        private readonly Texture _texture;
        private readonly Shader _shader;

        private readonly int _projectionMatrixLocation;
        private readonly int _viewMatrixLocation;
        private readonly int _skyboxLocation;

        private readonly Vector3[] vertices = new[]
        {
           new Vector3(-1.0f,  1.0f, -1.0f),
            new Vector3(-1.0f, -1.0f, -1.0f),
            new Vector3(1.0f, -1.0f, -1.0f),
           new Vector3( 1.0f, -1.0f, -1.0f),
            new Vector3(1.0f,  1.0f, -1.0f),
          new Vector3(  -1.0f,  1.0f, -1.0f),

          new Vector3(  -1.0f, -1.0f,  1.0f),
          new Vector3(  -1.0f, -1.0f, -1.0f),
           new Vector3( -1.0f,  1.0f, -1.0f),
         new Vector3(   -1.0f,  1.0f, -1.0f),
         new Vector3(   -1.0f,  1.0f,  1.0f),
         new Vector3(   -1.0f, -1.0f,  1.0f),

        new Vector3(    1.0f, -1.0f, -1.0f),
        new Vector3(    1.0f, -1.0f,  1.0f),
        new Vector3(    1.0f,  1.0f,  1.0f),
        new Vector3(    1.0f,  1.0f,  1.0f),
        new Vector3(    1.0f,  1.0f, -1.0f),
        new Vector3(    1.0f, -1.0f, -1.0f),

         new Vector3(   -1.0f, -1.0f,  1.0f),
        new Vector3(    -1.0f,  1.0f,  1.0f),
        new Vector3(    1.0f,  1.0f,  1.0f),
        new Vector3(    1.0f,  1.0f,  1.0f),
        new Vector3(    1.0f, -1.0f,  1.0f),
        new Vector3(    -1.0f, -1.0f,  1.0f),

         new Vector3(   -1.0f,  1.0f, -1.0f),
        new Vector3(    1.0f,  1.0f, -1.0f),
        new Vector3(    1.0f,  1.0f,  1.0f),
        new Vector3(    1.0f,  1.0f,  1.0f),
        new Vector3(    -1.0f,  1.0f,  1.0f),
        new Vector3(    -1.0f,  1.0f, -1.0f),

         new Vector3(   -1.0f, -1.0f, -1.0f),
         new Vector3(   -1.0f, -1.0f,  1.0f),
         new Vector3(   1.0f, -1.0f, -1.0f),
         new Vector3(   1.0f, -1.0f, -1.0f),
         new Vector3(   -1.0f, -1.0f,  1.0f),
        new Vector3(    1.0f, -1.0f,  1.0f)
        };

        public Skybox()
        {
            var filenames = new[]
            {
                "skybox/hq/right.jpg",
                "skybox/hq/left.jpg",
                "skybox/hq/top.jpg",
                "skybox/hq/bottom.jpg",
                "skybox/hq/back.jpg",
                "skybox/hq/front.jpg",
            };

            _texture = new Texture(filenames.ToList());
            _shader = new Shader("skybox.vert", "skybox.fs");
            _projectionMatrixLocation = GL.GetUniformLocation(_shader.ProgramId, "projection");
            _viewMatrixLocation = GL.GetUniformLocation(_shader.ProgramId, "view");
            _skyboxLocation = GL.GetUniformLocation(_shader.ProgramId, "skybox");

            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            _vboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindVertexArray(0);
        }

        public void Render(Matrix4 proj, Matrix4 view)
        {
            GL.DepthFunc(DepthFunction.Lequal);
            GL.CullFace(CullFaceMode.Front);
            _shader.Use();

            var skyView = new Matrix4(new Matrix3(view));

            GL.UniformMatrix4(_projectionMatrixLocation, false, ref proj);
            GL.UniformMatrix4(_viewMatrixLocation, false, ref skyView);

            GL.BindVertexArray(_vaoId);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.Uniform1(_skyboxLocation, 0);
            GL.BindTexture(TextureTarget.TextureCubeMap, _texture.Id);
            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length);
            GL.BindVertexArray(0);

            GL.CullFace(CullFaceMode.Back);
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _shader.Dispose();
                    _texture.Dispose();
                }

                GL.DeleteVertexArray(_vaoId);
                GL.DeleteBuffer(_vboId);

                disposedValue = true;
            }
        }

        ~Skybox()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
