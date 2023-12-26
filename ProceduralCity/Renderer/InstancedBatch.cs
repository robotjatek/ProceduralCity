using System;
using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

using ProceduralCity.Renderer.Uniform;

namespace ProceduralCity.Renderer
{
    class InstancedBatch(Shader shader, IEnumerable<ITexture> textures) : IBatch, IDisposable
    {
        private readonly Shader _shader = shader;
        private readonly IEnumerable<ITexture> _textures = textures;
        private readonly List<Vector3> _vertices = [];
        private readonly List<Vector2> _UVs = [];
        private int _instanceCount = 0;

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
        }

        /// <summary>
        /// Updates the Model matrices for the instanced meshes
        /// </summary>
        /// <param name="models">
        /// The model matrices to upload to the GPU
        /// Always a full-sized array, but only the beginning is relevant as everything after the <paramref name="instanceCount"/> is just old junk data</param>
        /// <param name="instanceCount">The usable matrix count at the beginning of the array. Will set the <see cref="_instanceCount"/> member variable too"/></param>
        public void UpdateModels(Matrix4[] models, int instanceCount)
        {
            _instanceCount = instanceCount;
            GL.BindBuffer(BufferTarget.ArrayBuffer, _instancedModelVbo);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _instanceCount * Vector4.SizeInBytes * 4, models);
        }

        public void Draw(Matrix4 projection, Matrix4 view)
        {
            if (!_ready)
            {
                Setup();
            }

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
            GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, Vertices.Length, _instanceCount);
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
            GL.BufferData(BufferTarget.ArrayBuffer, _instanceCount * Vector4.SizeInBytes * 4, IntPtr.Zero, BufferUsageHint.StreamDraw);
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
                GL.DeleteBuffer(_instancedModelVbo);

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

