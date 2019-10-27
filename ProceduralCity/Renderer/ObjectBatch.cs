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
        private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();
        private readonly UniformHandler _uniformHandler = new UniformHandler();

        private Vector3[] Vertices { get; set; }
        private Vector2[] UVs { get; set; }

        private bool _ready = false;
        private int _vaoId;
        private int _vertexVboId;
        private int _uvVboId;
        private int _projectionLocation;
        private int _viewLocation;
        private int _modelLocation;
        private int _textureLocation;

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

            _shader.Use();
            GL.BindVertexArray(_vaoId);

            SetUniforms();

            if (_texture != null)
            {
                GL.BindTexture(TextureTarget.Texture2D, _texture.Id);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.Uniform1(_textureLocation, 0);
            }

            GL.UniformMatrix4(_projectionLocation, false, ref projection);
            GL.UniformMatrix4(_viewLocation, false, ref view);
            GL.UniformMatrix4(_modelLocation, false, ref model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
            GL.BindVertexArray(0);
        }

        private void SetUniforms()
        {
            var uniforms = _shader.Uniforms;
            foreach (var uniform in uniforms)
            {
                if (_uniformLocations.TryGetValue(uniform.Key, out int location) == false)
                {
                    location = GL.GetUniformLocation(_shader._programId, uniform.Key);
                    _uniformLocations.Add(uniform.Key, location);
                }

                if (location != -1)
                {
                    uniform.Value.Visit(location, _uniformHandler);
                }
            }
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
            if (_texture != null)
            {
                _textureLocation = GL.GetUniformLocation(_shader._programId, "tex");
            }
            _projectionLocation = GL.GetUniformLocation(_shader._programId, "_projection");
            _viewLocation = GL.GetUniformLocation(_shader._programId, "_view");
            _modelLocation = GL.GetUniformLocation(_shader._programId, "_model");
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
