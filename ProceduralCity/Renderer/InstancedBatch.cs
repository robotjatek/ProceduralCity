using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using ProceduralCity.Renderer.Uniform;

namespace ProceduralCity.Renderer
{
    // TODO: remove and add objects dynamically
    // TODO: do not update matrices for culled objects
    class InstancedBatch(Shader shader, IEnumerable<ITexture> textures) : IBatch, IDisposable
    {
        private readonly Shader _shader = shader;
        private readonly IEnumerable<ITexture> _textures = textures;
        private readonly List<Vector3> _vertices = [];
        private readonly List<Vector2> _UVs = [];

        private readonly List<Mesh> _meshes = [];
        private Matrix4[] _instanceModelMatrixValues; // Temp array for model matrix updates.
        private bool disposedValue;
        private bool _ready = false;
        private int _vaoId;
        private int _vertexVboId;
        private int _uvVboId;
        private int _instancedModelVbo;

        private Vector3[] Vertices { get; set; }

        private Vector2[] UVs { get; set; }

        public void AddMesh(Mesh m)
        {
            if (_ready)
            {
                throw new InvalidOperationException("This batch is already initialized. You cannot add more instances to it");
            }

            if (_vertices.Count == 0)
            {
                _vertices.AddRange(m.Vertices);
                _UVs.AddRange(m.UVs);
            }
            _meshes.Add(m);

            // TODO: dynamically update the instance models in every frame to only contain data for meshes that are not culled
        }

        public void Draw(Matrix4 projection, Matrix4 view)
        {
            if (!_ready)
            {
                Setup();
            }

            SetupInstancedArray();

            GL.BindVertexArray(_vaoId);
            var textureOffset = 0;
            foreach (var texture in _textures)
            {
                texture.Bind(TextureUnit.Texture0 + textureOffset);
                textureOffset++;
            }

            _shader.SetUniformValue("_projection", new Matrix4Uniform
            {
                Value = projection
            });
            _shader.SetUniformValue("_view", new Matrix4Uniform
            {
                Value = view
            });

            _shader.Use();
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, Vertices.Length, _meshes.Count);
            GL.BindVertexArray(0);
        }

        private void Setup()
        {
            var vertexLayoutId = 0;
            var uvLayoutId = 2;

            Vertices = [.. _vertices];
            _vertices.Clear();
            UVs = [.. _UVs];
            _UVs.Clear();
            _instanceModelMatrixValues = new Matrix4[_meshes.Count];

            _vaoId = GL.GenVertexArray();
            GL.BindVertexArray(_vaoId);

            _vertexVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexVboId);
            GL.EnableVertexAttribArray(vertexLayoutId);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Vector3.SizeInBytes, Vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertexLayoutId, 3, VertexAttribPointerType.Float, false, 0, 0);

            _uvVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _uvVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, UVs.Length * Vector2.SizeInBytes, UVs, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(uvLayoutId);
            GL.VertexAttribPointer(uvLayoutId, 2, VertexAttribPointerType.Float, false, 0, 0);

            _instancedModelVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instancedModelVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _meshes.Count * Vector4.SizeInBytes * 4, IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.EnableVertexAttribArray(3);
            GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, 4 * Vector4.SizeInBytes, 0);
            GL.VertexAttribDivisor(3, 1);

            GL.EnableVertexAttribArray(4);
            GL.VertexAttribPointer(4, 4, VertexAttribPointerType.Float, false, 4 * Vector4.SizeInBytes, 16);
            GL.VertexAttribDivisor(4, 1);

            GL.EnableVertexAttribArray(5);
            GL.VertexAttribPointer(5, 4, VertexAttribPointerType.Float, false, 4 * Vector4.SizeInBytes, 32);
            GL.VertexAttribDivisor(5, 1);

            GL.EnableVertexAttribArray(6);
            GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, false, 4 * Vector4.SizeInBytes, 48);
            GL.VertexAttribDivisor(6, 1);

            GL.BindVertexArray(0);
            _ready = true;
            _shader.Use();
        }

        private void SetupInstancedArray()
        {
            Parallel.For(0, _meshes.Count, (i) =>
            {
                _instanceModelMatrixValues[i] = _meshes[i].Model;
            });

            GL.BindBuffer(BufferTarget.ArrayBuffer, _instancedModelVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _instanceModelMatrixValues.Length * Vector4.SizeInBytes * 4, _instanceModelMatrixValues);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // no managed objects to dispose
                }

                GL.DeleteVertexArray(_vaoId);
                GL.DeleteBuffer(_vertexVboId);
                GL.DeleteBuffer(_uvVboId);

                disposedValue = true;
            }
        }

        ~InstancedBatch()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

