using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ProceduralCity.Renderer.Uniform;

namespace ProceduralCity.Renderer
{
    class ObjectBatch : IDisposable
    {
        private bool disposedValue = false;

        private readonly Shader _shader;
        private readonly Texture _texture;
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector2> _UVs = new List<Vector2>();

        private Vector3[] Vertices { get; set; }
        private Vector2[] UVs { get; set; }

        private bool _ready = false;
        private int _vaoId;
        private int _vertexVboId;
        private int _uvVboId;

        public ObjectBatch(Shader shader, Texture texture)
        {
            _shader = shader;
            _texture = texture;
        }

        public void AddRenderable(IRenderable r)
        {
            _vertices.AddRange(r.Vertices);
            _UVs.AddRange(r.UVs);
        }

        public void Draw(Matrix4 projection, Matrix4 view, Matrix4 model)
        {
            if (!_ready)
            {
                Setup();
            }

            GL.BindVertexArray(_vaoId);

            if (_texture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, _texture.Id);
            }

            _shader.SetUniformValue("_projection", new Matrix4Uniform
            {
                Value = projection
            });
            _shader.SetUniformValue("_view", new Matrix4Uniform
            {
                Value = view
            });
            _shader.SetUniformValue("_model", new Matrix4Uniform
            {
                Value = model
            });
            _shader.Use();

            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
            GL.BindVertexArray(0);
        }

        private void Setup()
        {
            var vertexLayoutId = 0;
            var uvLayoutId = 2;

            Vertices = _vertices.ToArray();
            UVs = _UVs.ToArray();

            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            _vertexVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Vector3.SizeInBytes, Vertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(vertexLayoutId);
            GL.VertexAttribPointer(vertexLayoutId, 3, VertexAttribPointerType.Float, false, 0, 0);

            _uvVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _uvVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, _UVs.Count * Vector2.SizeInBytes, UVs, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(uvLayoutId, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(uvLayoutId);

            GL.BindVertexArray(0);
            _ready = true;
            _shader.Use();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No managed objects to dispose at the moment
                }

                GL.DeleteVertexArray(_vaoId);
                GL.DeleteBuffer(_vertexVboId);
                GL.DeleteBuffer(_uvVboId);

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
