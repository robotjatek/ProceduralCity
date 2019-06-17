using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ProceduralCity.Renderer
{
    class ObjectBatch : IDisposable
    {
        private bool disposedValue = false;

        private readonly Shader _shader;
        private readonly Texture _texture;
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _UVs = new List<Vector2>();

        private bool _ready = false;
        private int _vaoId;
        private int _vertexVboId;
        private int _uvVboId;
        private int _mvpLocation;
        private int _textureLocation;

        public ObjectBatch(Shader shader, Texture texture)
        {
            _shader = shader;
            _texture = texture;
        }

        public void AddRenderable(IRenderable r)
        {
            _vertices.AddRange(r.GetVertices());
            _UVs.AddRange(r.GetUVs());
        }

        public void Draw(Matrix4 mvp)
        {
            if (!_ready)
            {
                Setup();
            }

            if (_texture != null)
            {
                GL.BindTexture(TextureTarget.Texture2D, _texture.Id);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.Uniform1(_textureLocation, 0);
            }

            GL.UniformMatrix4(_mvpLocation, false, ref mvp);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count);
            GL.BindVertexArray(0); //unbind vao
        }

        private void Setup()
        {
            var vertexLayoutId = 0;
            var uvLayoutId = 2;

            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            _vertexVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count * Vector3.SizeInBytes, _vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertexLayoutId, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(vertexLayoutId);

            _uvVboId = GL.GenVertexArray();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _uvVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, _UVs.Count * Vector2.SizeInBytes, _UVs.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(uvLayoutId, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(uvLayoutId);

            GL.BindVertexArray(0);
            _ready = true;
            _shader.Use();
            if (_texture != null)
            {
                _textureLocation = GL.GetUniformLocation(_shader.ProgramId, "tex");
            }
            _mvpLocation = GL.GetUniformLocation(_shader.ProgramId, "mvp");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                GL.DeleteVertexArray(_vaoId);
                GL.DeleteBuffer(_vertexVboId);
                GL.DeleteBuffer(_uvVboId);

                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~ObjectBatch()
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
